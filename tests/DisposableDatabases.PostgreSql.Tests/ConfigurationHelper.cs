// <copyright file="ConfigurationHelper.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

namespace DisposableDatabases.PostgreSql.Tests;

public static class ConfigurationHelper
{
	public static string GetRequiredValue(string key)
	{
		string? value = GlobalTestSetup.Configuration?[key];

		Assume.That(value, Is.Not.Null, $"Configuration value for '{key}' is missing.");
		Assume.That(value, Is.Not.Empty, $"Configuration value for '{key}' is empty.");

		return value!;
	}
}
