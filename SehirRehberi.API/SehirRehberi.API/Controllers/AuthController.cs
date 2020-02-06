using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SehirRehberi.API.Data;
using SehirRehberi.API.Dtos;
using SehirRehberi.API.Models;

namespace SehirRehberi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _authRepository;
        private IConfiguration _configuration;
        private object encoding;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto)
        {
            if (await _authRepository.UserExists(userForRegisterDto.UserName))
            {
                ModelState.AddModelError("UserName", "User Name already exist");

            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userToCreate = new User
            {
                UserName = userForRegisterDto.UserName
            };

            var createdUset = await _authRepository.Register(userToCreate, userForRegisterDto.Password);
            return StatusCode(201);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
        {
            //Kullanıcıyı kontrol eet

            var user = await _authRepository.Login(userForLoginDto.UserName, userForLoginDto.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            //Eğer kullanıcı varsa sisteme giriş yaptıysa token yollayacağız
            // token sürecince sistemde kalabilir

            //Token temsilcisini tanımlıyoruz
            var tokenHandler = new JwtSecurityTokenHandler();
            //Appsettingdeki token değerimizi aldık
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("appsettings:Token").Value);

            //Token açıklamalarını barındırak değişkenimizi tanımlayalım
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    //İçerisine gerekli kullanıcı bilgilerini gömdük
                    new Claim (ClaimTypes.NameIdentifier,user.UserID.ToString()),
                    new Claim (ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.Now.AddDays(1),//token geçerlilik süresi

                //Ve kullanılan algoritma ve key içeriği
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            //Token olusturuldu
            var token = tokenHandler.CreateToken(tokenDescription);
            //Token stringi elde edildi
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(tokenString);




        }
    }
}