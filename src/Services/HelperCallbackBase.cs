// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Services;

internal abstract class HelperCallbackBase : ICallAutomationHelperCallback
{
    public IEnumerable<Type> Types { get; }

    public CallAutomationCallbacksLegacy HelperCallbacks { get; }

    protected string RequestId { get; }

    protected HelperCallbackBase(string requestId, IEnumerable<Type> types)
    {
        HelperCallbacks = new CallAutomationCallbacksLegacy(requestId);
        RequestId = requestId;
        Types = types.ToList();

        CallbackRegistry.RegisterHelperCallback(this, Types);
    }
}