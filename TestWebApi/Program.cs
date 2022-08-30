using TestWebApi.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
builder.Services.Configure<KestrelServerOptions>(options =>
    {
        options.Limits.MaxRequestBodySize = int.MaxValue/2;
    });

builder.Services.Configure<FormOptions>(options =>
   {
       options.ValueLengthLimit = int.MaxValue/4;
       options.MultipartBodyLengthLimit = int.MaxValue/2;
       
    }); 
builder.Services.AddControllers();
builder.Services.AddDbContext<DbFileContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/users");
                    
                });

var app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}");
});

app.Run();
