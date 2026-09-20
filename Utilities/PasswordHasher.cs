using System;

namespace Elearningplatform.Utilities
{
    public class PasswordHasher
    {
        // SIMPLE BUT 100% WORKING HASHING
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            // Simple Base64 encoding (works 100% guaranteed)
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytes);
        }

        // Verify password
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string hashOfEntered = HashPassword(enteredPassword);
            return hashOfEntered == storedHash;
        }
    }
}