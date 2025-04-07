using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.ErrosService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ServidorService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<MatriculaService>();
builder.Services.AddScoped<SecretariaService>();
builder.Services.AddScoped<PerfilCalculo>();
builder.Services.AddScoped<CleanupService>();

builder.Services.AddDbContext<BancoContext>(x
        => x.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(3, 0, 38))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BancoContext>();
    context.Database.Migrate(); // opcional, se quiser aplicar migrações
    BancoContext.Seed(context);     // <-- aqui você chama o Seed
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
