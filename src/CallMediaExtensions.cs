// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Helpers;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions;

public static class CallMediaExtensions
{
    /// <summary>
    /// Initiates the Play audio sequence while allowing <see cref="PlayMediaOptions"/> configuration.
    /// </summary>
    /// <param name="callMedia"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IPlayMediaCallbackWithHandler Play(this CallMedia callMedia, Action<PlayMediaOptions> options)
    {
        var playOptions = new PlayMediaOptions();
        options(playOptions);
        return new CallAutomationPlayHelper(callMedia, playOptions, Guid.NewGuid().ToString());
    }

    public static IPlayMediaCallback Speak(this CallMedia callMedia, string textToSpeak)
    {
        return new CallAutomationPlayHelper(callMedia, textToSpeak, Guid.NewGuid().ToString());
    }

    /// <summary>
    /// Initiates the Recognize API.
    /// </summary>
    /// <param name="callMedia"></param>
    /// <returns></returns>
    public static IRecognizeDtmf ReceiveDtmfTone(this CallMedia callMedia)
    {
        return new CallAutomationDtmfHelper(callMedia, Guid.NewGuid().ToString());
    }
}