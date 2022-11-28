// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Converters;
using CallAutomation.Extensions.Interfaces;
using System.Text.Json.Serialization;

namespace CallAutomation.Extensions.Models
{
    public class OperationContext : IOperationContext
    {
        [JsonInclude]
        public string? RequestId { get; internal set; }
    }
}
