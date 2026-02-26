using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using EquipmentBorrowingSystem.Data;
using EquipmentBorrowingSystem.EMail;
using EquipmentBorrowingSystem.OverdueNotificationService;
using System.Text.Encodings.Web;
using System.Text.Unicode;


var builder = WebApplication.CreateBuilder(args);
//取得組態中資料庫連線設定
string connectionString = builder.Configuration.GetConnectionString("EmployeeContext");

//註冊EF Core的EmployeeContext
builder.Services.AddDbContext<EmployeeContext>(options => options.UseSqlServer(connectionString));// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

    //options.JsonSerializerOptions.Encoder =
    //    JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);

}); ;

//註冊IEmailSender和EmailSender的方法
builder.Services.AddTransient<IEmailSender, EmailSender>();


builder.Services.AddScoped<OverdueNotificationTask>();



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.Zero; // 或者設為 null
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.MaxValue; // 設置 Session 永不過期
});

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//.AddCookie(options =>
//{
//    options.LoginPath = "/Account/Login";
//    options.AccessDeniedPath = "/Account/AccessDenied";
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// 啟動後台工作
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
_ = Task.Run(async () =>
{
    while (true)
    {
        // 創建作用域
        using (var scope = scopeFactory.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var overdueNotificationTask = serviceProvider.GetRequiredService<OverdueNotificationTask>();
            overdueNotificationTask.Run();
            overdueNotificationTask.Run1();
        }
        await Task.Delay(TimeSpan.FromSeconds(1)); // 每秒執行一次
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
//app.UseAuthentication();
//app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=BorrowEquipment1}/{action=Browse_equipment}/{id?}");

app.Run();
public static class SharedData
{
    public static string WelcomeMessage { get; set; }
}
