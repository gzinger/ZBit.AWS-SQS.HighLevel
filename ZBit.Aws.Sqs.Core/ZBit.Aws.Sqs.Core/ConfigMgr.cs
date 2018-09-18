using Microsoft.Extensions.Configuration;
using System.IO;

namespace ZBit.Aws.Sqs.Core
{
	public class ConfigMgr {
		private static IConfigurationRoot _configuration;

		public static IConfigurationRoot Default {
			get {
				if (null == _configuration) {
					var mgr = new ConfigMgr();
					_configuration = mgr.BuildConfiguration();
				}
				return _configuration;
			}
		}

		public IConfigurationRoot BuildConfiguration() {
			var builder = new ConfigurationBuilder()
			 .SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json");
			return builder.Build();
		}
	}
}
