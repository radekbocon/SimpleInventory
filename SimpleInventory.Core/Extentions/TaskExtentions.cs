using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Extentions
{
    public static class TaskExtentions
    {
        public static async void Await(this Task task)
        {
            await task;
        }
    }
}
