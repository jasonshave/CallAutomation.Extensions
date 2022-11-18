// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.
namespace CallAutomation.Extensions.Interfaces;

public interface ICallbackContext<T> : IExecuteAsync<T>
{
    /// <summary>
    /// Setting custom context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns><see cref="IExecuteAsync"/></returns>
    IExecuteAsync<T> WithContext(IOperationContext context);
}

public interface ICallbackContext : IExecuteAsync
{
    /// <summary>
    /// Setting custom context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns><see cref="IExecuteAsync"/></returns>
    IExecuteAsync WithContext(IOperationContext context);
}
