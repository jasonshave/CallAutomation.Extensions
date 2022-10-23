// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface IRecognizeDtmf
{
    /// <summary>
    /// Collect DTMF tones from a participant on the call.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ICanChooseRecognizeOptions FromParticipant(string id);
}