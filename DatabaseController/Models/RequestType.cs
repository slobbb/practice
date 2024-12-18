using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseController.Models
{
    public class RequestType
    {
        public int id;
        public string name;
        public RequestType(int id, string name) { 
            this.id = id;
            this.name = name;
        }    
    }
}
