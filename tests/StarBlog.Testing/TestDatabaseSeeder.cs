using FreeSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StarBlog.Data;
using StarBlog.Data.Models;

namespace StarBlog.Testing;

public static class TestDatabaseSeeder {
    public static async Task EnsureEfCoreDatabaseAsync(IServiceProvider serviceProvider) {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public static async Task SeedMinimalBlogDataAsync(IServiceProvider serviceProvider) {
        using var scope = serviceProvider.CreateScope();

        var categoryRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<Category>>();
        var postRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<Post>>();

        var category = await categoryRepo.InsertAsync(new Category {
            Name = "Test Category",
            ParentId = 0,
            Visible = true,
            Posts = new List<Post>()
        });

        await postRepo.InsertAsync(new Post {
            Id = Guid.NewGuid().ToString("N"),
            Title = "Test Post",
            Slug = "test-post",
            IsPublish = true,
            Summary = "summary",
            Content = "# Hello\n\nBody",
            Path = "test",
            CreationTime = DateTime.UtcNow,
            LastUpdateTime = DateTime.UtcNow,
            CategoryId = category.Id,
            Categories = category.Id.ToString()
        });
    }
}

