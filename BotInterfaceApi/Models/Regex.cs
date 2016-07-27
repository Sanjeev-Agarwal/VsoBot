using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace BotInterfaceApi.Helpers
{
   
    public class RegexUtilities
    {
        bool invalid = false;
        readonly string pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        public string GetValidEmailId(string strIn)
        {
            if (String.IsNullOrEmpty(strIn))
                return string.Empty;

            strIn = strIn.Replace(" ", string.Empty);
            strIn = strIn.Replace("mailto:", string.Empty);

            Regex emailRegex = new Regex(pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            MatchCollection emailMatches = emailRegex.Matches(strIn);
            return emailMatches[0].Value;
        }
    
        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            strIn = strIn.Replace(" ", string.Empty);
            strIn = strIn.Replace("mailto:", string.Empty);

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
}