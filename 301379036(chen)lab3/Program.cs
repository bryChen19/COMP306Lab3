using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using _301379036_chen_lab3.Data;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using _301379036_chen_lab3.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 【关键修改 1】：加上 .AddRoles<IdentityRole>() 注册 RoleManager
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<S3Service>();
builder.Services.AddDefaultAWSOptions(
    builder.Configuration.GetAWSOptions()
);

builder.Services.AddScoped<IDynamoDBContext>(
    serviceProvider =>
    {
        IAmazonDynamoDB client = serviceProvider.GetRequiredService<IAmazonDynamoDB>();
        return new DynamoDBContext(client);
    }
);

builder.Services.AddScoped<ICommentService, DynamoDbCommentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 【关键修改 2】：使用 RoleManager 初始化角色
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Podcaster", "Listener" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 身份验证与授权中间件
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();