// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface IExecuteAsync<T> : ICallbackContext<IExecuteAsync<T>>
{
    ///// <summary>
    ///// Setting custom context.
    ///// </summary>
    ///// <param name="context"></param>
    ///// <returns><see cref="IExecuteAsync{T}"/></returns>
    //IExecuteAsync<T> WithContext(IOperationContext context);

    /// <summary>
    /// Executes the create call process.
    /// </summary>
    /// <returns><see cref="T"/></returns>
    ValueTask<T> ExecuteAsync();
}

public interface IExecuteAsync : ICallbackContext<IExecuteAsync>
{
    /// <summary>
    /// Executes the create call process.
    /// </summary>
    /// <returns></returns>
    ValueTask ExecuteAsync();
}