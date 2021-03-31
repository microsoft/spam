﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using Microsoft.CodeAnalysis.Sarif.PatternMatcher.Plugins.Security.Utilities;
using Microsoft.CodeAnalysis.Sarif.PatternMatcher.Sdk;

namespace Microsoft.CodeAnalysis.Sarif.PatternMatcher.Plugins.Security.Internal
{
    public class CloudantConnectionStringValidator : ValidatorBase
    {
        internal static CloudantConnectionStringValidator Instance;

        static CloudantConnectionStringValidator()
        {
            Instance = new CloudantConnectionStringValidator();
        }

        public static ValidationState IsValidStatic(ref string matchedPattern,
                                           ref Dictionary<string, string> groups,
                                           ref string failureLevel,
                                           ref string message,
                                           out Fingerprint fingerprint)
        {
            return IsValidStatic(Instance,
                                 ref matchedPattern,
                                 ref groups,
                                 ref failureLevel,
                                 ref message,
                                 out fingerprint);
        }

        public static ValidationState IsValidDynamic(ref Fingerprint fingerprint, ref string message, ref Dictionary<string, string> options)
        {
            return IsValidDynamic(Instance,
                                  ref fingerprint,
                                  ref message,
                                  ref options);
        }

        protected override ValidationState IsValidStaticHelper(ref string matchedPattern,
                                                      ref Dictionary<string, string> groups,
                                                      ref string failureLevel,
                                                      ref string message,
                                                      out Fingerprint fingerprint)
        {
            fingerprint = default;

            // We need uri and neither account nor password, or uri and both account and password.  Use XOR
            if (!groups.TryGetNonEmptyValue("uri", out string uri) ||
                (groups.TryGetNonEmptyValue("account", out string account) ^
                groups.TryGetNonEmptyValue("secret", out string secret)))
            {
                return ValidationState.NoMatch;
            }

            fingerprint = new Fingerprint()
            {
                Uri = uri,
                Account = account,
                Secret = secret,
                Platform = nameof(AssetPlatform.Cloudant),
            };

            return ValidationState.Unknown;
        }

        protected override ValidationState IsValidDynamicHelper(ref Fingerprint fingerprint,
                                                       ref string message,
                                                       ref Dictionary<string, string> options)
        {
            // TODO: Create a unit test for this. https://github.com/microsoft/sarif-pattern-matcher/issues/258

            string uri = fingerprint.Uri;
            string account = fingerprint.Account;
            string password = fingerprint.Secret;

            try
            {
                // At this point account and password must be either both full or both empty.  Only check one
                if (string.IsNullOrWhiteSpace(account))
                {
                    using (HttpClient client = CreateHttpClient())
                    using (HttpResponseMessage response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return ReturnAuthorizedAccess(ref message, uri);
                        }

                        message = CreateUnexpectedResponseCodeMessage(response.StatusCode, uri);
                    }
                }
                else
                {
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.Credentials = new NetworkCredential(account, password);
                    using (HttpClient client = new HttpClient(handler)
                    {
                        BaseAddress = new Uri(uri),
                    })
                    using (HttpResponseMessage response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return ReturnAuthorizedAccess(ref message, uri);
                        }

                        message = CreateUnexpectedResponseCodeMessage(response.StatusCode, uri);
                    }
                }
            }
            catch (Exception e)
            {
                return ReturnUnhandledException(ref message, e, asset: uri);
            }

            return ValidationState.Unknown;
        }
    }
}
