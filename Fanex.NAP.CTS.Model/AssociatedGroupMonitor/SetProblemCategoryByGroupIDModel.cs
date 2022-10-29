namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{
    public class SetProblemCategoryByGroupIDModel
    {    
        public long GroupID { get; set; }
    
        public int PACategoryID { get; set; }
       
        public int PAAllCredit { get; set; }
     
        public int PAAllLicensee { get; set; }
     
        public string PASites { get; set; }
    
        public int CreatedBy { get; set; }

    }
}
