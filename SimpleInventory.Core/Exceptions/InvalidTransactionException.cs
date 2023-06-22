using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Exceptions
{
    public class InvalidTransactionException : Exception
    {
        public InvalidTransactionException() { }

        public InvalidTransactionException(string message)
            : base(message)
        {

        }
    }
}
