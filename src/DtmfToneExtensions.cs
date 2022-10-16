// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions;

public static class DtmfToneExtensions
{
    public static IDtmfTone Convert(this DtmfTone tone)
    {
        if (tone == DtmfTone.One)
        {
            return default(One);
        }

        if (tone == DtmfTone.Two)
        {
            return default(Two);
        }

        return default(One);
    }
}