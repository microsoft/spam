﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.CodeAnalysis.Sarif.PatternMatcher.Sdk;

using Moq;
using Moq.Protected;

using Xunit;

namespace Microsoft.CodeAnalysis.Sarif.PatternMatcher.Plugins.Security.Validators
{
    public class SquareCredentialsValidatorTests
    {
        [Fact]
        public void SquareCredentialsValidator_Test()
        {
            string fingerprintText = "";
            if (string.IsNullOrEmpty(fingerprintText))
            {
                return;
            }

            string message = null;
            ResultLevelKind resultLevelKind = default;
            var fingerprint = new Fingerprint(fingerprintText);
            var keyValuePairs = new Dictionary<string, string>();

            SquareCredentialsValidator.IsValidDynamic(ref fingerprint,
                                                      ref message,
                                                      keyValuePairs,
                                                      ref resultLevelKind);
        }

        [Fact]
        public void SquareCredentialsValidator_TestingMock()
        {
            var testCases = new[]
            {
                new
                {
                    Title = "Testing unexpected OK StatusCode",
                    HttpStatusCode = HttpStatusCode.OK,
                    HttpContent = (HttpContent)null,
                    ExpectedValidationState = ValidationState.Unknown,
                    ExpectedMessage = "An unexpected HTTP response code was received: 'OK'."
                }
            };

            const string fingerprintText = "[id=a][secret=b]";

            var sb = new StringBuilder();
            foreach (var testCase in testCases)
            {
                string message = null;
                ResultLevelKind resultLevelKind = default;
                var fingerprint = new Fingerprint(fingerprintText);
                var keyValuePairs = new Dictionary<string, string>();

                SquareCredentialsValidator.SetHttpClient(new HttpClient(MockHttpMessageHandler(HttpStatusCode.OK, null)));

                ValidationState currentState = SquareCredentialsValidator.IsValidDynamic(ref fingerprint,
                                                                                         ref message,
                                                                                         keyValuePairs,
                                                                                         ref resultLevelKind);
                if (currentState != testCase.ExpectedValidationState)
                {
                    sb.AppendLine($"The test case '{testCase.Title}' was expecting '{testCase.ExpectedValidationState}' but found '{currentState}'.");
                }

                if (message != testCase.ExpectedMessage)
                {
                    sb.AppendLine($"The test case '{testCase.Title}' was expecting '{testCase.ExpectedValidationState}' but found '{currentState}'.");
                }
            }

            sb.Length.Should().Be(0, sb.ToString());
        }

        private HttpMessageHandler MockHttpMessageHandler(HttpStatusCode httpStatusCode, HttpContent httpContent)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = httpStatusCode,
                    Content = httpContent
                });

            return mockMessageHandler.Object;
        }
    }
}
