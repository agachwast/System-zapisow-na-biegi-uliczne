using System;

namespace RunningEventRegistration
{
    public class RaceInfo
    {
        public DateTime RaceDate { get; private set; }
        public string RaceName { get; private set; }
        public string RaceLocation { get; private set; }
        public string MapImagePath { get; private set; }
        public string Distance { get; private set; }
        public string Description { get; private set; }

        public RaceInfo(string distance, string mapImagePath, string description)
        {
            Distance = distance;
            MapImagePath = mapImagePath;
            Description = description;
            RaceDate = new DateTime(2025, 9, 30, 8, 0, 0); // Data biegu - 30 września 2025, 8:00
            RaceName = "Bieg Uliczny 2025";
            RaceLocation = "Warszawa, Centrum";
        }

        public TimeSpan GetTimeUntilRace()
        {
            return RaceDate - DateTime.Now;
        }

        public string GetFormattedTimeUntilRace()
        {
            var timeUntil = GetTimeUntilRace();
            if (timeUntil.TotalSeconds <= 0)
            {
                return "Bieg już się rozpoczął!";
            }

            return $"{timeUntil.Days} dni, {timeUntil.Hours} godzin, {timeUntil.Minutes} minut, {timeUntil.Seconds} sekund";
        }

        public bool IsRaceActive()
        {
            return DateTime.Now >= RaceDate && DateTime.Now <= RaceDate.AddHours(6); // Zakładamy, że bieg trwa 6 godzin
        }

        public bool IsRegistrationOpen()
        {
            return DateTime.Now < RaceDate.AddDays(-1); // Rejestracja zamyka się dzień przed biegiem
        }
    }

    public class RaceInfoManager
    {
        private static readonly Dictionary<string, RaceInfo> _races;

        static RaceInfoManager()
        {
            _races = new Dictionary<string, RaceInfo>
            {
                { "5km", new RaceInfo("5km", "/maps/5km.png", "Bieg na 5km - idealny dla początkujących biegaczy. Trasa prowadzi przez park miejski.") },
                { "10km", new RaceInfo("10km", "/maps/10km.png", "Bieg na 10km - dla średniozaawansowanych. Trasa obejmuje centrum miasta.") },
                { "21km", new RaceInfo("21km", "/maps/21km.png", "Półmaraton - wymagająca trasa przez różne dzielnice miasta.") },
                { "42km", new RaceInfo("42km", "/maps/42km.png", "Maraton - pełna trasa maratońska przez całe miasto.") }
            };
        }

        public static RaceInfo GetRaceInfo(string distance)
        {
            if (_races.TryGetValue(distance, out RaceInfo raceInfo))
            {
                return raceInfo;
            }
            throw new ArgumentException($"Nie znaleziono informacji o biegu na dystansie {distance}");
        }

        public static Dictionary<string, RaceInfo> GetAllRaces()
        {
            return _races;
        }

        public static string GetGlobalCountdown()
        {
            // Używamy daty z pierwszego biegu (5km) jako daty głównej
            return _races["5km"].GetFormattedTimeUntilRace();
        }

        public static bool IsAnyRaceActive()
        {
            return _races.Values.Any(race => race.IsRaceActive());
        }

        public static bool IsRegistrationOpenForAnyRace()
        {
            return _races.Values.Any(race => race.IsRegistrationOpen());
        }
    }
} 