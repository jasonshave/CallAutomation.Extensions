// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface IOperationContext
{
    string RequestId { get; }

    string? Payload { get; }

    string? PayloadType { get; }
}