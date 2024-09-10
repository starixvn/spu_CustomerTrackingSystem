namespace Fanex.NAP.CTS.Model.Account
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System;

    public class CustomerClassificationHistoryModel
    {        
        public DateTime CreatedTime { get; set; }

        public string CategoryName { get; set; }

        public string CustomerClass { get; set; }      

        public string SourceTypeName { get; set; }

        public int SourceTypeID { get; set; }

        public string CreatedBy { get; set; }
  
        public string Remark { get; set; }
          
        public int ? SportType { get; set; }     

        public string SportName { 
            get 
            {
                if (this.SportType != null)
                {
                    return CachedRef.GetSportTypeName(this.SportType.Value);
                }

                return string.Empty;
            } 
        }

        public bool IsGetSportName { get; set; }
    }
}
