namespace Fanex.NAP.CTS.Model.Common
{
    using Fanex.NAP.CTS.Model.Common.Entities;

    public class StaticListModel
    {
        public StaticListModel() { }

        public StaticListModel(StaticListEntity model)
        {
            ListID = model.ListID;
            ItemID = model.ItemID;
            ListName = model.ListName;
            ListNameDisplay = model.ListNameDisplay;
            ItemName = model.ItemName;
            ItemNameDisplay = model.ItemNameDisplay;
            ItemValue = model.ItemValue;
            ItemDefaultValue = model.ItemDefaultValue;
            PriorityOrder = model.PriorityOrder;
            Status = model.Status ? 1 : 0;
            Description = model.Description;
        }

        public int ListID { get; set; }

        public int ItemID { get; set; }

        public string ListName { get; set; }

        public string ListNameDisplay { get; set; }

        public string ItemName { get; set; }

        public string ItemNameDisplay { get; set; }

        public string ItemValue { get; set; }

        public string ItemDefaultValue { get; set; }

        public int PriorityOrder { get; set; }

        public int Status { get; set; }

        public string Description { get; set; }
    }
}
