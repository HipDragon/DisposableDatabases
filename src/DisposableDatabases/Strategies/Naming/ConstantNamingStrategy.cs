// <copyright file="ConstantNamingStrategy.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using CommunityToolkit.Diagnostics;
using DisposableDatabases.Interfaces.Strategies;

namespace DisposableDatabases.Strategies.Naming;

/// <summary>
/// Represents a strategy that uses a constant name for the naming process.
/// </summary>
/// <remarks>
/// This does not generate unique names when <see cref="GenerateName" /> is called.
/// </remarks>
public class ConstantNamingStrategy : INamingStrategy
{
	/// <summary>
	/// The constant name specified.
	/// </summary>
	private readonly string _name;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstantNamingStrategy" /> class.
	/// </summary>
	/// <param name="name">The constant name to be used.</param>
	/// <exception cref="ArgumentException">Thrown when the name is either empty or whitespace.</exception>
	/// <exception cref="ArgumentNullException">Thrown when the name is null.</exception>
	public ConstantNamingStrategy(string? name)
	{
		Guard.IsNotNullOrWhiteSpace(name);

		_name = name;
	}

	/// <summary>
	/// Returns the constant name configured by this naming strategy.
	/// </summary>
	/// <returns>A string representing the name.</returns>
	public string GenerateName()
	{
		return _name;
	}
}
