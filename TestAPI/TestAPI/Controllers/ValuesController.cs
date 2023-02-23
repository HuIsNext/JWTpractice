using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAPI.jwtServices;
using TestAPI.Models;

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
            string token = TokenService.CreateToken(userA);   
            return Ok(new
            {            
                userA,
                token
            });

        }

        //[HttpPost]
        //[AllowAnonymous]
        //public IActionResult Post([FromBody] UserTable user)
        //{
        //    string token = TokenService.CreateToken(user);
        //    return Ok(new
        //        {
        //        token
        //         });

        //}

        [HttpGet("{id}")]
        public string Get2(int id)
        {
            return string.Format("你好{0}", id.ToString());
        }
    }
}
