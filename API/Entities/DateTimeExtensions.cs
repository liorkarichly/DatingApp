using System;

namespace API.Entities
{
    public static class DateTimeExtensions
    {

        public static int CalculatorAge(this DateTime i_DateBirth)
        {

            var today = DateTime.Today;
            var age = today.Year - i_DateBirth.Year;

            //Check if the birthday is gratter than today because they haven't birthday
            if (i_DateBirth > today.AddYears(-age)){

                age--;
            
            }

            return age;

        }
        
    }
}