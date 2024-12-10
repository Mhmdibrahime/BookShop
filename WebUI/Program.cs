
using Services.AuthorServices;
using Services.BookServices;
using Services.CategoryServices;
using Services.PublisherServices;
using Microsoft.AspNetCore.Identity;
using DataAccess.Data;
using DataAccess.Implementation;
using DataAccess.Dbintializer;
using Services.EmailServices;
using Microsoft.AspNetCore.Identity.UI.Services;
using Services.ShoppingCartServices;
using Services.BorrowinTransactionService;
using Services.PurchaseTransactionService;
using Microsoft.EntityFrameworkCore;
using Entities.Domains;
using Entities.Repositories;
using Entities.APIServces;
using Stripe;
using Services.RateService;

namespace WebUI
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")
                ));




            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(4);
                options.SignIn.RequireConfirmedAccount = true;
            })
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders()
                            .AddDefaultUI();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // √Ê „« Ì ‰«”» „⁄ «Õ Ì«Ã« ﬂ
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; // ·Ã⁄· «·‹ Session „ «Õ« Õ Ï ›Ì ”Ì«”… «·Œ’Ê’Ì…
            });
            builder.Services.AddSingleton<IEmailSender,EmailService>();
            builder.Services.Configure<StripeDetails>(builder.Configuration.GetSection("stripe"));
            builder.Services.AddScoped<IDbintializer, Dbintializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitofWork>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<IPublisherService, PublisherService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddTransient<IEmailSender, EmailService>();
            builder.Services.AddTransient<ICartService, CartService>();
            builder.Services.AddTransient<IRateService, RateService>();
            builder.Services.AddTransient<IBorrowinTransactionService, BorrowinTransactionService>();
            builder.Services.AddTransient<IPurchaseTransactionService, PurchaseTransactionService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:Secretkey").Get<string>();
            SpeedUp();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
              

            app.MapRazorPages();

            app.Run();
            void SpeedUp()
            {
                using var scope = app.Services.CreateScope();
                {
                    var dBintailizer = scope.ServiceProvider.GetRequiredService<IDbintializer>();
                    dBintailizer.Initialize();
                }
            }
        }

    }
}

