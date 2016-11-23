//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Reflection
{
	using System;
	using System.Reflection;
	using System.Configuration;

	using Logging;
	using Extensions;
	using Interfaces;

	public class StaticConfigurationResolver : IStaticConfigurationResolver
	{
		public void Apply(Type type)
		{
			var properties = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (var property in properties)
			{
				var attribute = property.GetCustomAttribute<ConfigurationAttribute>();
				if (attribute != null)
				{
					var value = Convert.ChangeType(TryGetConfigurationValue(attribute), property.PropertyType);
					property.SetValue(null, value);
				}
			}
		}

		private string TryGetConfigurationValue(ConfigurationAttribute attribute)
		{
			try
			{
				return GetConfigurationValue(attribute);
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
		
		public virtual string GetConfigurationValue(ConfigurationAttribute attribute)
		{
			return ConfigurationManager.AppSettings[attribute.Name];			
		}
	}
}