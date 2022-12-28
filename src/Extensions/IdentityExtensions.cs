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

        return CommunicationIdentifier.FromRawId(id);
    }
}