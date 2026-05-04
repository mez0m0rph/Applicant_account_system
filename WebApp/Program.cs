using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<IAuthApiService, AuthApiService>();
builder.Services.AddHttpClient<IApplicantApiService, ApplicantApiService>();
builder.Services.AddHttpClient<IProgramApiService, ProgramApiService>();
builder.Services.AddHttpClient<IAdmissionApiService, AdmissionApiService>();
builder.Services.AddHttpClient<IDocumentApiService, DocumentApiService>();
builder.Services.AddHttpClient<INotificationApiService, NotificationApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();