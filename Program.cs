using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Solution_Magasin.Constants;
using Solution_Magasin.Models;
using Solution_Magasin.Services;

namespace Solution_Magasin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Configuration du contexte de base de données principal
            builder.Services.AddDbContext<DotnetProjectContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configuration du contexte Identity
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configuration d'Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Configuration du mot de passe
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // Configuration du compte
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;

                // Configuration du verrouillage
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configuration des cookies
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            // Configuration des politiques d'autorisation
            builder.Services.AddAuthorization(options =>
            {
                // Politique pour les clients uniquement
                options.AddPolicy(RoleConstants.ClientPolicy, policy =>
                    policy.RequireRole(RoleConstants.Client));

                // Politique pour tous les employés
                options.AddPolicy(RoleConstants.EmployePolicy, policy =>
                    policy.RequireRole(RoleConstants.GetEmployeeRoles()));

                // Politique pour les administrateurs uniquement
                options.AddPolicy(RoleConstants.AdminPolicy, policy =>
                    policy.RequireRole(RoleConstants.Administrateur));

                // Politique pour les responsables d'achat uniquement
                options.AddPolicy(RoleConstants.ResponsableAchatPolicy, policy =>
                    policy.RequireRole(RoleConstants.ResponsableAchat));

                // Politique pour les magasiniers uniquement
                options.AddPolicy(RoleConstants.MagasinierPolicy, policy =>
                    policy.RequireRole(RoleConstants.Magasinier));
            });

            // Enregistrer les services
            builder.Services.AddScoped<DatabaseSeeder>();
            builder.Services.AddScoped<IImageUploadService, ImageUploadService>();

            var app = builder.Build();

            // Seed la base de données avec les rôles et l'admin par défaut
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
                await seeder.SeedAsync();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            // Enable static files middleware for serving images from wwwroot
            app.UseStaticFiles();
            
            app.UseRouting();

            // Add session middleware before authentication
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
