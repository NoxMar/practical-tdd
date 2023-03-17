namespace Uqs.Customer;

public class ProfileService
{
    public void ChangeUsername(string username)
    {
        if (username is null)
        {
            throw new ArgumentNullException(nameof(username), "Null");
        }

        if (username.Length < 8 || username.Length > 12)
        {
            throw new ArgumentOutOfRangeException("username", "Length");
        }
    }
}