using Newtonsoft.Json;
using SEIS752CardGame.Business.Models;
using System.Net;
using System.Web;

namespace SEIS752CardGame.Business.Services
{
	public class GoogleOauthService : BaseService<GoogleOauthService, IGoogleOauthService>, IGoogleOauthService
	{
		private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };

		private const string GOOGLE_OAUTH_URL = "https://accounts.google.com/o/oauth2/auth?response_type=code&client_id={0}&redirect_uri={1}&state={2}&scope={3}";
		private const string GOOGLE_OAUTH_TOKEN_URL = "https://accounts.google.com/o/oauth2/token";
		private const string OAUTH_TOKEN_DATA = "code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code";
		private const string GOOGLE_PROFILE_API_URL = "https://www.googleapis.com/userinfo/v2/me";

		public string CreateInitialAuthUrl(string clientId, string redirectUri, string state, string scope)
		{
			var url = string.Format(GOOGLE_OAUTH_URL, clientId, HttpUtility.UrlEncode(redirectUri), HttpUtility.UrlEncode(state), scope);
			return url;
		}

		public GoogleAccessTokenResponse RetrieveAccessToken(string authorizationCode, string clientId, string clientSecret,
			string redirectUri)
		{
			var webClient = new WebClient();

			var data = string.Format(OAUTH_TOKEN_DATA, authorizationCode, clientId, clientSecret, redirectUri);

			webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
			var response = webClient.UploadString(GOOGLE_OAUTH_TOKEN_URL, data);

			var tokenResponse = JsonConvert.DeserializeObject<GoogleAccessTokenResponse>(response, JsonSettings);

			return tokenResponse;
		}

		public GoogleProfileResponse RetrieveUserProfile(string accessToken)
		{
			var webClient = new WebClient();

			webClient.Headers.Add(HttpRequestHeader.Authorization, string.Format("Bearer {0}", accessToken));
			
			var response = webClient.DownloadString(GOOGLE_PROFILE_API_URL);

			var profileResponse = JsonConvert.DeserializeObject<GoogleProfileResponse>(response, JsonSettings);

			return profileResponse;
		}
	}
}
