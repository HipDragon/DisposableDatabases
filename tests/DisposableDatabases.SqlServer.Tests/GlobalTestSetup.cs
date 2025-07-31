// <copyright file="GlobalTestSetup.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using Microsoft.Extensions.Configuration;

namespace DisposableDatabases.SqlServer.Tests;

[SetUpFixture]
public class GlobalTestSetup
{
	public static IConfiguration? Configuration { get; private set; }

	[OneTimeSetUp]
	public void Setup()
	{
		IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddUserSecrets<GlobalTestSetup>()
		                                                                       .AddEnvironmentVariables();

		Configuration = configurationBuilder.Build();
	}
}
