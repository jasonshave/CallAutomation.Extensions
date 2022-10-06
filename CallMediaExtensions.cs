// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Helpers;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Models;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions;

public static class CallMediaExtensions
{
    public static IPlayMediaCallback Play(this CallMedia callMedia, Action<PlayMediaOptions> options)
    {
        var playOptions = new PlayMediaOptions();
        options(playOptions);
        return new CallAutomationPlayHelper(callMedia, playOptions, Guid.NewGuid().ToString());
    }
}