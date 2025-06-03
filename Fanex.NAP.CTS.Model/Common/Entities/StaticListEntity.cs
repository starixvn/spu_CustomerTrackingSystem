namespace Fanex.NAP.CTS.Model.Common.Entities
{
    public class StaticListEntity
    {
        public int ListID { get; set; }

        public int ItemID { get; set; }

        public string ListName { get; set; }

        public string ListNameDisplay { get; set; }

        public string ItemName { get; set; }

        public string ItemNameDisplay { get; set; }

        public string ItemValue { get; set; }

        public string ItemDefaultValue { get; set; }

        public int PriorityOrder { get; set; }

        public bool Status { get; set; }

        public string Description { get; set; }
    }
}
