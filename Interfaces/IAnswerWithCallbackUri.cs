﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;

public interface IAnswerWithCallbackUri
{
    IAnswerCallback WithCallbackUri(string callbackUri);
}