using System;

namespace SeleniumEduApp.Models
{
    public class UserProfile
    {
        public string Name { get; set; } = "Selenium Learner";
        public int DailyTokens { get; set; } = 5; // Har din 5 AI uses free
        public int LearningStreak { get; set; } = 0; // Lagatar kitne din padhai ki
        public DateTime LastLoginDate { get; set; } = DateTime.Now;

        // Ek simple function check karne ke liye ki kya naya din shuru ho gaya hai
        public void CheckAndResetDailyTokens()
        {
            if (LastLoginDate.Date < DateTime.Now.Date)
            {
                // Agar pichla login kal ya usse pehle ka tha, toh tokens wapas 5 kar do
                DailyTokens = 5;
                
                // Agar exact 1 din pehle aaye the, toh streak badha do
                if ((DateTime.Now.Date - LastLoginDate.Date).Days == 1)
                {
                    LearningStreak++;
                }
                else 
                {
                    LearningStreak = 1; // Streak toot gayi, wapas 1 se shuru
                }

                LastLoginDate = DateTime.Now;
            }
        }
    }
}