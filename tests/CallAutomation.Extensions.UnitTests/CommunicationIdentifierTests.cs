// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using CallAutomation.Extensions.Extensions;
using FluentAssertions;

namespace CallAutomation.Extensions.UnitTests;

public class CommunicationIdentifierTests
{
    [Fact]
    public void Input_ConvertsTo_CorrectType()
    {
        // arrange
        var phoneNumber1 = "+14255551212";
        var phoneNumber2 = "4:+14255551212";

        // act
        var result1 = phoneNumber1.ConvertToCommunicationIdentifier();
        var result2 = phoneNumber2.ConvertToCommunicationIdentifier();

        // assert
        result1.Should().BeOfType<PhoneNumberIdentifier>();
        result2.Should().BeOfType<PhoneNumberIdentifier>();
    }
}