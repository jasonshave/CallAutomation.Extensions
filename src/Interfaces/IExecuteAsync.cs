// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface IExecuteAsync<T>
{
    /// <summary>
    /// Executes the create call process.
    /// </summary>
    /// <returns><see cref="T"/></returns>
    ValueTask<T> ExecuteAsync(IOperationContext operationContext);
}

public interface IExecuteAsync
{
    /// <summary>
    /// Executes the create call process.
    /// </summary>
    /// <returns></returns>
    ValueTask ExecuteAsync(IOperationContext operationContext);
}