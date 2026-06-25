using festas_da_aldeia.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/CartazPages", "RequireAdmin");
    
    options.Conventions.AuthorizePage("/ArtistaPages/Admin", "RequireAdmin");
    options.Conventions.AuthorizePage("/ArtistaPages/Create", "RequireAdmin");
    options.Conventions.AuthorizePage("/ArtistaPages/Edit", "RequireAdmin");
    options.Conventions.AuthorizePage("/ArtistaPages/Delete", "RequireAdmin");
    
    options.Conventions.AuthorizePage("/EventoPages/Index", "RequireAdmin");
    options.Conventions.AuthorizePage("/EventoPages/Create", "RequireAdmin");
    options.Conventions.AuthorizePage("/EventoPages/Edit", "RequireAdmin");
    options.Conventions.AuthorizePage("/EventoPages/Delete", "RequireAdmin");
    options.Conventions.AuthorizePage("/EventoPages/Details", "RequireAdmin");
    
    options.Conventions.AuthorizePage("/LocalPages/Index", "RequireAdmin");
    options.Conventions.AuthorizePage("/LocalPages/Create", "RequireAdmin");
    options.Conventions.AuthorizePage("/LocalPages/Edit", "RequireAdmin");
    options.Conventions.AuthorizePage("/LocalPages/Delete", "RequireAdmin");
    options.Conventions.AuthorizePage("/LocalPages/Details", "RequireAdmin");
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapHub<festas_da_aldeia_pages.Hubs.VisualizacoesHub>("/visualizacoesHub");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbSeeder.SeedAllAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao popular a base de dados.");
    }
}

app.Run();
