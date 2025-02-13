// <copyright file="UserDefinedDatabaseNamingStrategy.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using DisposableDatabases.Interfaces.Strategies;

namespace DisposableDatabases.Strategies.DatabaseNaming;

/// <summary>
/// Represents a strategy that uses a user-defined database name for the naming process.
/// </summary>
/// <remarks>
/// This does not generate unique database names when <see cref="GenerateDatabaseName" /> is called.
/// </remarks>
public class UserDefinedDatabaseNamingStrategy : IDatabaseNamingStrategy
{
	/// <summary>
	/// The name of the database specified by the user.
	/// </summary>
	private readonly string _databaseName;

	/// <summary>
	/// Initializes a new instance of the <see cref="UserDefinedDatabaseNamingStrategy" /> class.
	/// </summary>
	/// <param name="databaseName">The name of the database specified by the user.</param>
	/// <exception cref="ArgumentException">Thrown when database name is null, empty, or whitespace.</exception>
	public UserDefinedDatabaseNamingStrategy([NotNull] string? databaseName)
	{
		Guard.IsNotNullOrWhiteSpace(databaseName);

		_databaseName = databaseName;
	}

	/// <summary>
	/// Generates and returns the name of the database based on a user-specified naming strategy.
	/// </summary>
	/// <returns>A string representing the name of the database.</returns>
	public string GenerateDatabaseName()
	{
		return _databaseName;
	}
}
