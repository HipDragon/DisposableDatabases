// <copyright file="IDisposableDatabase.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

namespace DisposableDatabases.Interfaces;

/// <summary>
/// IDisposableDatabase purpose is to allow the creation of disposable database.
/// </summary>
public interface IDisposableDatabase : IDisposable, IAsyncDisposable
{
	/// <summary>
	/// The connection string to the disposable database created.
	/// </summary>
	string ConnectionString { get; }

	/// <summary>
	/// The database name for this disposable database
	/// </summary>
	string DatabaseName { get; }

	/// <summary>
	/// Prevents the disposable database from being dropped, preserving its state.
	/// </summary>
	/// <remarks>
	/// When this method is invoked, the associated database will be retained even when the instance is disposed.
	/// This can be useful for scenarios where the database needs to be preserved for later inspection or use.
	/// </remarks>
	bool PreserveDatabase { get; set; }
}
