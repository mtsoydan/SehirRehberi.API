using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SehirRehberi.API.Models;
using Microsoft.EntityFrameworkCore;

namespace SehirRehberi.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private DataContext _datacontext;
        public AuthRepository(DataContext dataContext)
        {
            _datacontext = dataContext;
        }
        public async Task<User> Login(string userName, string password)
        {
            var user = await _datacontext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return null;
            }
            if (!VeriyfPasswordHash(password,user.PasswordHash,user.PasswordSalt))
            {
                return null;
            }
            return user;
        }

        private bool VeriyfPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                //gelen kullanıcı değerini ve girilen password değerini hashle karşılaştırıp doğruysa girişi sağlıyoruz
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;

            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            //Out ile referansını yollayıp içeriğinin değişmesini sağlıyoruz
            CreatePasswordHash(password, out passwordHash,out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _datacontext.Users.AddAsync(user);
            await _datacontext.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                //.net ile gelen bu kütüphanede hem salt olustu hemde hash kodu olustu
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        public async Task<bool> UserExists(string userName)
        {
            if (await _datacontext.Users.AnyAsync(u => u.UserName == userName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
