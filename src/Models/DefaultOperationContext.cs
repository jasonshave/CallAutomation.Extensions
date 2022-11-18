// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Models;

internal sealed class DefaultOperationContext : OperationContext
{
    public override string RequestId { get; }

    public DefaultOperationContext()
    {
        RequestId = Guid.NewGuid().ToString();
    }
}