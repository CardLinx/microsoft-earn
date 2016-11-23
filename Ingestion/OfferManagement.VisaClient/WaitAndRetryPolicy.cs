//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Polly;
using System;

namespace OfferManagement.VisaClient
{
    public class WaitAndRetryPolicy : IRetryPolicy
    {
        public static readonly IRetryPolicy Instance = new WaitAndRetryPolicy();

        public Policy Get()
        {
            var policy = Policy.Handle<Exception>().WaitAndRetryAsync(
                          new[]
                          {
                            TimeSpan.FromSeconds(20),
                            TimeSpan.FromSeconds(60),
                            TimeSpan.FromSeconds(120)
                          });

            return policy;
        }
    }


    public class RetryPolicy : IRetryPolicy
    {
        public static readonly IRetryPolicy Instance = new RetryPolicy();

        public Policy Get()
        {
            var policy = Policy.Handle<Exception>().RetryAsync(1);
            return policy;
        }
    }

    //Hacky way to implement NoRtry policy. It just handle DivideByZeroException exception
    public class NoRetryPolicy : IRetryPolicy
    {
        public static readonly IRetryPolicy Instance = new NoRetryPolicy();

        public Policy Get()
        {
            var policy = Policy.Handle<DivideByZeroException>().RetryAsync(1);
            return policy;
        }
    }
}