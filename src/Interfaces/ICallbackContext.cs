// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallbackContext<T> : IExecuteAsync<T>
{
    /// <summary>
    /// Setting custom context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns><see cref="IExecuteAsync"/></returns>
    IExecuteAsync<T> WithContext(OperationContext context);
}

public interface ICallbackContext : IExecuteAsync
{
    /// <summary>
    /// Setting custom context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns><see cref="IExecuteAsync"/></returns>
    IExecuteAsync WithContext(OperationContext context);
}
