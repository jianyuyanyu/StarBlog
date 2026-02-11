using FreeSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StarBlog.Data;
using StarBlog.Data.Models;
using StarBlog.Web.Services;
using StarBlog.Web.Services.OutboxServices;

var services = new ServiceCollection();

services.AddLogging(builder => {
    builder.ClearProviders();
    builder.AddSimpleConsole(options => { options.SingleLine = true; });
    builder.SetMinimumLevel(LogLevel.Information);
});

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?> { ["host"] = "https://example.test" })
    .Build();
services.AddSingleton<IConfiguration>(configuration);

var dbPath = Path.Combine(AppContext.BaseDirectory, "comment-reply-smoke.db");
if (File.Exists(dbPath)) File.Delete(dbPath);

var fsql = FreeSqlFactory.Create($"Data Source={dbPath};");
services.AddSingleton<IFreeSql>(fsql);
services.AddFreeRepository();

services.AddMemoryCache();
services.Configure<OutboxOptions>(_ => { });
services.AddScoped<OutboxService>();
services.AddScoped<EmailService>();
services.AddScoped<CommentService>();
services.AddScoped<IOutboxHandler, NoopEmailHandler>();

var sp = services.BuildServiceProvider();

using var scope = sp.CreateScope();
var outboxRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();
var commentRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<Comment>>();
var anonymousRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<AnonymousUser>>();
var postRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<Post>>();
var commentService = scope.ServiceProvider.GetRequiredService<CommentService>();

var post = new Post {
    Id = "p1",
    Title = "Hello",
    Slug = "hello",
    IsPublish = true,
    CreationTime = DateTime.Now,
    LastUpdateTime = DateTime.Now,
    CategoryId = 1,
};
await postRepo.InsertAsync(post);

var parentAuthor = new AnonymousUser { Id = "au1", Name = "Parent", Email = "parent@example.test" };
var replier = new AnonymousUser { Id = "au2", Name = "Replier", Email = "replier@example.test" };
await anonymousRepo.InsertAsync(parentAuthor);
await anonymousRepo.InsertAsync(replier);

var parentComment = new Comment {
    Id = "c1",
    PostId = post.Id,
    AnonymousUserId = parentAuthor.Id,
    Content = "parent content",
    Visible = true,
    IsNeedAudit = false,
};
await commentRepo.InsertAsync(parentComment);

var replyComment = new Comment {
    Id = "c2",
    ParentId = parentComment.Id,
    PostId = post.Id,
    AnonymousUserId = replier.Id,
    Content = "reply content",
    Visible = true,
    IsNeedAudit = false,
};
await commentRepo.InsertAsync(replyComment);

await commentService.EnqueueReplyNotificationIfNeededAsync(replyComment);

var outboxRows = await outboxRepo.Where(a => a.Type == OutboxTaskTypes.EmailSend).ToListAsync();
if (outboxRows.Count != 1) throw new Exception($"Expected 1 email.send outbox message, got {outboxRows.Count}");

Console.WriteLine("CommentReplyNotify smoke test succeeded.");

file sealed class NoopEmailHandler : IOutboxHandler {
    public string Type => OutboxTaskTypes.EmailSend;

    public Task HandleAsync(OutboxMessage message, CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}
