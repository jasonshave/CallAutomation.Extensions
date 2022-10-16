﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Models;

public struct Five : IDtmfTone
{
    public DtmfTone Value => DtmfTone.Five;
}