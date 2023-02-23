using Microsoft.EntityFrameworkCore;
using TestAPI.Models;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using TestAPI.jwtServices;  
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TestAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MvcUserDbContext>(
        options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

        });

builder.Services.AddCors(options => options.AddPolicy(name: "mis2000labOrigins",
    policy =>
    {
        policy.WithOrigins("http://localhost:44450").AllowAnyMethod().AllowAnyHeader();
    }));
var key = Encoding.UTF8.GetBytes(Settings.Secret);

builder.Services.AddAuthentication(z =>
{
    z.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    z.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
    x.IncludeErrorDetails = true; // 預設值為 true
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        // 底下三個設定屬性也可以寫在 appsettings.json檔案。https://www.cnblogs.com/nsky/p/10312101.html
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // 或是寫成 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("您自己輸入的Secret Hash數值"))
        // (1) 用來雜湊 (Hash) 打亂的關鍵數值
        ValidateIssuer = false,        // (2) 是誰核發的？  (false 不驗證)
        ValidateAudience = false  // (3) 哪些客戶（Client）可以使用？  (false 不驗證)
    };
});


// (1)  Microsoft.AspNetCore.Authentication.Cookies
// (2) 使用這兩者 .AddAuthentication() 和 .AddCookie() 方法來建立驗證中介軟體服務
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//                              .AddCookie(options => {
//                                  // 以下這兩個設定可有可無
//                                  options.AccessDeniedPath = "/Home/AccessDeny";   // 拒絕，不允許登入，會跳到這一頁。
//                                  options.LoginPath = "/Home/Login";     // 登入頁。
//                              });


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
// Authentication（驗證）：確認使用者是否真的是 "所宣稱的那個人"。  你有沒有買票，你的票是真的嗎？可以上車、上飛機？
// Authorization（授權）：根據使用者的角色來 "授予應有的權限"。  你的票可以去頭等艙？或經濟艙？
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
