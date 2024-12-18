using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseController
{
    public class TechType
    {
        public int techTypeID;
        public string techType;
        public TechType(int techTypeID, string techType) {
            this.techType = techType;
            this.techTypeID = techTypeID;
        }   
    }
}
