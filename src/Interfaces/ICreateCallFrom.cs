// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface ICreateCallFrom
{
    ICreateCallWithCallbackUri From(string from);

    ICreateCallWithCallbackUri From(string from, Action<CallFromOptions> options);
}