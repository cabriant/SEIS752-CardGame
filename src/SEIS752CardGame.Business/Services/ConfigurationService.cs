using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SEIS752CardGame.Business.Models;
using SEIS752CardGame.Business.Utilities;

namespace SEIS752CardGame.Business.Services
{
	public class ConfigurationService : BaseService<ConfigurationService, IConfigurationService>, IConfigurationService
	{
		private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects };

		private const int TWILIO_CONFIGURATION_TYPE = 0;
		private const int ENCRYPTION_CONFIGURATION_TYPE = 1;

		private DateTime _expires;

		private TwilioConfig _twilioConfig;
		private EncryptionConfig _encryptionConfig;

		public ConfigurationService()
		{
			_twilioConfig = new TwilioConfig();
			_encryptionConfig = new EncryptionConfig();

			ReloadAllConfigs();
		}

		#region Reload Configs

		private void ReloadAllConfigs()
		{
			ReloadTwilioConfig();
			ReloadEncryptionConfig();

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

		#endregion

		#region NAS Config

		public TwilioConfig GetTwilioConfiguration()
		{
			if (_expires <= DateTime.Now)
			{
				ReloadAllConfigs();
			}

			return this._twilioConfig;
		}

		#endregion

		#region File Manager Config

		public EncryptionConfig GetEncryptionConfig()
		{
			if (_expires <= DateTime.Now)
			{
				ReloadAllConfigs();
			}

			return this._encryptionConfig;
		}

		#endregion
	}
}
