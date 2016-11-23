//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.DataModel
{
    public abstract class ModelBase
    {
        public abstract string Id { get; set; }

        public abstract string Type { get; set; }

        public abstract string Author { get; set; }
    }
}