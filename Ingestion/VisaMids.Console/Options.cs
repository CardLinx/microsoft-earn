//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using CommandLine;
using System.Text;
using CommandLine.Text;

namespace VisaMids.Console
{
    public class Options
    {
        [Option('m', "visaMethod", DefaultValue = 1, HelpText = "1 = SearchMerchantDetailsByAttribute, 2 = CreateOffer")]
        public VisaMethod VisaMethod { get; set; }

        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            /*
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
            */

            return HelpText.AutoBuild(this,
                                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));

        }
    }
}
