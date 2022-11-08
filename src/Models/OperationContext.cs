// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Models
{
    public class OperationContext : IOperationContext
    {
        public string? RequestID { get; set; }
    }
}
