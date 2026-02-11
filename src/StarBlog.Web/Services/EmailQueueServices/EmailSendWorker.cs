using StarBlog.Web.Services;

namespace StarBlog.Web.Services.EmailQueueServices;

public class EmailSendWorker : BackgroundService {
    private const int MaxAttempts = 5;
    private static readonly TimeSpan MaxBackoff = TimeSpan.FromMinutes(10);

    private readonly ILogger<EmailSendWorker> _logger;
    private readonly EmailSendQueueService _queue;
    private readonly EmailService _emailService;

    public EmailSendWorker(ILogger<EmailSendWorker> logger, EmailSendQueueService queue, EmailService emailService) {
        _logger = logger;
        _queue = queue;
        _emailService = emailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            EmailQueueItem item;
            try {
                item = await _queue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException) {
                break;
            }

            try {
                await _emailService.SendEmailAsync(item.Subject, item.HtmlBody, item.ToName, item.ToAddress);
                _logger.LogInformation("邮件队列 发送成功：{EmailId}，收件人：{ToAddress}，主题：{Subject}，尝试：{Attempt}",
                    item.Id, item.ToAddress, item.Subject, item.Attempt + 1);
            }
            catch (Exception ex) {
                var nextAttempt = item.Attempt + 1;
                if (nextAttempt >= MaxAttempts) {
                    _logger.LogError(ex, "邮件队列 发送失败（放弃）：{EmailId}，收件人：{ToAddress}，主题：{Subject}，已尝试：{Attempt}",
                        item.Id, item.ToAddress, item.Subject, nextAttempt);
                    continue;
                }

                var delay = GetBackoffDelay(nextAttempt);
                var retryItem = item with { Attempt = nextAttempt };
                _logger.LogWarning(ex, "邮件队列 发送失败，将在 {Delay} 后重试：{EmailId}，收件人：{ToAddress}，主题：{Subject}，下一次尝试：{Attempt}",
                    delay, item.Id, item.ToAddress, item.Subject, nextAttempt + 1);

                _ = Task.Run(async () => {
                    try {
                        await Task.Delay(delay, stoppingToken);
                        _queue.Enqueue(retryItem);
                    }
                    catch (OperationCanceledException) {
                    }
                }, CancellationToken.None);
            }
        }
    }

    private static TimeSpan GetBackoffDelay(int attempt) {
        var baseSeconds = Math.Min(Math.Pow(2, attempt), MaxBackoff.TotalSeconds);
        var jitterMs = Random.Shared.Next(0, 500);
        return TimeSpan.FromSeconds(baseSeconds) + TimeSpan.FromMilliseconds(jitterMs);
    }
}
