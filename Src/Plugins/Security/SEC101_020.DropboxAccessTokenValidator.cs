﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.CodeAnalysis.Sarif.PatternMatcher.Plugins.Security.Utilities;
using Microsoft.CodeAnalysis.Sarif.PatternMatcher.Sdk;

namespace Microsoft.CodeAnalysis.Sarif.PatternMatcher.Plugins.Security
{
    public class DropboxAccessTokenValidator : ValidatorBase
    {
        internal static DropboxAccessTokenValidator Instance;

        static DropboxAccessTokenValidator()
        {
            Instance = new DropboxAccessTokenValidator();
        }

        public static ValidationState IsValidStatic(ref string matchedPattern,
                                                    ref Dictionary<string, string> groups,
                                                    ref string message,
                                                    out ResultLevelKind resultLevelKind,
                                                    out Fingerprint fingerprint)
        {
            return IsValidStatic(Instance,
                                 ref matchedPattern,
                                 ref groups,
                                 ref message,
                                 out resultLevelKind,
                                 out fingerprint);
        }

        public static ValidationState IsValidDynamic(ref Fingerprint fingerprint,
                                                     ref string message,
                                                     ref Dictionary<string, string> options,
                                                     out ResultLevelKind resultLevelKind)
        {
            return IsValidDynamic(Instance,
                                  ref fingerprint,
                                  ref message,
                                  ref options,
                                  out resultLevelKind);
        }

        protected override ValidationState IsValidStaticHelper(ref string matchedPattern,
                                                               ref Dictionary<string, string> groups,
                                                               ref string message,
                                                               out ResultLevelKind resultLevelKind,
                                                               out Fingerprint fingerprint)
        {
            fingerprint = default;
            resultLevelKind = default;

            if (!groups.TryGetNonEmptyValue("refine", out string secret))
            {
                return ValidationState.NoMatch;
            }

            if (!ContainsDigitAndChar(secret))
            {
                return ValidationState.NoMatch;
            }

            fingerprint = new Fingerprint()
            {
                Secret = secret,
                Platform = nameof(AssetPlatform.Dropbox),
            };

            return ValidationState.Unknown;
        }

        protected override ValidationState IsValidDynamicHelper(ref Fingerprint fingerprint,
                                                                ref string message,
                                                                ref Dictionary<string, string> options,
                                                                out ResultLevelKind resultLevelKind)
        {
            resultLevelKind = new ResultLevelKind
            {
                Level = FailureLevel.Note,
            };

            const string NoAccessMessage = "Your app is not permitted to access this endpoint";
            const string DisabledMessage = "This app is currently disabled.";

            string secret = fingerprint.Secret;
            using HttpClient httpClient = CreateHttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secret);

            try
            {
                using HttpResponseMessage response = httpClient
                    .PostAsync("https://api.dropboxapi.com/2/file_requests/count", null)
                    .GetAwaiter()
                    .GetResult();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    {
                        resultLevelKind.Level = FailureLevel.Error;
                        return ValidationState.AuthorizedError;
                    }

                    case HttpStatusCode.BadRequest:
                    {
                        string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        // App deleted.
                        if (body.EndsWith(DisabledMessage))
                        {
                            return ValidationState.Expired;
                        }

                        // Request was successful but AccessToken does not have access.
                        if (body.Contains(NoAccessMessage))
                        {
                            if (secret.Length == 64)
                            {
                                resultLevelKind.Level = FailureLevel.Error;

                                // No expiration token.
                                return ValidationState.AuthorizedError;
                            }
                            else
                            {
                                resultLevelKind.Level = FailureLevel.Warning;

                                // Short expiration token (4h).
                                return ValidationState.AuthorizedWarning;
                            }
                        }

                        // We don't recognize this message.
                        message = CreateUnexpectedResponseCodeMessage(response.StatusCode);
                        return ValidationState.Unknown;
                    }

                    case HttpStatusCode.Unauthorized:
                    {
                        return ValidationState.Unauthorized;
                    }

                    default:
                    {
                        message = CreateUnexpectedResponseCodeMessage(response.StatusCode);
                        return ValidationState.Unknown;
                    }
                }
            }
            catch (Exception e)
            {
                return ReturnUnhandledException(ref message, e);
            }
        }
    }
}
