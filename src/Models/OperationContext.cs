// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Models
{
    public class OperationContext : IOperationContext
    {
        public string? RequestId { get; set; }
    }
}
