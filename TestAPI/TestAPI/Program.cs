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
    // �����ҥ��ѮɡA�^�����Y�|�]�t WWW-Authenticate ���Y�A�o�̷|��ܥ��Ѫ��Բӿ��~��]
    x.IncludeErrorDetails = true; // �w�]�Ȭ� true
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        // ���U�T�ӳ]�w�ݩʤ]�i�H�g�b appsettings.json�ɮסChttps://www.cnblogs.com/nsky/p/10312101.html
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // �άO�g�� IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("�z�ۤv��J��Secret Hash�ƭ�"))
        // (1) �Ψ����� (Hash) ���ê�����ƭ�
        ValidateIssuer = false,        // (2) �O�ֵ֮o���H  (false ������)
        ValidateAudience = false  // (3) ���ǫȤ�]Client�^�i�H�ϥΡH  (false ������)
    };
});


// (1)  Microsoft.AspNetCore.Authentication.Cookies
// (2) �ϥγo��� .AddAuthentication() �M .AddCookie() ��k�ӫإ����Ҥ����n��A��
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//                              .AddCookie(options => {
//                                  // �H�U�o��ӳ]�w�i���i�L
//                                  options.AccessDeniedPath = "/Home/AccessDeny";   // �ڵ��A�����\�n�J�A�|����o�@���C
//                                  options.LoginPath = "/Home/Login";     // �n�J���C
//                              });


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
// Authentication�]���ҡ^�G�T�{�ϥΪ̬O�_�u���O "�ҫź٪����ӤH"�C  �A���S���R���A�A�����O�u���ܡH�i�H�W���B�W�����H
// Authorization�]���v�^�G�ھڨϥΪ̪������ "�¤��������v��"�C  �A�����i�H�h�Y�����H�θg�ٿ��H
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
