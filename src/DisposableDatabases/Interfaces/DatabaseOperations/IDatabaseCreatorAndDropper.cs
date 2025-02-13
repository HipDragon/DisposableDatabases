// <copyright file="IDatabaseCreatorAndDropper.cs" company="DisposableDatabases">
//     Copyright (c) 2022 Joshua B Raymond. All rights reserved.
// </copyright>

namespace DisposableDatabases.Interfaces.DatabaseOperations;

/// <summary>
/// Provides methods to create and drop a database.
/// </summary>
public interface IDatabaseCreatorAndDropper : IDatabaseCreator, IDatabaseDropper
{
}
