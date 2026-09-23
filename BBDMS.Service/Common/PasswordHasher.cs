using System;

namespace BBDMS.Service.Common
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }

        public static bool VerifyPassword(string inputPassword, string? storedPassword, out bool needsRehash)
        {
            needsRehash = false;

            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedPassword))
                return false;

            // Check if storedPassword is a valid BCrypt hash format
            if (IsBCryptHash(storedPassword))
            {
                try
                {
                    return BCrypt.Net.BCrypt.Verify(inputPassword, storedPassword);
                }
                catch
                {
                    return false;
                }
            }

            // Fallback for legacy plain text passwords: check equality and mark for upgrade
            if (inputPassword == storedPassword)
            {
                needsRehash = true;
                return true;
            }

            return false;
        }

        private static bool IsBCryptHash(string password)
        {
            return password.StartsWith("$2a$") || password.StartsWith("$2b$") || password.StartsWith("$2y$");
        }
    }
}
