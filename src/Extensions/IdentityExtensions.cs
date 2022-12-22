// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace CallAutomation.Extensions.Extensions;

internal static class IdentityExtensions
{
    public static CommunicationIdentifier ConvertToCommunicationIdentifier(this string id)
    {
        if (id.StartsWith("+", StringComparison.OrdinalIgnoreCase))
            return new PhoneNumberIdentifier(id);

        if (id.StartsWith("4:+", StringComparison.OrdinalIgnoreCase))
            return new PhoneNumberIdentifier(id.Substring("4:".Length));

        return CommunicationIdentifier.FromRawId(id);
    }
}