using GerenciadorAcademico.Repositories;
using GerenciadorAcademico.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(JsonFileStore<>));
builder.Services.AddScoped<IProjetoRepository, ProjetoJsonRepository>();
builder.Services.AddScoped<IProfessorRepository, ProfessorJsonRepository>();
builder.Services.AddScoped<IProjetoService, ProjetoService>();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Projetos/Erro"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Projetos}/{action=Index}/{id?}");
app.Run();
