// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Models;

public class OperationContext
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();

    public string? Payload { get; set; }

    public string? PayloadType { get; set; }
}