namespace Fanex.NAP.CTS.Core.ExportExcel
{
    using System.IO;

    internal class ExcelStream : MemoryStream
    {
        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
            {
                base.Close();
            }
        }
    }
}