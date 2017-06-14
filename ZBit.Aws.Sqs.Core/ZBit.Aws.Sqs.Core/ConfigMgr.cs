using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZBit.Aws.Sqs.Core {
	public class ConfigMgr {
		private static IConfigurationRoot _Configuration = null;

		public static IConfigurationRoot Default {
			get {
				if (null == _Configuration) {
					var mgr = new ConfigMgr();
					_Configuration = mgr.BuildConfiguration();
				}
				return _Configuration;
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
