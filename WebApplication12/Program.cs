using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApplication12
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<EFContext>();
            builder.Services.AddSession();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            { 
                option.AccessDeniedPath = "/DbView/LoginView";
                option.LoginPath = "/DbView/LoginView";
            });
            builder.Services.AddAuthorization();
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EFContext>();
                context.Items.Add(new Models.Item { name = "first", description = "first description" });
                context.Items.Add(new Models.Item { name = "second", description = "second description" });
                context.Items.Add(new Models.Item { name = "third", description = "third description" });
                context.Items.Add(new Models.Item { name = "fourth", description = "fourth description" });
                context.Items.Add(new Models.Item { name = "fifth", description = "fifth description" });
                context.SaveChanges();
            }



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }

}



