using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Models
{
    public class AddressModel
    {
        public string? FullName { get; set; }
        public string? Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public AddressModel()
        {

        }

        public AddressModel(AddressModel address)
        {
            FullName = address.FullName;
            Line1 = address.Line1;
            Line2 = address.Line2;
            PhoneNumber = address.PhoneNumber;
            PostCode = address.PostCode;
            City = address.City;
            Country = address.Country;
        }
    }
}
