//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // 
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace Common.Utils
{
    using System;

    /// <summary>
    /// Thread-safe, mostly lock-free random number generator.
    /// </summary>
    public class ThreadSafeRandomNumberGenerator
    {
        /// <summary>
        /// Instance of Random class
        /// </summary>
        private static readonly Random Global = new Random();
        
        /// <summary>
        /// Storing random number
        /// </summary>
        [ThreadStatic]
        private static Random local;

        private static Random GetLocalRandom()
        {
            if (local == null)
            {
                // Initialize the local random variable for this thread.
                int seed;
                lock (Global)
                {
                    seed = Global.Next();
                }

                local = new Random(seed);
            }

            return local;
        }

        /// <summary>
        /// Thread-safe, mostly lock-free random number generator.
        /// </summary>
        /// <param name="maxValue">maxValue of random number</param>
        /// <returns>random number</returns>
        public static int Next(int maxValue)
        {
            Random inst = GetLocalRandom();

            return inst.Next(maxValue);
        }

        /// <summary>
        /// Thread-safe, mostly lock-free random number generator.
        /// </summary>
        /// <returns>random number</returns>
        public static int Next()
        {
            return Next(int.MaxValue);
        }

        /// <summary>
        /// Thread-safe, mostly lock-free random number generator.
        /// </summary>
        /// <returns>random number</returns>
        public static double NextDouble()
        {
            Random inst = GetLocalRandom();

            return inst.NextDouble();
        }

    }
}