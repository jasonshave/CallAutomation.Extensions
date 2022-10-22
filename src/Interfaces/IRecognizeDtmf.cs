﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface IRecognizeDtmf
{
    ICanChooseRecognizeOptions FromParticipant(string rawId);
}