// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Models;
using FluentAssertions;

namespace CallAutomation.Extensions.UnitTests
{
    public class DtmfToneCallbackTests
    {
        [Fact]
        public void ToneMap_Adds_CorrectTone()
        {
            // arrange
            var subject = new DtmfToneCallbacks();

            // act
            subject.AddCallback<One>(() => "TheQuickBrownFoxJumpedOverTheLazyDog");
            var result = subject.GetCallbacks(default(One));

            // assert
            result.Count.Should().Be(1);
            result.FirstOrDefault().DynamicInvoke().Should().Be("TheQuickBrownFoxJumpedOverTheLazyDog");
        }
    }
}