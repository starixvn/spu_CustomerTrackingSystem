namespace Fanex.NAP.CTS.Model.GraphDB
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    [XName("A")]
    public class Association
    {
        long firstAssociationDateInMiliseconds;
        long createdDateInMiliseconds;

        public int CustId { get; set; }

        public string CreatedDate
        {
            get { return ConvertToDate(createdDateInMiliseconds).ToString(); }
        }

        [XName("AssociatedDate")]
        public string FirstAssociatedDate
        {
            get { return ConvertToDate(firstAssociationDateInMiliseconds).ToString(); }
        }

        [XmlIgnore]
        public string FirstAssociatedDateInMili
        {
            set
            {
                try
                {
                    firstAssociationDateInMiliseconds = Convert.ToInt64(value);
                }
                catch (Exception)
                {
                    firstAssociationDateInMiliseconds = (Convert.ToDateTime(value)).Ticks;
                }
            }
        }

        [XmlIgnore]
        public string CreatedDateInMili
        {
            set
            {
                try
                {
                    createdDateInMiliseconds = Convert.ToInt64(value);
                }
                catch (Exception)
                {
                    createdDateInMiliseconds = (Convert.ToDateTime(value)).Ticks;
                }
            }
        }

        private DateTime ConvertToDate(long timeSpan)
        {
            try
            {
                var converter = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return converter.AddMilliseconds(timeSpan);
            }
            catch (Exception)
            {
                var converter = DateTime.MinValue;
                return converter.AddTicks(timeSpan);
            }
        }
    }
}
