using System.Text.RegularExpressions;

namespace Uqs.Customer;

public class ProfileService
{
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

        if (!Regex.Match(username, @"^[a-zA-Z0-9_]+$").Success)
        {
            throw new ArgumentOutOfRangeException("username", "InvalidChars");
        }
    }
}