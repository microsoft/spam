﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Strings.Interop;

namespace Microsoft.CodeAnalysis.Sarif.PatternMatcher
{
    /// <summary>
    ///  FlexMatch is a generic subset of System.Text.RegularExpressions.Match.
    /// </summary>
    public class FlexMatch
    {
        public bool Success { get; set; }

        public int Index { get; set; }

        public int Length { get; set; }

        public FlexString Value { get; set; }
    }
}
