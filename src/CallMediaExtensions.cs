// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Helpers;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions;

public static class CallMediaExtensions
{
    public static IPlayMediaCallback Play(this CallMedia callMedia, Action<PlayMediaOptions> options)
    {
        var playOptions = new PlayMediaOptions();
        options(playOptions);
        return new CallAutomationPlayHelper(callMedia, playOptions, Guid.NewGuid().ToString());
    }

    public static IRecognizeDtmf ReceiveDtmfTones(this CallMedia callMedia, int numTones)
    {
        return new CallAutomationRecognizeDtmfHelper(callMedia, numTones, Guid.NewGuid().ToString());
    }
}