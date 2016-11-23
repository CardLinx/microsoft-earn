//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Earn.Offers.Earn.Models
{
    public class MerchantModel
    {
        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [DisplayName("Legal Company Name")]
        [DataType(DataType.Text)]
        public string LegalCompanyName { get; set; }

        [StringLength(150)]
        [DisplayName("DBA Company Name")]
        [DataType(DataType.Text)]
        public string DbaCompanyName { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [DisplayName("Contact Name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [DataType(DataType.Text)]
        [DisplayName("Contact Title/Position")]
        public string Title { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [DataType(DataType.Text)]
        [DisplayName("Contact Address, City, State and Zip code")]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [DisplayName("Contact Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(15)]
        [DisplayName("Contact Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string PhoneNumber { get; set; }

        public bool Terms { get; set; }

        public DateTime SignedOn { get; set; }
    }
}