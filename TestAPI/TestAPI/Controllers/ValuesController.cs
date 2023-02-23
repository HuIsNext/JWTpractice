using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAPI.jwtServices;
using TestAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace TestAPI.Controllers
{
    // Nuget套件 -- 
    //(1) Microsoft.AspNetCore.Authentication
    //(2) Microsoft.AspNetCore.Authentication.JwtBearer 
    //(3) Microsoft.EntityFrameworkCore             
    //(4) Microsoft.EntityFrameworkCore.Tools   
    //(5) Microsoft.EntityFrameworkCore.SqlServer  
    //(6) System.Security.Claims

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly MvcUserDbContext _db;
        public ValuesController(MvcUserDbContext context)
        {
            _db = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public string Get()
        {
            return "你好陌生人";
        }


        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] UserTable userA)
        {
            var ListOne = from user in _db.UserTables
                          where user.UserMobilePhone == userA.UserMobilePhone
                          select user;

            UserTable ut = ListOne.FirstOrDefault();
            string token = TokenService.CreateToken(userA);
            return Ok(new
            {
                message="成功",
                ut,
                token
            });

        }

        //[HttpGet("{id}")]
        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public string Get2()
        {
            return string.Format("你好 驗證成功");
        }
    }
}
