using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCoreBasicsMitDbFirstUndAuth.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EfCoreBasicsMitDbFirstUndAuth.Services
{
    public class AuthService
    {
        private readonly EfCoreDbFirstAuthContext _ctx;

        public AuthService(EfCoreDbFirstAuthContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<AppUser> GetUserIfCanLogInAsync(string username, string password)
        {
            //Benutzer aufgrund des Usernamens abrufen
            var user = await _ctx.AppUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            //Wenn nicht gefunden darf er sich nicht einloggen
            if (user is null) return null;

            //Falls gefunden muss das Passwort überprüft werden
            var saltedAndHashedPassword = SaltAndHashPassword(password, user.Salt);

            //Achtung, byte[] können nicht einfach mit == verglichen werden
            if(saltedAndHashedPassword.SequenceEqual(user.PwHash))
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> RegisterNewUser(string username, string password)
        {
            //Gibt es Username/Email schon?
            var userExists = await _ctx.AppUsers.AnyAsync(u => u.Username == username);

            //Wenn ja, beenden wir die Methode
            if (userExists) return false;

            //Salt erzeugen
            var salt = GenerateSalt(32);

            var saltedAndHashedPassword = SaltAndHashPassword(password, salt);

            //In die DB speichern (inkl. Salt, gehashtem PW und Default-Rolle)
            var newUser = new AppUser
            {
                Username = username,
                PwHash = saltedAndHashedPassword,
                Salt = salt,
                RoleId = 1 //Default-Rolle für öffentliche Registrierung = 1
            };

            _ctx.AppUsers.Add(newUser);

            await _ctx.SaveChangesAsync();

            return true;
        }

        private byte[] GenerateSalt(int numBytes)
        {
            //Sicherer Zufallsgenerator aus System.Security.Cryptogrpahy
            var rng = RandomNumberGenerator.Create();

            var salt = new byte[numBytes];

            rng.GetBytes(salt);

            return salt;
        }

        private byte[] SaltAndHashPassword(string password, byte[] salt)
        {
            //Passwort in byte[] umwandeln
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            //Salt an Passwort hängen
            var saltedPassword = passwordBytes.Concat(salt).ToArray();

            //Hashen
            var hasher = SHA256.Create();

            //Gesaltetes Passwort Hashen
            var saltedAndHashedPassword = hasher.ComputeHash(saltedPassword);

            return saltedAndHashedPassword;
        }
    }
}
