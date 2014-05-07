using SEIS752CardGame.Business.Models;

namespace SEIS752CardGame.Business.Services
{
	public interface IConfigurationService
	{
		TwilioConfig GetTwilioConfiguration();
		EncryptionConfig GetEncryptionConfig();
	}
}