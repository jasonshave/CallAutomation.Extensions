// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface ICanRecognizeDtmfOptions
{
    /// <summary>
    /// Allows specification of the recognize API options through the <see cref="RecognizeOptions"/>
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    IHandleDtmfResponseWithHandler WithOptions(Action<RecognizeOptions> options);
}