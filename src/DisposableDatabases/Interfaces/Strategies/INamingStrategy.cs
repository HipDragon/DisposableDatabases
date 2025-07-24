// <copyright file="INamingStrategy.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

namespace DisposableDatabases.Interfaces.Strategies;

/// <summary>
/// An interface for defining the strategy to generate names.
/// </summary>
public interface INamingStrategy
{
	/// <summary>
	/// Generates a name based on the implementation strategy.
	/// </summary>
	/// <returns>A string representing the generated name.</returns>
	string GenerateName();
}
