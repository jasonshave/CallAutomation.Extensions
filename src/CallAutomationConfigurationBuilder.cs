// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;

namespace JasonShave.Azure.Communication.CallAutomation.Extensions;

public class CallAutomationConfigurationBuilder
{
    public IServiceCollection Services { get; }

    public CallAutomationConfigurationBuilder(IServiceCollection services) => Services = services;
}