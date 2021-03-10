﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

// From the Nuget package
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;

// From the DLL
using Aliyun.Acs.Iot.Model.V20170420;

using Microsoft.CodeAnalysis.Sarif.PatternMatcher.Plugins.Security.Utilities;

namespace Microsoft.CodeAnalysis.Sarif.PatternMatcher.Plugins.Security
{
    public class AlibabaAccessKeyValidator : ValidatorBase
    {
        internal static AlibabaAccessKeyValidator Instance;

        private const string _keyNotFound = "InvalidAccessKeyId.NotFound";
        private const string _invalidSecret = "SDK.InvalidAccessKeySecret";

        static AlibabaAccessKeyValidator()
        {
            Instance = new AlibabaAccessKeyValidator();
        }

        public static string IsValidStatic(ref string matchedPattern,
                                           ref Dictionary<string, string> groups,
                                           ref string failureLevel,
                                           ref string fingerprint,
                                           ref string message)
        {
            return IsValidStatic(Instance,
                                 ref matchedPattern,
                                 ref groups,
                                 ref failureLevel,
                                 ref fingerprint,
                                 ref message);
        }

        public static string IsValidDynamic(ref string fingerprint, ref string message)
        {
            return IsValidDynamic(Instance,
                                   ref fingerprint,
                                   ref message);
        }

        protected override string IsValidStaticHelper(ref string matchedPattern,
                                                      ref Dictionary<string, string> groups,
                                                      ref string failureLevel,
                                                      ref string fingerprintText,
                                                      ref string message)
        {
            if (!groups.TryGetNonEmptyValue("account", out string account) ||
                !groups.TryGetNonEmptyValue("password", out string password))
            {
                return nameof(ValidationState.NoMatch);
            }

            fingerprintText = new Fingerprint()
            {
                Account = account,
                Password = password,
            }.ToString();

            return nameof(ValidationState.AuthorizedWarning);
        }

        protected override string IsValidDynamicHelper(ref string fingerprintText,
                                                       ref string message)
        {
            var fingerprint = new Fingerprint(fingerprintText);
            string account = fingerprint.Account;
            string password = fingerprint.Password;

            try
            {
                // Taken from https://www.alibabacloud.com/help/doc-detail/63638.htm
                IClientProfile clientProfile = DefaultProfile.GetProfile("cn-shanghai", account, password);
                DefaultAcsClient client = new DefaultAcsClient(clientProfile);

                PubRequest request = new PubRequest();

                // We don't actually care about any of the product stuff, so just leave dummy values in
                request.ProductKey = "<productKey>";
                request.TopicFullName = "/<productKey>/<deviceName>/get";
                byte[] payload = Encoding.Default.GetBytes("Hello World.");
                string payloadStr = Convert.ToBase64String(payload);
                request.MessageContent = payloadStr;
                request.Qos = 0;
                PubResponse response = client.GetAcsResponse(request);

                // If all goes well, we'll receive a "product format invalid" message in the response
                // which means authentication succeeded. Therefore the id and secret are valid.
                return ReturnAuthorizedAccess(ref message, asset: nameof(AssetPlatform.AlibabaCloud));
            }
            catch (ClientException ce)
            {
                switch (ce.ErrorCode)
                {
                    case _keyNotFound:
                        // Not even the client id we found is valid. Return no match.
                        return nameof(ValidationState.NoMatch);
                    case _invalidSecret:
                        return nameof(ValidationState.Unknown);
                    default:
                        return ReturnUnhandledException(ref message, ce, asset: nameof(AssetPlatform.AlibabaCloud));
                }
            }
        }
    }
}
