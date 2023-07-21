﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Microsoft.RE2.Managed;

using Sarif.PatternMatcher.Benchmark;

namespace Microsoft.CodeAnalysis.Sarif.PatternMatcher.Benchmark.Benchmarks
{
    //BenchmarkDotNet=v0.12.1, OS=Windows 10.0.22621
    //Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
    //  [Host]     : .NET Framework 4.8 (4.8.9166.0), X64 RyuJIT
    //  DefaultJob : .NET Framework 4.8 (4.8.9166.0), X64 RyuJIT

    //|         Method | FindAllMatches | UseSyntheticContent |    Mean |    Error |   StdDev |
    //|--------------- |--------------- |-------------------- |--------:|---------:|---------:|
    //|        IndexOf |          False |               False | 1.973 s | 0.0389 s | 0.0681 s |
    //| RE2RegexEngine |          False |               False | 1.499 s | 0.0293 s | 0.0490 s |

    public class SniffFullBenchmark : SniffBenchmark
    {
        public SniffFullBenchmark() : this(null)
        {
        }

        public SniffFullBenchmark(SniffFullOptions options) : base(options)
        {
        }

        protected override void InitializeSignatures()
        {
            Signatures = new[] {
                "SharedAccessKeyName",
                "EndpointSuffix",
                "DefaultEndpointsProtocol",
                "initial catalog",
                "Initial Catalog",
                "INITIAL CATALOG",
                "database",
                "Database",
                "DATABASE",
                "dbname",
                "DbName",
                "DBNAME",
                "AzCa",
                "ACDb",
                "AzSe",
                "AzFu",
                "8Q~",
                "7Q~",
                "ghp_",
                "npm_",
                "dapi",
                "AKIA",
                "LTAI",
                "ASIA",
                "GOOG",
                "aio_",
                "dvc_",
                "pat-",
                "ion_",
                "EZAK",
                "EZTK",
                "AKID",
                "tfp_",
                "FLWSECK-",
                "eyJrIjoi",
                "dG9rO",
                "lin_api_",
                "lin_oauth_",
                "Mid-server-",
                "NRIQ-",
                "NRAK-",
                "NRRA-",
                "_live",
                "pscale_",
                "amzn1",
                "jfrog",
                "shpat_",
                "shpss_",
                "PMAK-",
                "CLOJARS_",
                "duffel_",
                "rdme_xn8s9h",
                "rpa_",
                "samsara_api_",
                "gzUdQrDW",
                "secret_scanning",
                "xkeysib-",
                "xsmtpsib-",
                "shippo_",
                "xapp-",
                "sk_live_",
                "sys_live_",
                "zpka_",
                "_test_",
                "waka_",
                "gcntfy-",
                "persona_",

                "+ASt",
                "/AM7",
                "+ASb",
                "+AEh",
                "+ABa",
                "+AMC",
                "+ARm",
                "+ACR",
                ".office.com/webhook",
                ".core.windows.net",
                ".core.chinacloudapi.cn",
                ".core.cloudapi",
                ".azurewebsites",
                ".azureedge.net",
                ".msecnd.net",
                ".documents.azure.com",
                ".logic.azure.com",
                "hvs.",
                "dp.audit",
                "dp.pt",
                "dp.scim",
                "b.AAAAAQ",
                "hvb.AA",

                ".v1",
                "sk-",
                "SG.",
                "sk_",
                "PSK",

                "figd_",
                "figo_",
                "figu_",
                "figdr_",
                "figdh_",
                "figor_",
                "figoh_",
                "figur_",
                "figuh_",
                "pcs_",
                "pcu_",
                "wfa_",
                "wfb_",
                "wfc_",
                "wfd_",
                "wfe_",
                "wff_",
                "wfg_",
                "wfh_",
                "wfi_",
                "wfj_",
                "wfk_",
                "wfl_",
                "wfm_",
                "wfn_",
                "wfo_",
                "wfp_",
                "wfq_",
                "wfr_",
                "wfs_",
                "wft_",
                "wfu_",
                "wfv_",
                "wfw_",
                "wfx_",
                "wfy_",
                "wfz_",
                "y0-6_",
                "dp.ct",
                "dp.st",
                "CiQ",
                "CiR",
                "lma_",
                "lmb_",
                "oy2a",
                "oy2b",
                "oy2c",
                "oy2d",
                "oy2e",
                "oy2f",
                "oy2g",
                "oy2h",
                "oy2i",
                "oy2j",
                "oy2k",
                "oy2l",
                "oy2m",
                "oy2n",
                "oy2o",
                "oy2p",
                "xoxp",
                "xoxb",
                "xoxa",
                "xoxo",
                "xoxr",
                "xoxs",
                "gh1_",
                "ghp_",
                "gho_",
                "ghr_",
                "ghs_",
                "ghu_",
                "doo_v",
                "dop_v",
                "dor_v",
                "dos_v",

                // "[2-7a-z]{52}",
            };

            RegexSignatures = new[] {
                "SharedAccessKeyName",
                "EndpointSuffix",
                "DefaultEndpointsProtocol",
                "initial catalog",
                "Initial Catalog",
                "INITIAL CATALOG",
                "database",
                "Database",
                "DATABASE",
                "dbname",
                "DbName",
                "DBNAME",
                "AzCa",
                "ACDb",
                "AzSe",
                "AzFu",
                "8Q~",
                "7Q~",
                "ghp_",
                "npm_",
                "dapi",
                "AKIA",
                "LTAI",
                "ASIA",
                "GOOG",
                "aio_",
                "dvc_",
                "pat-",
                "ion_",
                "EZAK",
                "EZTK",
                "AKID",
                "tfp_",
                "FLWSECK-",
                "eyJrIjoi",
                "dG9rO",
                "lin_api_",
                "lin_oauth_",
                "Mid-server-",
                "NRIQ-",
                "NRAK-",
                "NRRA-",
                "_live",
                "pscale_",
                "amzn1",
                "jfrog",
                "shpat_",
                "shpss_",
                "PMAK-",
                "CLOJARS_",
                "duffel_",
                "rdme_xn8s9h",
                "rpa_",
                "samsara_api_",
                "gzUdQrDW",
                "secret_scanning",
                "xkeysib-",
                "xsmtpsib-",
                "shippo_",
                "xapp-",
                "sk_live_",
                "sys_live_",
                "zpka_",
                "_test_",
                "waka_",
                "gcntfy-",
                "persona_",

                "\\+ASt",
                "\\/AM7",
                "\\+ASb",
                "\\+AEh",
                "\\+ABa",
                "\\+AMC",
                "\\+ARm",
                "\\+ACR",
                "\\.office\\.com\\/webhook",
                "\\.core\\.windows\\.net",
                "\\.core\\.chinacloudapi\\.cn",
                "\\.core\\.cloudapi",
                "\\.azurewebsites",
                "\\.azureedge\\.net",
                "\\.msecnd\\.net",
                "\\.documents\\.azure\\.com",
                ".logic\\.azure\\.com",
                "hvs\\.",
                "dp\\.audit",
                "dp\\.pt",
                "dp\\.scim",
                "b\\.AAAAAQ",
                "hvb\\.AA",

                "\\b\\.v1",
                "\\bsk-",
                "\\bSG\\.",
                "\\bsk_",
                "\\bPSK",

                "fig[dou][rh]*_",
                "pc[su]_",
                "wf[a-z]_",
                "y[0-6]_",
                "dp\\.[cs]t",
                "\\bCi[QR]",
                "lm[ab]_",
                "oy2[a-p]",
                "xox[pbaors]",
                "gh[1porsu]_",
                "do[oprs]_v",

                "[2-7a-z]{52}",
            };
        }
    }
}
