﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.Sarif.PatternMatcher.Plugins.Security.Utilities
{
    public static class FilteringHelpers
    {
        public static readonly HashSet<string> LocalhostList = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "localhost",
            "(local)",
            "127.0.0.1",
            ".",
        };

        public static string StandardizeLocalhostName(string host)
        {
            return LocalhostList.Contains(host)
                ? "localhost"
                : host;
        }

        public static bool LikelyPowershellVariable(string input)
        {
            if (input.Length < 4)
            {
                // Not enough space for a variable name in the string
                return false;
            }

            if (input[0] != '$')
            {
                return false;
            }

            if (input[1] != '(')
            {
                return false;
            }

            if (input[input.Length - 1] != ')')
            {
                return false;
            }

            return true;
        }

        public static bool PasswordIsInCommonVariableContext(string secret)
        {
            var passwordContextList = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("{", "}"),
                new Tuple<string, string>("$(", ")"),
            };

            foreach (Tuple<string, string> tuplePair in passwordContextList)
            {
                if (secret.StartsWith(tuplePair.Item1))
                {
                    return secret.EndsWith(tuplePair.Item2);
                }
            }

            return false;
        }
    }
}
