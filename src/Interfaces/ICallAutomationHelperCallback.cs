// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

internal interface ICallAutomationHelperCallback
{
    ICallbacksHandler CallbackHandler { get; }
}