// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Converters;
using System.Text.Json.Serialization;

namespace CallAutomation.Extensions.Interfaces;

public interface IOperationContext
{
    public string? RequestId { get; }
}