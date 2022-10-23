// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

internal interface ICallAutomationHelperCallback
{
    IEnumerable<Type> Types { get; }

    CallAutomationCallbacks HelperCallbacks { get; }
}