using Fanex.NAP.CTS.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor
{
    public class DetailGroup
    {
        public int? GroupId { get; set; }

        private string _GroupName;
        public string GroupName {
            get
            {
                return _GroupName;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _GroupName = ConstsResources.NULL;
                }
                else
                {
                    _GroupName = value;
                }
            }
        }

    }
}