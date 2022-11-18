// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Models;

public abstract class OperationContext
{
    public abstract string RequestId { get; }

    public virtual string? Payload { get; set; }

    public virtual Type? PayloadType { get; set; }
}