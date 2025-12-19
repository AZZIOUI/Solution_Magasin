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

            // Configure DbContext
            builder.Services.AddDbContext<DotnetProjectContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure Identity with int as primary key
            builder.Services.AddIdentity<AspNetUser, AspNetRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;

                // SignIn settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<DotnetProjectContext>()
            .AddDefaultTokenProviders();

            // Configure Cookie settings
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Configure Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                // Politique pour l'espace Client
                options.AddPolicy(RoleConstants.ClientPolicy, policy =>
                    policy.RequireRole(RoleConstants.Client));

                // Politique pour l'espace Employé (tous les employés)
                options.AddPolicy(RoleConstants.EmployePolicy, policy =>
                    policy.RequireRole(RoleConstants.GetEmployeeRoles()));

                // Politique pour l'administrateur uniquement
                options.AddPolicy(RoleConstants.AdminPolicy, policy =>
                    policy.RequireRole(RoleConstants.Administrateur));

                // Politique pour Responsable Achat
                options.AddPolicy(RoleConstants.ResponsableAchatPolicy, policy =>
                    policy.RequireRole(RoleConstants.Administrateur, RoleConstants.ResponsableAchat));

                // Politique pour Magasinier
                options.AddPolicy(RoleConstants.MagasinierPolicy, policy =>
                    policy.RequireRole(RoleConstants.Administrateur, RoleConstants.Magasinier));
            });

            var app = builder.Build();

            // Seed roles and users
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<AspNetRole>>();
                    var userManager = services.GetRequiredService<UserManager<AspNetUser>>();
                    
                    await DbInitializer.SeedRolesAsync(roleManager);
                    await DbInitializer.SeedAdminAsync(userManager);
                    
                    // Créer des utilisateurs de test (uniquement en développement)
                    if (app.Environment.IsDevelopment())
                    {
                        await DbInitializer.SeedTestUsersAsync(userManager);
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation des rôles");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

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
