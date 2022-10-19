// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Models;

public sealed class CallAutomationSdkClientOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public Uri? OverrideEndpointUri { get; set; }
}