// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Models;

public class DefaultOperationContext : IOperationContext
{
    public string RequestId { get; } = Guid.NewGuid().ToString();

    public string? Payload => null;

    public string? PayloadType => null;
}