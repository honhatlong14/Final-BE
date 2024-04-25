using Common.Constants;
using Data.DbContext;
using Data.Entities.User;
using Data.IRepository;
using Microsoft.EntityFrameworkCore;
using Service.IServices;

namespace Webapi.Initializer;

public static class DbInitializer
{
    public static void Initialize(IApplicationBuilder app)
    {
        // service scope  
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            var userService =
                serviceScope.ServiceProvider.GetService<IUserService>();
            var userRepo =
                serviceScope.ServiceProvider.GetService<IRepository<User>>();
            try
            {
                if (context.Database.GetPendingMigrations().Count() > 0)
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
            }
            
            // user admin
            var userDb = userRepo.GetFirstOrDefaultAsync(_ => _.Email == "Admin@gmail.com" && !_.IsDeleted).GetAwaiter().GetResult();
            if (userDb != null)
                return;
            
            var user = new User()
            {
                FullName = "Long Admin", 
                Email = "Admin@gmail.com",
                Role = StringEnum.Roles.Admin,
                AcceptTerms = true,
                Verified = DateTime.UtcNow,
                Avatar = ""
            };

            userService.Register(user, "Admin123@", String.Empty, Guid.NewGuid().ToString()).GetAwaiter().GetResult();
            userRepo.UpdateOneAsync(_ => _.Email == "Admin@gmail.com", _ => _.VerificationToken, null).GetAwaiter().GetResult();
        }
    }
}