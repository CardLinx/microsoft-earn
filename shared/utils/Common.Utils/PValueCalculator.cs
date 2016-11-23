//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace Common.Utils
{
    public static class PValueCalculator
    {
        public struct WaldCIResult
        {
            public double lift;
            public double ll;
            public double ul;
            public double pval;
        }

        private const double Mean = 0; // μ, The mean of the normal distribution.
        private const double StandardDeviation = 1; // σ, Range: σ ≥ 0.

        private static double FixPValue(double pValue)
        {
            return pValue > 0.5 ? 2 * (1 - pValue) : 2 * pValue;
        }

        /**
         * Wald confidence interval for binomial proportions (similar to above) with optional agresti-coull corrections for better coverage
         */
        public static WaldCIResult WaldCI(double s1, double n1, double s2, double n2, double conf = 0.95, bool acCorrect = true)
        {
            double p1hat = 0;
            double p2hat = 0;

            if (acCorrect)
            {
                n1 += 2;
                n2 += 2;
                p1hat = (s1 + 1) / n1;
                p2hat = (s2 + 1) / n2;
            }
            else
            {
                p1hat = s1 / n1;
                p2hat = s2 / n2;
            }

            double z = Math.Abs(Normal.InvCDF(Mean, StandardDeviation, (1 - conf) / 2));
            double p1se = Math.Sqrt(p1hat * (1 - p1hat) / n1);
            double p2se = Math.Sqrt(p2hat * (1 - p2hat) / n2);

            double pdiff = p2hat - p1hat;
            double sediff = Math.Sqrt(Math.Pow(p1se, 2) + Math.Pow(p2se, 2));
            double zscore = pdiff / sediff;

            WaldCIResult result = new WaldCIResult();
            result.lift = pdiff / p1hat;
            result.ll = (pdiff - z * sediff) / p1hat;
            result.ul = (pdiff + z * sediff) / p1hat;
            result.pval = FixPValue(Normal.CDF(Mean, StandardDeviation, zscore));

            return result;
        }
    }
}