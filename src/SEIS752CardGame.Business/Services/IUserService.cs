using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Business.Services
{
    public interface IUserService
    {
	    UserModel GetUser(string userId);
	    UserModel AuthenticateUser(string email, string password);
	    bool CreateUser(UserModel model);
	    bool UpdateUser(UserModel model);
	    bool CheckEmailInUse(string email);
	    bool CheckDisplayNameInUse(string displayName);
	    bool UpdateUserCashValue(string userId, int amtAddSub);
	    int GetUserAccountValue(string userId);
		string CreateResetCode(string email, out string phoneNumber);
	    string ValidateCodeAndCreateToken(string email, string code);
	    bool ValidateTokenAndResetPassword(string email, string token, string newPassword);
    }
}
