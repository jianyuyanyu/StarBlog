using StarBlog.Data.Models;

namespace StarBlog.Api.Services.OutboxServices;

public interface IOutboxHandler {
    string Type { get; }

    Task HandleAsync(OutboxMessage message, CancellationToken cancellationToken);
}
