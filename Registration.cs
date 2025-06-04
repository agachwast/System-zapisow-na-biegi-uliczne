using System;

namespace RaceRegistration.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AgeGroup { get; set; }
        public string Gender { get; set; }
        public string Distance { get; set; }
        public bool TshirtOrdered { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
} 