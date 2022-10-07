// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace JasonShave.Azure.Communication.CallAutomation.Extensions.Interfaces;

public interface ICanExecuteAsync
{
    ValueTask ExecuteAsync();
}