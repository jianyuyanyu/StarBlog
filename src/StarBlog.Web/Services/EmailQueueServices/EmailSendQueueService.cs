using System.Collections.Concurrent;

namespace StarBlog.Web.Services.EmailQueueServices;

public class EmailSendQueueService {
    private readonly ConcurrentQueue<EmailQueueItem> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);
    private readonly ILogger<EmailSendQueueService> _logger;

    public EmailSendQueueService(ILogger<EmailSendQueueService> logger) {
        _logger = logger;
    }

    public EmailQueueItem Enqueue(string subject, string htmlBody, string toName, string toAddress) {
        var item = new EmailQueueItem {
            Id = Guid.NewGuid(),
            Subject = subject,
            HtmlBody = htmlBody,
            ToName = toName,
            ToAddress = toAddress,
            Attempt = 0,
        };

        Enqueue(item);
        return item;
    }

    public void Enqueue(EmailQueueItem item) {
        _queue.Enqueue(item);
        _signal.Release();
        _logger.LogDebug("邮件队列 入队：{EmailId}，收件人：{ToAddress}，主题：{Subject}", item.Id, item.ToAddress, item.Subject);
    }

    public async ValueTask<EmailQueueItem> DequeueAsync(CancellationToken cancellationToken) {
        while (true) {
            await _signal.WaitAsync(cancellationToken);
            if (_queue.TryDequeue(out var item)) {
                return item;
            }
        }
    }
}
