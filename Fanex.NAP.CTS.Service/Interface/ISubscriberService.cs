using Fanex.NAP.CTS.Model.API.FPS.Subscriber;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Model.Subscriber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Service.Interface
{
   public interface ISubscriberService
    {
        SubscriberInfo GetSubscriberBySubscriberName(string subscriberName);

        SubscriberFpsInfo GetSubscriberBySubscriberName_DCSExtra(string subscriberName);

    }
}
