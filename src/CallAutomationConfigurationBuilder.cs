// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;

namespace CallAutomation.Extensions;

public sealed class CallAutomationConfigurationBuilder
{
    public IServiceCollection Services { get; }


    public CallAutomationConfigurationBuilder(IServiceCollection services)
    {
        Services = services;
    }
}