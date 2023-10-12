using System.Text.RegularExpressions;

namespace DrTrottoirApi.Validators
{
    public class KboValidator
    {
        private const string KboRegexPattern = @"^\d{4}.\d{3}.\d{3}$";

        public static bool IsValid(string kboNumber)
        {
            if (string.IsNullOrWhiteSpace(kboNumber))
            {
                return false;
            }

            Regex regex = new Regex(KboRegexPattern);

            return regex.IsMatch(kboNumber);
        }
    }
}
