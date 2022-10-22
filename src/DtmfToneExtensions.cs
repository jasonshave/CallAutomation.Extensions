// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions;

public static class DtmfToneExtensions
{
    public static IDtmfTone? Convert(this DtmfTone tone)
    {
        if (tone == DtmfTone.One) return default(One);
        if (tone == DtmfTone.Two) return default(Two);
        if (tone == DtmfTone.Three) return default(Three);
        if (tone == DtmfTone.Four) return default(Four);
        if (tone == DtmfTone.Five) return default(Five);
        if (tone == DtmfTone.Six) return default(Six);
        if (tone == DtmfTone.Seven) return default(Seven);
        if (tone == DtmfTone.Eight) return default(Eight);
        if (tone == DtmfTone.Nine) return default(Nine);
        if (tone == DtmfTone.Zero) return default(Zero);

        if (tone == DtmfTone.Pound) return default(Pound);
        if (tone == DtmfTone.Asterisk) return default(Asterisk);

        return null;
    }
}