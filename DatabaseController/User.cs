using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseController
{
    public class User
    {
        public int id;
        public string name;
        public int userTypes;
        public User(int id, string name, int userTypes) {
            this.id = id;
            this.name = name;
            this.userTypes = userTypes;
        }
    }
}
