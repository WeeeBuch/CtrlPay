using CtrlPay.DB;
using CtrlPay.Entities;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Core
{
    public static class AuthLogic
    {
        const int SaltSize = 16;            // 128 bit
        const int KeySize = 32;             // 256 bit
        const int Iterations = 100_000;

        public static AuthLogicReturnModel Login(string username, string password)
        {
            CtrlPayDbContext db = new CtrlPayDbContext();
            User? user = db.Users.FirstOrDefault(u => u.Username == username);
            if(user == null)
            {
                return new AuthLogicReturnModel(3, ReturnCodeConverter.GetMessage(3));
            }

            byte[] salt = user.PasswordSalt;
            byte[] storedHash = user.PasswordHash;

            var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256
            );

            byte[] computedHash = pbkdf2.GetBytes(KeySize);

            bool valid = CryptographicOperations.FixedTimeEquals(
                            computedHash,
                            storedHash);

            if( !valid )
            {
                return new AuthLogicReturnModel(2, ReturnCodeConverter.GetMessage(2));
            }

            if (user.TwoFactorEnabled)
            {
                return new AuthLogicReturnModel(5, ReturnCodeConverter.GetMessage(5));
            } 
            return new AuthLogicReturnModel(0, ReturnCodeConverter.GetMessage(0));            
        }
        public static AuthLogicReturnModel AddUser(string username, string password, Role role)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256
            );

            byte[] hash = pbkdf2.GetBytes(KeySize);

            CtrlPayDbContext db = new CtrlPayDbContext();
            User newUser = new User(username, hash, salt, role);
            db.Users.Add(newUser);
            db.SaveChanges();
            return new AuthLogicReturnModel(0, ReturnCodeConverter.GetMessage(0));
        }
        public static AuthLogicReturnModel TotpLogin(byte[] secret, string code)
        {
            bool valid = VerifyTotp(secret, code);
            if( valid )
            {
                return new AuthLogicReturnModel(0, ReturnCodeConverter.GetMessage(0));
            }
            else
            {
                return new AuthLogicReturnModel(4, ReturnCodeConverter.GetMessage(4));
            }
        }
        private static bool VerifyTotp(byte[] secret, string code)
        {
            var timestep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;

            for (long i = -1; i <= 1; i++) // tolerance ±30 s
            {
                if (GenerateTotp(secret, timestep + i) == code)
                    return true;
            }
            return false;
        }

        private static string GenerateTotp(byte[] secret, long timestep)
        {
            var timestepBytes = BitConverter.GetBytes(timestep);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timestepBytes);

            using var hmac = new HMACSHA1(secret);
            var hash = hmac.ComputeHash(timestepBytes);

            int offset = hash[^1] & 0x0F;

            int binary =
                ((hash[offset] & 0x7f) << 24) |
                ((hash[offset + 1] & 0xff) << 16) |
                ((hash[offset + 2] & 0xff) << 8) |
                (hash[offset + 3] & 0xff);

            int otp = binary % 1_000_000;

            return otp.ToString("D6");
        }

    }
}
