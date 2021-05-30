﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.Sarif.PatternMatcher.Sdk;
using Microsoft.RE2.Managed;

using Octokit;
using Octokit.Internal;

namespace Microsoft.CodeAnalysis.Sarif.PatternMatcher.Plugins.Security
{
    public class GitHubPatValidator : ValidatorBase
    {
        internal static GitHubPatValidator Instance;

        static GitHubPatValidator()
        {
            Instance = new GitHubPatValidator();
        }

        public static IEnumerable<ValidationResult> IsValidStatic(Dictionary<string, FlexMatch> groups)
        {
            return IsValidStatic(Instance, groups);
        }

        public static ValidationState IsValidDynamic(ref Fingerprint fingerprint,
                                                     ref string message,
                                                     Dictionary<string, string> options,
                                                     ref ResultLevelKind resultLevelKind)
        {
            return IsValidDynamic(Instance,
                                  ref fingerprint,
                                  ref message,
                                  options,
                                  ref resultLevelKind);
        }

        protected override IEnumerable<ValidationResult> IsValidStaticHelper(Dictionary<string, FlexMatch> groups)
        {
            if (!groups.TryGetNonEmptyValue("secret", out FlexMatch secret))
            {
                return ValidationResult.CreateNoMatch();
            }

            ValidationResult validationResult;
            if (groups.TryGetNonEmptyValue("checksum", out FlexMatch checksum))
            {
                // TODO: #365 Utilize checksum https://github.com/microsoft/sarif-pattern-matcher/issues/365

                validationResult = new ValidationResult
                {
                    RegionFlexMatch = secret,
                    Fingerprint = new Fingerprint
                    {
                        Secret = secret.Value,
                        Platform = nameof(AssetPlatform.GitHub),
                    },
                    ValidationState = ValidationState.Unknown,
                };

                return new[] { validationResult };
            }

            // It is highly likely we do not have a key if we can't
            // find at least one letter and digit within the pattern.
            if (!ContainsDigitAndChar(secret.Value))
            {
                return ValidationResult.CreateNoMatch();
            }

            string matchedPattern = groups["0"].Value;

            if (matchedPattern.IndexOf("raw", StringComparison.OrdinalIgnoreCase) >= 0 ||
                matchedPattern.IndexOf("tree", StringComparison.OrdinalIgnoreCase) >= 0 ||
                matchedPattern.IndexOf("blob", StringComparison.OrdinalIgnoreCase) >= 0 ||
                matchedPattern.IndexOf("gist", StringComparison.OrdinalIgnoreCase) >= 0 ||
                matchedPattern.IndexOf("spec", StringComparison.OrdinalIgnoreCase) >= 0 ||
                matchedPattern.IndexOf("repos", StringComparison.OrdinalIgnoreCase) >= 0 ||
                matchedPattern.IndexOf("assets", StringComparison.OrdinalIgnoreCase) >= 0 ||
                matchedPattern.IndexOf("commit", StringComparison.OrdinalIgnoreCase) >= 0 ||
                matchedPattern.IndexOf("using ref", StringComparison.OrdinalIgnoreCase) >= 0 ||
                matchedPattern.IndexOf("githubusercontent.com", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ValidationResult.CreateNoMatch();
            }

            validationResult = new ValidationResult
            {
                RegionFlexMatch = secret,
                Fingerprint = new Fingerprint
                {
                    Secret = secret.Value,
                    Platform = nameof(AssetPlatform.GitHub),
                },
                ValidationState = ValidationState.Unknown,
            };

            return new[] { validationResult };
        }

        protected override ValidationState IsValidDynamicHelper(ref Fingerprint fingerprint,
                                                                ref string message,
                                                                Dictionary<string, string> options,
                                                                ref ResultLevelKind resultLevelKind)
        {
            string pat = fingerprint.Secret;

            try
            {
                var credentials = new Credentials(pat);
                var credentialsStore = new InMemoryCredentialStore(credentials);
                var client = new GitHubClient(new ProductHeaderValue(ScanIdentityId), credentialsStore);

                User user = client.User.Current().GetAwaiter().GetResult();
                string id = user.Login;
                string name = user.Name;

                if (!string.IsNullOrEmpty(user.Name))
                {
                    name = $" ({name})";
                }

                message = $"the compromised GitHub account is '[{id}{name}](https://github.com/{id})'";

                IReadOnlyList<Organization> orgs = client.Organization.GetAllForCurrent().GetAwaiter().GetResult();
                string orgNames = string.Join(", ", orgs.Select(o => o.Login));

                if (orgNames.Length == 0)
                {
                    orgNames = "[None]";
                }

                message += $" which has access to the following orgs '{orgNames}'";

                return ValidationState.Authorized;
            }
            catch (ForbiddenException)
            {
                // The token is valid but doesn't have sufficient scope to retrieve org data.
                return ValidationState.Authorized;
            }
            catch (AuthorizationException)
            {
                // The token is either invalid or has been killed
                return ValidationState.Unauthorized;
            }
            catch (Exception e)
            {
                return ReturnUnhandledException(ref message, e);
            }
        }
    }
}
