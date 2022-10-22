// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallFrom
{
    ICallWithCallbackUri From(string from);

    ICallWithCallbackUri From(string from, Action<CallFromOptions> options);
}