namespace StarBlog.Web.Services.EmailQueueServices;

public sealed record EmailQueueItem {
    public required Guid Id { get; init; }
    public required string Subject { get; init; }
    public required string HtmlBody { get; init; }
    public required string ToName { get; init; }
    public required string ToAddress { get; init; }
    public int Attempt { get; init; }
    public DateTimeOffset EnqueuedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
