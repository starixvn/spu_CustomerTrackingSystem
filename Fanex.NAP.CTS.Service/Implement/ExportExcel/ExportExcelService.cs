namespace Fanex.NAP.CTS.Service.Implement.ExportExcel
{
    using Fanex.NAP.TGRS.Core.ExcelUtility.RichTextExtensions;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using NPOI.SS.UserModel;
    using System.Collections.Generic;
    using System;
    using Fanex.NAP.CTS.Service.Interface.ExportExcel;

    public class ExportExcelService : IExportExcelService
    {
        private readonly string _fileName;
        private readonly string _templatePath;
        private readonly string _sheetName;
        protected int _lineCountFilterBlock;

        public ExportExcelService() { }

        public ExportExcelService(string fileName, string templatePath, string sheetName)
        {
            _fileName = fileName;
            _templatePath = templatePath;
            _sheetName = sheetName;
        }

        public virtual byte[] Export(List<(string, string)> blocks, IEnumerable<object[]> rowDatas, object[] footer)
        {
            var writer = new ExcelTemplateWriter(_templatePath, _fileName, _sheetName);
            writer.CopyRows(0, 1);
            FilterBlock(ref writer, blocks);
            FillHeader(ref writer);
            FillRowData(rowDatas, ref writer);
            FillFooter(footer, ref writer);

            return writer.ToBytes();
        }

        protected virtual void FilterBlock(ref ExcelTemplateWriter writer, List<(string, string)> items)
        {
            RichTextRender contentRender = new RichTextRender().Clear();
            IFont boldFont = writer.GetFont(10, 0);
            IFont normalFont = writer.GetFont(11, 0);
            _lineCountFilterBlock = 0;

            items.ForEach(item =>
            {
                AppendBlockContent(ref contentRender, ref _lineCountFilterBlock, item.Item1, boldFont, item.Item2, normalFont, "\n");
            });

            IRichTextString contentRichTextString = contentRender.Render();
            IRichTextString[] contentRichTextArray = new IRichTextString[1];
            contentRichTextArray[0] = contentRichTextString;
            writer.WriteRow(1, contentRichTextArray);
        }

        protected virtual void FillRowData(IEnumerable<object[]> items, ref ExcelTemplateWriter writer)
        {
            foreach (var item in items)
            {
                writer.WriteRow(3, item);
            }
        }

        protected virtual void AppendBlockContent(ref RichTextRender content, ref int lineCount, string name, IFont nameFont, string value, IFont valueFont, string endString)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            TextBlock nameBlock = new TextBlock(name + ":  ", nameFont);
            content = content.AddText(nameBlock);

            TextBlock valueBlock = new TextBlock(value, valueFont);
            content = content.AddText(valueBlock);

            if (!string.IsNullOrEmpty(endString))
            {
                content = content.AddText(endString);
            }

            lineCount += CountLine(name + value);
        }

        protected virtual int CountLine(string line)
        {
            return Convert.ToInt32(Math.Ceiling((float)line.Length / 180));
        }

        protected virtual void FillHeader(ref ExcelTemplateWriter writer)
        {
            writer.CopyRows(2, 1);
        }

        protected virtual void FillFooter(object[] item, ref ExcelTemplateWriter writer)
        {
            writer.WriteRow(4, item);
        }
    }
}
