using Newtonsoft.Json;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Business.Utilities;
using System;
using System.Linq;

namespace SEIS752CardGame.Business.Services
{
	public class ConfigurationService : BaseService<ConfigurationService, IConfigurationService>, IConfigurationService
	{
		private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };

		private const int TWILIO_CONFIGURATION_TYPE = 0;
		private const int ENCRYPTION_CONFIGURATION_TYPE = 1;
		private const int GOOGLE_OAUTH_CONFIGURATION_TYPE = 2;

		private DateTime _expires;

		private TwilioConfig _twilioConfig;
		private EncryptionConfig _encryptionConfig;
		private GoogleOauthConfig _googleOauthConfig;

		public ConfigurationService()
		{
			_twilioConfig = new TwilioConfig();
			_encryptionConfig = new EncryptionConfig();
			_googleOauthConfig = new GoogleOauthConfig();

			ReloadAllConfigs();
		}

		#region Reload Configs

		private void ReloadAllConfigs()
		{
			ReloadTwilioConfig();
			ReloadEncryptionConfig();
			ReloadGoogleOauthConfig();

			_expires = DateTime.Now.AddMinutes(5);
		}

		private void ReloadTwilioConfig()
		{
			lock (this._twilioConfig)
			{
				var context = Database.GetContext();
				var config = (from c in context.configurations
							where c.config_type == TWILIO_CONFIGURATION_TYPE
							orderby c.version descending
							select c).FirstOrDefault();

				if (config == null)
					throw new Exception("No Config Exists!");

				this._twilioConfig = JsonConvert.DeserializeObject<TwilioConfig>(config.config, JsonSettings);
			}
		}

		private void ReloadEncryptionConfig()
		{
			lock (this._encryptionConfig)
			{
				var context = Database.GetContext();
				var config = (from c in context.configurations
							  where c.config_type == ENCRYPTION_CONFIGURATION_TYPE
							  orderby c.version descending
							  select c).FirstOrDefault();

				if (config == null)
					throw new Exception("No Config Exists!");

				this._encryptionConfig = JsonConvert.DeserializeObject<EncryptionConfig>(config.config, JsonSettings);
			}
		}

		private void ReloadGoogleOauthConfig()
		{
			lock (this._googleOauthConfig)
			{
				var context = Database.GetContext();
				var config = (from c in context.configurations
							  where c.config_type == GOOGLE_OAUTH_CONFIGURATION_TYPE
							  orderby c.version descending
							  select c).FirstOrDefault();

				if (config == null)
					throw new Exception("No Config Exists!");

				this._googleOauthConfig = JsonConvert.DeserializeObject<GoogleOauthConfig>(config.config, JsonSettings);
			}
		}

		#endregion

		public TwilioConfig GetTwilioConfiguration()
		{
			if (_expires <= DateTime.Now)
			{
				ReloadAllConfigs();
			}

			return this._twilioConfig;
		}

		public EncryptionConfig GetEncryptionConfig()
		{
			if (_expires <= DateTime.Now)
			{
				ReloadAllConfigs();
			}

			return this._encryptionConfig;
		}

		public GoogleOauthConfig GetGoogleOauthConfig()
		{
			if (_expires <= DateTime.Now)
			{
				ReloadAllConfigs();
			}

			return this._googleOauthConfig;
		}
	}
}
