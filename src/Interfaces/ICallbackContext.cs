// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.
namespace CallAutomation.Extensions.Interfaces;

public interface ICallbackPayload<T> : IExecuteAsync<T>
{
    /// <summary>
    /// Setting custom context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns><see cref="IExecuteAsync"/></returns>
    IExecuteAsync<T> WithPayload(IPayload payload);
}

public interface ICallbackPayLoad : IExecuteAsync
{
    /// <summary>
    /// Setting custom context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns><see cref="IExecuteAsync"/></returns>
    IExecuteAsync WithPayload(IPayload payload);
}
