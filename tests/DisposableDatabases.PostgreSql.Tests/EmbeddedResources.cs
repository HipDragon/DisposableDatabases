// <copyright file="EmbeddedResources.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

namespace DisposableDatabases.PostgreSql.Tests;

internal static class EmbeddedResources
{
	private const string ResourcePrefix = "DisposableDatabases.PostgreSql.Tests.";

	private const string ScriptsFolder = ResourcePrefix + "Scripts.";

	internal const string TestSqlScript = ScriptsFolder + "TestSqlScript.sql";
}
