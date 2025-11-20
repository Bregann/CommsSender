using CommsSender.Domain.Data.Database.Models;

namespace CommsSender.Domain.Interfaces.Helpers
{
    public interface IUserContextHelper
    {
        string GetUserId();
        string GetUserFirstName();
        User GetUser();
    }
}
