using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class APIResponse<T>
    {
        public string message { get; set; }
        public string status { get; set; }
        public T data { get; set; }
    }
}
