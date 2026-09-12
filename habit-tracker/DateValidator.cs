using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// This class and method were created for unit tests.
namespace habit_tracker
{
    public static class DateValidator
    {
        public static bool IsValidDate(string dateInput)
        {
            return DateTime.TryParseExact(
                dateInput,
                "dd-MM-yy",
                new CultureInfo("en-US"),
                DateTimeStyles.None,
                out _
            );
        }
    }
}
