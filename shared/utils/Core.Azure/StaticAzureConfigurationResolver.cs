//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Azure
{
	using Microsoft.WindowsAzure.ServiceRuntime;
	using Reflection;

	public sealed class StaticAzureConfigurationResolver : StaticConfigurationResolver
	{
		public override string GetConfigurationValue(ConfigurationAttribute attribute)
		{
			return RoleEnvironment.GetConfigurationSettingValue(attribute.Name);
		}
	}
}