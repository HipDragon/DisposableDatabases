// <copyright file="IDatabaseNamingStrategy.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

namespace DisposableDatabases.Interfaces.Strategies;

/// <summary>
/// An interface for defining the strategy to generate database names.
/// </summary>
public interface IDatabaseNamingStrategy
{
	/// <summary>
	/// Generates a database name based on the implementation strategy.
	/// </summary>
	/// <returns>A string representing the generated database name.</returns>
	string GenerateDatabaseName();
}
