// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Services;

internal abstract class HelperCallbackBase : ICallAutomationHelperCallback
{
    public CallAutomationCallbacks HelperCallbacks { get; }

    protected string RequestId { get; }

    protected HelperCallbackBase(string requestId)
    {
        HelperCallbacks = new CallAutomationCallbacks(requestId);
        RequestId = requestId;

        CallbackRegistry.RegisterHelperCallback(this);
    }
}