// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Models;
using System.Reflection;
using System.Text.Json;

namespace CallAutomation.Extensions.Interfaces;

internal interface ICallAutomationHelperCallback
{
    IEnumerable<Type> Types { get; }

    OperationContext Context { get; }

    void SetContext(OperationContext context);

    string GetSerializedContext(JsonSerializerOptions? serializerOptions = null);

    void AddDelegateCallback<T>(Delegate callback);

    void AddHandlerCallback<THandler, T>(string methodName, params Type[] methodParameters)
        where THandler : CallAutomationHandler;

    IEnumerable<Delegate> GetDelegateCallbacks(Type type);

    IEnumerable<(MethodInfo, Type)> GetHandlers(Type type);
}