﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

using Microsoft.CodeAnalysis.Sarif.PatternMatcher.Sdk;

using Xunit;

namespace Microsoft.CodeAnalysis.Sarif.PatternMatcher.Plugins.Security.Validators
{
    public class GitHubPatValidatorTests
    {
        [Fact]
        public void GitHubPatValidator_TestStatic()
        {
            ValidationState expectedValidationState = ValidationState.NoMatch;

            string matchedPattern = "ghp_stuff"; // Insert new GitHub PAT here
            Dictionary<string, string> groups = new Dictionary<string, string>();
            groups.Add("secret", "");
            groups.Add("checksum", "");

            string failureLevel = null;

            string message = null;

            ValidationState actualValidationState = GitHubPatValidator.IsValidStatic(ref matchedPattern, ref groups, ref failureLevel, ref message, out Fingerprint fingerprint);

            Assert.Equal(matchedPattern, fingerprint.Secret);
            Assert.Equal(AssetPlatform.GitHub.ToString(), fingerprint.Platform);
            Assert.Equal(expectedValidationState, actualValidationState);
        }

        [Fact]
        public void GitHubPatValidator_TestDynamic()
        {
            ValidationState expectedValidationState = ValidationState.Unauthorized;

            string fingerprintText = "[platform=GitHub][secret=dummy]";
            Fingerprint fingerprint = new Fingerprint(fingerprintText);
            string message = null;
            Dictionary<string, string> options = new Dictionary<string, string>();

            ValidationState actualValidationState = GitHubPatValidator.IsValidDynamic(ref fingerprint, ref message, ref options);

            Assert.Equal(expectedValidationState, actualValidationState);
        }
    }
}
