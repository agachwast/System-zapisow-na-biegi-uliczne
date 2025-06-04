using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RunningEventRegistration
{
    public class RegistrationSystem
    {
        private readonly string _baseDirectory;
        private readonly string _usersFilePath;
        private readonly List<string> _validDistances = new List<string> { "5km", "10km", "21km", "42km" };
        private readonly List<string> _validAgeGroups = new List<string> 
        { 
            "18-24", "25-34", "35-44", "45-54", "55-64", "65+" 
        };
        private readonly List<string> _validGenders = new List<string> { "M", "F" };
        private Dictionary<string, User> _users;

        public RegistrationSystem(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
            _usersFilePath = Path.Combine(_baseDirectory, "users.csv");
            _users = new Dictionary<string, User>();
            InitializeDirectories();
            LoadUsers();
        }

        private void InitializeDirectories()
        {
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }

            foreach (var distance in _validDistances)
            {
                string distancePath = Path.Combine(_baseDirectory, distance);
                if (!Directory.Exists(distancePath))
                {
                    Directory.CreateDirectory(distancePath);
                }
            }
        }

        private void LoadUsers()
        {
            if (File.Exists(_usersFilePath))
            {
                var lines = File.ReadAllLines(_usersFilePath).Skip(1); // Skip header
                foreach (var line in lines)
                {
                    try
                    {
                        var user = User.FromString(line);
                        _users[user.Email] = user;
                    }
                    catch (Exception)
                    {
                        // Skip invalid user entries
                        continue;
                    }
                }
            }
            else
            {
                // Create users file with header
                File.WriteAllText(_usersFilePath, "Email,PasswordHash,RegistrationDate,IsAdmin\n");
            }
        }

        public bool RegisterUser(string email, string password)
        {
            try
            {
                if (_users.ContainsKey(email.ToLower()))
                {
                    return false; // User already exists
                }

                var user = new User(email, password);
                _users[user.Email] = user;

                // Append user to file
                File.AppendAllText(_usersFilePath, user.ToString() + "\n");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AuthenticateUser(string email, string password)
        {
            if (!_users.TryGetValue(email.ToLower(), out User user))
            {
                return false;
            }

            return user.VerifyPassword(password);
        }

        public bool RegisterParticipant(string userEmail, Participant participant)
        {
            if (!_users.ContainsKey(userEmail.ToLower()))
            {
                return false; // User must be registered first
            }

            try
            {
                if (!ValidateParticipant(participant))
                {
                    return false;
                }

                string distancePath = Path.Combine(_baseDirectory, participant.Distance);
                string filePath = Path.Combine(distancePath, "registrations.csv");

                // Create file with headers if it doesn't exist
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "Email,FirstName,LastName,AgeGroup,Gender,Distance,WantsTShirt,TShirtColor,RegistrationId,RegistrationDate\n");
                }

                // Append participant data with user email
                string participantData = $"{userEmail},{participant.ToString()}\n";
                File.AppendAllText(filePath, participantData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ValidateParticipant(Participant participant)
        {
            if (string.IsNullOrWhiteSpace(participant.FirstName) || 
                string.IsNullOrWhiteSpace(participant.LastName) ||
                string.IsNullOrWhiteSpace(participant.AgeGroup) ||
                string.IsNullOrWhiteSpace(participant.Gender) ||
                string.IsNullOrWhiteSpace(participant.Distance))
            {
                return false;
            }

            return _validDistances.Contains(participant.Distance) &&
                   _validAgeGroups.Contains(participant.AgeGroup) &&
                   _validGenders.Contains(participant.Gender);
        }

        public List<Participant> GetParticipantsByDistance(string distance)
        {
            if (!_validDistances.Contains(distance))
            {
                return new List<Participant>();
            }

            string filePath = Path.Combine(_baseDirectory, distance, "registrations.csv");
            if (!File.Exists(filePath))
            {
                return new List<Participant>();
            }

            var participants = new List<Participant>();
            var lines = File.ReadAllLines(filePath).Skip(1); // Skip header

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length >= 8)  // Updated to include email
                {
                    var participant = new Participant(
                        parts[1], // FirstName
                        parts[2], // LastName
                        parts[3], // AgeGroup
                        parts[4], // Gender
                        parts[5], // Distance
                        bool.Parse(parts[6]) // WantsTShirt
                    );
                    participants.Add(participant);
                }
            }

            return participants;
        }

        public Dictionary<string, int> GetRegistrationStatistics()
        {
            var stats = new Dictionary<string, int>();
            foreach (var distance in _validDistances)
            {
                var participants = GetParticipantsByDistance(distance);
                stats[distance] = participants.Count;
            }
            return stats;
        }

        public List<Participant> GetUserRegistrations(string userEmail)
        {
            var userRegistrations = new List<Participant>();
            foreach (var distance in _validDistances)
            {
                string filePath = Path.Combine(_baseDirectory, distance, "registrations.csv");
                if (!File.Exists(filePath))
                {
                    continue;
                }

                var lines = File.ReadAllLines(filePath).Skip(1);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 8 && parts[0].ToLower() == userEmail.ToLower())
                    {
                        var participant = new Participant(
                            parts[1], // FirstName
                            parts[2], // LastName
                            parts[3], // AgeGroup
                            parts[4], // Gender
                            parts[5], // Distance
                            bool.Parse(parts[6]) // WantsTShirt
                        );
                        userRegistrations.Add(participant);
                    }
                }
            }
            return userRegistrations;
        }
    }
} 