namespace Fanex.NAP.CTS.Service.ExcelConfig.FraudCase.ScannedOfficeIP
{
    using Fanex.NAP.CTS.Core.ExportExcel;
    using Fanex.NAP.CTS.Model.FraudCase.ScannedOfficeIP;
    using NPOI.SS.UserModel;

    public class OfficeIPInternalAccountExcelConfig : FSExcelConfig<object, OfficeIPInternalAccount, object>
    {
        protected override int SheetIndex => 0;

        protected override string SheetName => null;

        protected override void FillData(IRow row, OfficeIPInternalAccount data)
        {
            row.GetCell(0).SetCellValue(row.RowNum - 2);
            row.GetCell(1).SetCellValue(data.UserName);
            row.GetCell(2).SetCellValue(data.Level);
            row.GetCell(3).SetCellValue(data.Currency);
        }

        protected override void FillLegend(IRow row, object legend)
        {
            /// no legend
        }
    }
}
