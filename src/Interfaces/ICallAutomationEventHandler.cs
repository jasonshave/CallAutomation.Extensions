// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

internal interface ICallAutomationEventHandler
{
    ValueTask Handle<T>(CallAutomationEventBase eventBase, T operationContext, string? id)
        where T : OperationContext?;
}