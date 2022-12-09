// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Services;

internal abstract class HelperCallbackBase : ICallAutomationHelperCallback
{
    public ICallbacksHandler CallbackHandler { get; protected set; }

    protected string RequestId { get; }

    protected HelperCallbackBase(string requestId)
    {
        CallbackHandler = new CallAutomationCallbacks();
        RequestId = requestId;
    }
}