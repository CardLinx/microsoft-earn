//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                PrintUsage();
                return;
            }
            if (args[0] == "-delete" || args[0] == "-d")
            {
                UserUpdater userUpdater = new UserUpdater();
                switch (args[1])
                {
                    case "-email":
                    case "-e":
                        userUpdater.DeleteUserByEmail(args[2]);
                        break;
                    case "-ms_id":
                    case "-m":
                        userUpdater.DeleteUserByMsId(args[2]);
                        break;
                    default:
                        PrintUsage();
                        break;
                }
            }
            else
            {
                PrintUsage();
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("UserUpdater -delete -e <email address>");
            Console.WriteLine("UserUpdater -delete -m <ms id>");
        }
    }
}