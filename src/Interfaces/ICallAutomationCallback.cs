// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

internal interface ICallAutomationCallback<in T>
{
    string RequestId { get; }

    List<Delegate> GetCallbacks(T value);
}