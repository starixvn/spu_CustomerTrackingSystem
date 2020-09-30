using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.DTO
{
    public class Role
    {
        private int _id;

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Name
        {
            get
            {
                switch (_id)
                {
                    case 1:
                        return "Member";
                    case 2:
                        return "Agent";
                    case 3:
                        return "Master";
                    case 4:
                        return "Super";
                }

                return "Others";
            }
        }
    }
}
