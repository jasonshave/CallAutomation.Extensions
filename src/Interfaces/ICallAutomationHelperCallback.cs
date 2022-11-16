// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Interfaces;

internal interface ICallAutomationHelperCallback
{
    IEnumerable<Type> Types { get; }

    CallAutomationCallbacksLegacy HelperCallbacks { get; }
}