namespace Uqs.Customer;

public class ProfileService
{
    public void ChangeUsername(string username)
    {
        if (username is null)
        {
            throw new ArgumentNullException("username", "Null");
        }
    }
}