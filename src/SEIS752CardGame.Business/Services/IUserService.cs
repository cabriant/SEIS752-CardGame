using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Business.Services
{
    public interface IUserService
    {
	    UserModel GetUser(string userId);
	    UserModel AuthenticateUser(string email, string password);
	    bool CreateUser(UserModel model);
	    bool UpdateUser(UserModel model);
    }
}
