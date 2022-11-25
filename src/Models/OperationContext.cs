// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Converters;
using CallAutomation.Extensions.Interfaces;
using System.Text.Json.Serialization;

namespace CallAutomation.Extensions.Models
{
    public class OperationContext
    {
        [JsonConverter(typeof(OperationContextJsonConverter))]
        public IPayload? Payload { get; set; }

        public string? RequestId { get;  set; }
    }
}
