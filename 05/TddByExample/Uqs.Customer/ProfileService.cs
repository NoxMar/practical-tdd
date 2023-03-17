using System.Text.RegularExpressions;

namespace Uqs.Customer;

public class ProfileService
{
    
    private const string AlphanumericUnderscoreRegex = @"^[a-zA-Z0-9_]+$";
    private static readonly Regex UsernameRegex = new (AlphanumericUnderscoreRegex, RegexOptions.Compiled);
    public void ChangeUsername(string username)
    {
        if (username is null)
        {
            throw new ArgumentNullException(nameof(username), "Null");
        }

        if (username.Length is < 8 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(username), "Length");
        }

        if (!UsernameRegex.Match(username).Success)
        {
            throw new ArgumentOutOfRangeException(nameof(username), "InvalidChars");
        }
    }
}