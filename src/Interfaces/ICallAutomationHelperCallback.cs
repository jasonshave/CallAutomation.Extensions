// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;

namespace CallAutomation.Extensions.Interfaces;

internal interface ICallAutomationHelperCallback
{
    IEnumerable<Type> Types { get; }

    public IOperationContext? OperationContext { get; }

    void AddDelegateCallback<T>(Delegate callback);

    void AddHandlerCallback<THandler, T>(string methodName, params Type[] methodParameters)
        where THandler : CallAutomationHandler;

    IEnumerable<Delegate> GetDelegateCallbacks(Type type);

    IEnumerable<(MethodInfo, Type)> GetHandlers(Type type);
}