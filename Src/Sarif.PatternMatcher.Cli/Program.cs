﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using CommandLine;

using Microsoft.CodeAnalysis.Sarif.Driver;

namespace Microsoft.CodeAnalysis.Sarif.PatternMatcher.Cli
{
    internal static class Program
    {
        [ThreadStatic]
        internal static IFileSystem FileSystem;

        [ThreadStatic]
        internal static Exception RuntimeException;

        [ThreadStatic]
        internal static AnalyzeCommand InstantiatedAnalyzeCommand;

        internal static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                args = EntryPointUtilities.GenerateArguments(args, FileSystem ?? new FileSystem(), new EnvironmentVariables());
            }
            catch (Exception ex)
            {
                RuntimeException = ex;
                Console.WriteLine(ex.ToString());
                return CommandBase.FAILURE;
            }

            bool isValidHelpCommand =
                args.Length > 0 &&
                args[0] == "help" &&
                ((args.Length == 2 && IsValidVerbName(args[1])) || args.Length == 1);

            bool isVersionCommand = args[0] == "version" && args.Length == 1;

            return Parser.Default.ParseArguments<
                AnalyzeOptions,
                AnalyzeDatabaseOptions,
                ExportRulesMetatadaOptions,
                ExportSearchDefinitionsOptions,
                ImportAndAnalyzeOptions,
                ValidateOptions>(args)
              .MapResult(
                (AnalyzeDatabaseOptions options) => new AnalyzeDatabaseCommand().Run(options),
                (ImportAndAnalyzeOptions options) => new ImportAndAnalyzeCommand().Run(options),
                (AnalyzeOptions options) => RunAnalyzeCommand(options),
                (ExportRulesMetatadaOptions options) => new ExportRulesMetatadaCommand().Run(options),
                (ExportSearchDefinitionsOptions options) => new ExportSearchDefinitionsCommand().Run(options),
                (ValidateOptions options) => new ValidateCommand().Run(options),
                _ => isValidHelpCommand || isVersionCommand
                        ? CommandBase.SUCCESS
                        : CommandBase.FAILURE);
        }

        internal static void ClearUnitTestData()
        {
            FileSystem = null;
            RuntimeException = null;
            InstantiatedAnalyzeCommand = null;
        }

        internal static int RunAnalyzeCommand(AnalyzeOptions options)
        {
            InstantiatedAnalyzeCommand = new AnalyzeCommand(fileSystem: FileSystem);
            return InstantiatedAnalyzeCommand.Run(options);
        }

        private static bool IsValidVerbName(string verb)
        {
            return
                verb == "analyze" ||
                verb == "analyze-database" ||
                verb == "export-rules" ||
                verb == "export-search-definitions" ||
                verb == "import-analyze";
        }
    }
}
