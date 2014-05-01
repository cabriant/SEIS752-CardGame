using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Business.Services
{
    public interface IUserService
    {
	    UserModel GetUser(string userId);
	    bool CreateUser(UserModel model);
	    bool UpdateUser(UserModel model);
    }
}
