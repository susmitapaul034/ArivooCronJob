using DBAccess.Implementation;
using DBAccess.Interface;
using ERPCronJob.Interfaces;
using ERPCronJob.Services;
using ERPCronJob.Services.Job;

 var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();

// Add scoped services
builder.Services.AddScoped<IPaymentGateWayRequeryService, PaymentGateWayRequeryService>();
builder.Services.AddScoped<IRequeryService, RequeryService>();
builder.Services.AddScoped<IERPRequeryService, ERPRequeryService>();

// Register services needed for your application
//builder.Services.AddCronJob<PaymentGateWayRequeryJob>(config =>
//    {
//        config.CronExpression = "0 * * * * ?"; // Example cron expression
//        config.TimeZoneInfo = TimeZoneInfo.Local;
//    });


builder.Services.AddCronJob<ERPRequeryJob>(config =>
{
    config.CronExpression = "0 * * * * ?"; // Example cron expression
    config.TimeZoneInfo = TimeZoneInfo.Local;
});

var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();

