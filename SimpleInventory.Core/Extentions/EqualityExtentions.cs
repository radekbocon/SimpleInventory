using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Extentions
{
    public static class EqualityExtentions
    {
        public static bool IsEqualTo(this CustomerModel obj, CustomerModel another)
        {
            if (obj == null || another == null)
                return false;

            var isCustomerEqual = obj.Email == another.Email &&
                obj.FirstName == another.FirstName &&
                obj.LastName == another.LastName &&
                obj.Id == another.Id &&
                obj.CompanyName == another.CompanyName &&
                obj.PhoneNumber == another.PhoneNumber;

            var areAddressesEqual = true;
            if (obj.Addresses.Count != another.Addresses.Count) 
                return false;

            for (int i = 0; i < obj.Addresses.Count; i++)
            {
                if (!obj.Addresses[i].IsEqualTo(another.Addresses[i]))
                {
                    areAddressesEqual = false;
                    break;
                }
            }

            return isCustomerEqual && areAddressesEqual;
        }

        public static bool IsEqualTo(this AddressModel obj, AddressModel another)
        {
            if (obj == null || another == null) return false;

            return obj.Line1 == another.Line1 &&
                obj.Line2 == another.Line2 &&
                obj.PhoneNumber == another.PhoneNumber &&
                obj.PostCode == another.PostCode &&
                obj.City == another.City &&
                obj.Country == another.Country;
        }

        public static bool IsEqualTo(this ItemModel obj, ItemModel another)
        {
            if (obj == null || another == null) return false;

            return obj.ProductId == another.ProductId &&
                obj.Name == another.Name &&
                obj.Description == another.Description &&
                obj.Type == another.Type &&
                obj.Price == another.Price;
        }
    }
}
