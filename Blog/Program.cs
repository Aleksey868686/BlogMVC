using Blog.Data;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



namespace Blog;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Configure DbContext with the connection string
        builder.Services.AddDbContext<BlogDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("BlogDb")));

        builder.Services.AddScoped<PostService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<CommentService>();
        builder.Services.AddScoped<TagService>();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               options.LoginPath = "/Authentication/Login";
               options.LogoutPath = "/Authentication/Logout";
           });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Administrator", policy =>
                policy.RequireClaim(ClaimTypes.Role, "Administrator"));

            options.AddPolicy("Moderator", policy =>
                policy.RequireAssertion(x => x.User.HasClaim(ClaimTypes.Role, "Moderator") || x.User.HasClaim(ClaimTypes.Role, "Administrator")));

            options.AddPolicy("User", policy =>
                policy.RequireAssertion(x => x.User.HasClaim(ClaimTypes.Role, "User") || x.User.HasClaim(ClaimTypes.Role, "Administrator")));

        });

        var app = builder.Build();
        InitializeDatabase(app);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");        

        app.Run();
    }

    private static void InitializeDatabase(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var db = serviceScope.ServiceProvider.GetRequiredService<BlogDbContext>();
            User user1 = null, user2 = null, user3 = null;
            Post post1 = null, post2 = null;
            Tag tag1 = null, tag2 = null, tag3 = null;

            if (!db.Roles.Any())
            {
                db.Roles.AddRange(
                    new Role { Name = "Administrator" },
                    new Role { Name = "Moderator" },
                    new Role { Name = "User" }
                );
                db.SaveChanges();
            }
            // Check if there's already data in the database to prevent duplicate entries.
            if (!db.Users.Any())
            {
                user1 = new User { FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", JoinDate = DateTime.UtcNow };
                user2 = new User { FirstName = "Bob", LastName = "Smith", Email = "bob@example.com", JoinDate = DateTime.UtcNow };
                user3 = new User { FirstName = "Bruce", LastName = "Wayne", Email = "bruce@example.com", JoinDate = DateTime.UtcNow };

                db.Users.AddRange(user1, user2, user3);
                db.SaveChanges();
            }

            if (!db.Posts.Any())
            {
                // Ensure the user variables have been initialized.
                if (user1 == null || user2 == null)
                {
                    user1 = db.Users.FirstOrDefault(u => u.FirstName == "Alice");
                    user2 = db.Users.FirstOrDefault(u => u.FirstName == "Bob");
                }

                post1 = new Post { Title = "Welcome to my blog", Content = "This is the first post on my blog.", DateCreated = DateTime.UtcNow, UserId = user1.Id };
                post2 = new Post { Title = "A day in the life", Content = "Blog post about daily routine.", DateCreated = DateTime.UtcNow, UserId = user2.Id };

                db.Posts.AddRange(post1, post2);
                db.SaveChanges(); 
            }

            if (!db.Tags.Any())
            {
                tag1 = new Tag { Name = "Personal" };
                tag2 = new Tag { Name = "Public" };
                tag3 = new Tag { Name = "Diary" };

                db.Tags.AddRange(tag1, tag2, tag3);
                db.SaveChanges();
            }

            if (!db.Comments.Any())
            {
                // Ensure the post and user variables have been initialized.
                if (post1 == null || user1 == null)
                {
                    post1 = db.Posts.FirstOrDefault(p => p.Title == "Welcome to my blog");
                    user1 = db.Users.FirstOrDefault(); // Fetch the first user as an example
                }

                var comment1 = new Comment { Content = "Great post!", PostId = post1.Id, UserId = user1.Id };
                var comment2 = new Comment { Content = "Thanks for sharing", PostId = post1.Id, UserId = user1.Id };

                db.Comments.AddRange(comment1, comment2);
                db.SaveChanges();
            }

            if (!db.PostTags.Any())
            {
                // Ensure the post and tag variables have been initialized.
                if (post1 == null || tag1 == null)
                {
                    post1 = db.Posts.FirstOrDefault(p => p.Title == "Welcome to my blog");
                    tag1 = db.Tags.FirstOrDefault(t => t.Name == "Personal");
                }

                if (post2 == null || tag2 == null)
                {
                    post2 = db.Posts.FirstOrDefault(p => p.Title == "A day in the life");
                    tag2 = db.Tags.FirstOrDefault(t => t.Name == "Public");
                    
                }

                if (post1 != null && post2 != null && tag1 != null && tag2 != null)
                {
                    var postTag1 = new PostTag { PostId = post1.Id, TagId = tag1.Id };
                    var postTag2 = new PostTag { PostId = post2.Id, TagId = tag2.Id };

                    db.PostTags.AddRange(postTag1, postTag2);
                }
            }

            if (!db.UserRoles.Any())
            {
                // Fetch the roles
                var adminRole = db.Roles.FirstOrDefault(r => r.Name == "Administrator");
                var modRole = db.Roles.FirstOrDefault(r => r.Name == "Moderator");
                var userRole = db.Roles.FirstOrDefault(r => r.Name == "User");

                // Fetch the users
                user1 = db.Users.FirstOrDefault(u => u.FirstName == "Alice");
                user2 = db.Users.FirstOrDefault(u => u.FirstName == "Bob");
                user3 = db.Users.FirstOrDefault(u => u.FirstName == "Bruce");

                // Assign roles to users
                if (adminRole != null && user1 != null)
                {
                    db.UserRoles.Add(new UserRole { UserId = user1.Id, RoleId = adminRole.Id });
                }
                if (modRole != null && user2 != null)
                {
                    db.UserRoles.Add(new UserRole { UserId = user2.Id, RoleId = modRole.Id });
                }
                if (userRole != null && user3 != null)
                {
                    db.UserRoles.Add(new UserRole { UserId = user3.Id, RoleId = userRole.Id });
                }

                db.SaveChanges();
            }

            db.SaveChanges();        
        }
    }
}