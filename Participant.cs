using System;

namespace RunningEventRegistration
{
    public class Participant
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AgeGroup { get; set; }
        public string Gender { get; set; }
        public string Distance { get; set; }
        public bool WantsTShirt { get; set; }
        public string TShirtColor { get; set; }
        public Guid RegistrationId { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Participant(string firstName, string lastName, string ageGroup, string gender, 
            string distance, bool wantsTShirt)
        {
            FirstName = firstName;
            LastName = lastName;
            AgeGroup = ageGroup;
            Gender = gender;
            Distance = distance;
            WantsTShirt = wantsTShirt;
            TShirtColor = GetTShirtColorForDistance(distance);
            RegistrationId = Guid.NewGuid();
            RegistrationDate = DateTime.Now;
        }

        private string GetTShirtColorForDistance(string distance)
        {
            return distance switch
            {
                "5km" => "Niebieski",
                "10km" => "Zielony",
                "21km" => "Pomarańczowy",
                "42km" => "Czerwony",
                _ => "Nieznany"
            };
        }

        public override string ToString()
        {
            return $"{FirstName},{LastName},{AgeGroup},{Gender},{Distance},{WantsTShirt},{TShirtColor},{RegistrationId},{RegistrationDate}";
        }
    }
} 