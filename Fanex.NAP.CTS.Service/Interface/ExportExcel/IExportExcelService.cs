namespace Fanex.NAP.CTS.Service.Interface.ExportExcel
{
    using System.Collections.Generic;

    public interface IExportExcelService
    {
        byte[] Export(List<(string, string)> blocks, IEnumerable<object[]> rowDatas, object[] footer);
    }
}
