using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Business.Services
{
	public interface IGoogleOauthService
	{
		string CreateInitialAuthUrl(string clientId, string redirectUri, string state, string scope);
		GoogleAccessTokenResponse RetrieveAccessToken(string authorizationCode, string clientId, string clientSecret,
			string redirectUri);
		GoogleProfileResponse RetrieveUserProfile(string accessToken);
	}
}