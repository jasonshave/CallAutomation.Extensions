// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface ICallbacksHandler
{
    void AddDelegateCallback<T>(string requestId, Delegate callback);

    void AddHandlerCallback<THandler, T>(string requestId, string methodName)
        where THandler : CallAutomationHandler;

    IEnumerable<Delegate> GetDelegateCallbacks(string requestId, Type type);

    IEnumerable<(string HandlerName, string MethodName)> GetHandlers(string requestId, Type type);
}
