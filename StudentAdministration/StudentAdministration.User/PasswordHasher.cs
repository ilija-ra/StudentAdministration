using System.Security.Cryptography;

namespace StudentAdministration.User
{
    internal class PasswordHasher
    {
        public static (string hashedPassword, string salt) HashPassword(string password)
        {
            // Generate a random salt
            byte[] saltBytes = GenerateSalt();

            // Hash the password with the salt
            byte[] hashedPasswordBytes = HashPasswordWithSalt(password, saltBytes);

            // Convert byte arrays to base64-encoded strings
            string hashedPassword = Convert.ToBase64String(hashedPasswordBytes);
            string salt = Convert.ToBase64String(saltBytes);

            return (hashedPassword, salt);
        }

        public static bool VerifyPassword(string hashedPassword, string salt, string password)
        {
            // Convert base64-encoded strings to byte arrays
            byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Hash the provided password with the salt
            byte[] hashedAttemptBytes = HashPasswordWithSalt(password, saltBytes);

            // Compare the hashed passwords
            return SlowEquals(hashedPasswordBytes, hashedAttemptBytes);
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[32]; // You can adjust the salt size according to your needs
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000)) // Iteration count can be adjusted
            {
                return pbkdf2.GetBytes(32); // Output size can be adjusted
            }
        }

        // Constant-time comparison to prevent timing attacks
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }
    }
}
