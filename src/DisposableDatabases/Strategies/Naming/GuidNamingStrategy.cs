// <copyright file="GuidNamingStrategy.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

using System.Globalization;
using CommunityToolkit.Diagnostics;
using DisposableDatabases.Interfaces.Strategies;

namespace DisposableDatabases.Strategies.Naming;

/// <summary>
/// A naming strategy that generates a name using a GUID with a specified format.
/// </summary>
public class GuidNamingStrategy : INamingStrategy
{
	private const string DefaultGuidFormat = "N";
	private readonly string _guidFormat;

	/// <summary>
	/// Initializes a new instance of the <see cref="GuidNamingStrategy" /> class with the default GUID format.
	/// </summary>
	public GuidNamingStrategy() : this(DefaultGuidFormat)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GuidNamingStrategy" /> class with the specified GUID format.
	/// </summary>
	/// <param name="guidFormat">The format string for the GUID, which will be validated.</param>
	/// <exception cref="ArgumentException">Thrown when the GUID format is null, whitespace, or invalid.</exception>
	public GuidNamingStrategy(string? guidFormat)
	{
		Guard.IsNotNullOrWhiteSpace(guidFormat);

#pragma warning disable CA1062 // Validate arguments of public methods - Done by Guard.IsNotNull
		if (!IsValidGuidFormat(guidFormat))
#pragma warning restore CA1062
		{
			ThrowHelper.ThrowArgumentException(nameof(guidFormat), "Invalid GUID format specified.");
		}

		_guidFormat = guidFormat;
	}

	/// <summary>
	/// Generates a unique name using a GUID with the specified format.
	/// </summary>
	/// <returns>A unique name.</returns>
	public virtual string GenerateName()
	{
		return Guid.NewGuid().ToString(_guidFormat, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Validates if the provided GUID format is acceptable.
	/// </summary>
	/// <param name="guidFormat">The format string for the GUID, which will be validated.</param>
	/// <returns>True if the format is valid; otherwise, false.</returns>
	private static bool IsValidGuidFormat(string guidFormat)
	{
		switch (guidFormat.ToUpper(CultureInfo.InvariantCulture))
		{
			case "N":
			case "D":
			case "B":
			case "P":
			case "X":
				return true;
			default:
				return false;
		}
	}
}
