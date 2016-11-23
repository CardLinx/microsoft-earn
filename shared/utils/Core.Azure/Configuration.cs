//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Azure
{
	using System;
	using System.Reflection;
	using System.Configuration;

	using Microsoft.WindowsAzure.ServiceRuntime;

	using Logging;
	using Extensions;
	using Reflection;

	public static class Configuration
	{
		public static bool IsAzure = true;

		public static void Apply(Type type)
		{
			var properties = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (var property in properties)
			{
				var attribute = property.GetCustomAttribute<ConfigurationAttribute>();
				if (attribute != null)
				{
					var value = Convert.ChangeType(GetConfigurationValue(attribute), property.PropertyType);
					property.SetValue(null, value);
				}
			}
		}

		public static string GetConfigurationValue(ConfigurationAttribute attribute)
		{
			// there are a few problems with doing it differently, more precisely
			// with trying to find at runtime which environment we are running in:

			// #1: when running in the console application, WindowsAzure.ServiceRuntime.dll fails to load
			// so we can't even check RoleEnvironment.IsAvailable

			// #2: when invoked by a test within Visual Studio, RoleEnvironment.IsAvailable returns true
			// and then, of course, fails to read configuration

			// so there

			try
			{
				if (IsAzure)
					return RoleEnvironment.GetConfigurationSettingValue(attribute.Name);
                return ConfigurationManager.AppSettings[attribute.Name];
			}
			catch (Exception e)
			{
				if (attribute.DefaultValue == null)
				{
					Log.Error("Unable to read configuration variable [{0}]".ExpandWith(attribute.Name));
					Log.Error(e);
					throw;
				}
				Log.Warning("Unable to read configuration variable [{0}], using default value [{0}]".ExpandWith(attribute.Name, attribute.DefaultValue));
				return attribute.DefaultValue;
			}
		}
	}
}