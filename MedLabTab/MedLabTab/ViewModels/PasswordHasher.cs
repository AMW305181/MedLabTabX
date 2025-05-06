using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MedLabTab.ViewModels
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; 
        private const int HashSize = 32; 
        private const int Iterations = 100000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public static string Hash(string password)
        {
            
            byte[] salt = new byte [SaltSize];
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(salt);


            var pbkdf2 = new Rfc2898DeriveBytes(
             password: password,
             salt: salt,
             iterations: Iterations,
              hashAlgorithm: Algorithm);

            var hash = pbkdf2.GetBytes(HashSize);
            
            var hashBytes = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, HashSize);

            var base64Hash = Convert.ToBase64String(hashBytes);

            return $"$PBKDF2${Iterations}${Algorithm.Name}${base64Hash}";
        }

        public static bool Verify(string password, string hashedPassword)
        {
            // Extract parts from the stored hash
            var parts = hashedPassword.Split('$');
            if (parts.Length != 5 || parts[1] != "PBKDF2")
            {
                throw new FormatException("The hashed password is not in the expected format.");
            }

            var iterations = int.Parse(parts[2]);
            var algorithm = new HashAlgorithmName(parts[3]);
            var base64Hash = parts[4];

            var hashBytes = Convert.FromBase64String(base64Hash);

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(
                password: password,
                salt: salt,
                iterations: iterations,
                hashAlgorithm: algorithm);

            byte[] hash = pbkdf2.GetBytes(HashSize);

            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
