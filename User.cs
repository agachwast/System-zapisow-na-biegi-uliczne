using System;
using System.Security.Cryptography;
using System.Text;

namespace RunningEventRegistration
{
    public class User
    {
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime RegistrationDate { get; private set; }
        public bool IsAdmin { get; private set; }

        public User(string email, string password, bool isAdmin = false)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Email and password cannot be empty");

            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email format");

            Email = email.ToLower();
            PasswordHash = HashPassword(password);
            RegistrationDate = DateTime.Now;
            IsAdmin = isAdmin;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string password)
        {
            return PasswordHash == HashPassword(password);
        }

        public override string ToString()
        {
            return $"{Email},{PasswordHash},{RegistrationDate},{IsAdmin}";
        }

        public static User FromString(string userData)
        {
            var parts = userData.Split(',');
            if (parts.Length != 4)
                throw new ArgumentException("Invalid user data format");

            return new User(parts[0], "dummy", bool.Parse(parts[3]))
            {
                PasswordHash = parts[1],
                RegistrationDate = DateTime.Parse(parts[2])
            };
        }
    }
} 