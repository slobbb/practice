using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseController
{
    public class Model
    {
        public int techModelID;
        public int techTypeID;
        public string techModel;
        public Model(int techModelID, int techTypeID, string techModel) {
            this.techModelID = techModelID;
            this.techTypeID = techTypeID;
            this.techModel = techModel;
        }
    }
}
