namespace Fanex.NAP.CTS.Core.Extentions
{
    using ExcelDataReader;
    using NPOI.SS.Formula.Functions;
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using System.Xml;
    using static Fanex.NAP.CTS.Core.Extentions.Extentions;

    public static class FileExtensions
    {
        private const int CHUNK_SIZE = 10 * 1024 * 1024; // 10MB chunks for Base64 for csv processing

        private const int ROW_BATCH_SIZE = 10_000; // number of rows per batch for xls, xlsx processing
        private const int BASE64_BUFFER_SIZE = 10 * 1024 * 1024; // 10MB chunks for ROW_BATCH_SIZE processing

        private const string CSV_EXTENSION = ".csv";
        private const string XLSX_EXTENSION = ".xlsx";
        private const string XLS_EXTENSION = ".xls";
        private const string MEDIA_JSON_TYPE = "application/json";

        public static IEnumerable<byte[]> FileBaseToChunked(this HttpPostedFileBase fileBase)
        {
            var pool = ArrayPool<byte>.Shared;
            byte[] buffer = pool.Rent(CHUNK_SIZE);

            try
            {
                int bytesRead;
                while ((bytesRead = fileBase.InputStream.Read(buffer, 0, CHUNK_SIZE)) > 0)
                {
                    var chunk = new byte[bytesRead];
                    Buffer.BlockCopy(buffer, 0, chunk, 0, bytesRead);
                    yield return chunk;
                }
            }
            finally
            {
                pool.Return(buffer);
            }
        }

        public static IEnumerable<byte[]> ConvertHttpPostedExcelFileToCsvChunked(this HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("Invalid file upload.");

            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (ext == CSV_EXTENSION)
            {
                foreach (var item in file.FileBaseToChunked())
                {
                    yield return item;
                }
            }
            else
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                switch (ext)
                {
                    case XLSX_EXTENSION:
                        foreach (var item in ConvertXlsxStreamToChunked(file.InputStream))
                        {
                            yield return item;
                        }
                        break;

                    case XLS_EXTENSION:
                        foreach (var item in ConvertXlsStreamToChunked(file.InputStream))
                        {
                            yield return item;
                        }
                        break;

                    default:
                        throw new InvalidOperationException("Unsupported file type.");
                }
            }
        }

        #region XLSX (OpenXML, SAX)
        private static IEnumerable<byte[]> ConvertXlsxStreamToChunked(Stream xlsxStream)
        {
            using (var archive = new ZipArchive(xlsxStream, ZipArchiveMode.Read, leaveOpen: true))
            {
                string[] sharedStrings = LoadSharedStrings(archive);

                var sheetEntry = archive.GetEntry("xl/worksheets/sheet1.xml")
                    ?? throw new InvalidOperationException("sheet1.xml not found.");

                using (var sheetStream = sheetEntry.Open())
                using (var reader = XmlReader.Create(sheetStream, new XmlReaderSettings { IgnoreWhitespace = true }))
                {
                    yield return WriteUtf8BomOnce();
                    foreach (var item in ProcessSheetRowsBatch(reader, sharedStrings))
                    {
                        yield return item;
                    }
                    ;
                }
            }
        }

        private static IEnumerable<byte[]> ProcessSheetRowsBatch(XmlReader reader, string[] sharedStrings)
        {
            char delimiter = ',';
            var sb = new StringBuilder();
            int count = 0;

            while (reader.Read())
            {
                if (!IsRowStart(reader)) continue;

                var rowText = ReadRow(reader, sharedStrings, delimiter);
                sb.AppendLine(rowText);
                count++;

                if (count >= ROW_BATCH_SIZE)
                {
                    yield return Encoding.UTF8.GetBytes(sb.ToString());
                    sb.Clear();
                    count = 0;
                }
            }

            // Return leftover rows
            if (sb.Length > 0)
                yield return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static bool IsRowStart(XmlReader reader)
            => reader.NodeType == XmlNodeType.Element && reader.Name == "row";

        private static string ReadRow(XmlReader reader, string[] sharedStrings, char delimiter)
        {
            var rowBuilder = new StringBuilder();

            while (reader.Read())
            {
                if (IsRowEnd(reader)) break;
                if (!IsCellStart(reader)) continue;

                string value = ResolveCellValue(reader, sharedStrings);
                if (rowBuilder.Length > 0)
                    rowBuilder.Append(delimiter);

                rowBuilder.Append(value);
            }

            return rowBuilder.ToString();
        }

        private static bool IsRowEnd(XmlReader reader)
            => reader.NodeType == XmlNodeType.EndElement && reader.Name == "row";

        private static bool IsCellStart(XmlReader reader)
            => reader.NodeType == XmlNodeType.Element && reader.Name == "c";

        private static string ResolveCellValue(XmlReader reader, string[] sharedStrings)
        {
            string cellType = reader.GetAttribute("t");
            string value = ReadCellValue(reader);

            if (cellType == "s" && int.TryParse(value, out int ssi) && ssi >= 0 && ssi < sharedStrings.Length)
                value = sharedStrings[ssi];

            return EscapeCsvValue(value);
        }

        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains("\"") || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
                return $"\"{value.Replace("\"", "\"\"")}\"";

            return value;
        }

        private static byte[] WriteUtf8BomOnce()
        {
            var bom = Encoding.UTF8.GetPreamble();
            return bom;
        }

        private static string[] LoadSharedStrings(ZipArchive archive)
        {
            var sharedEntry = archive.GetEntry("xl/sharedStrings.xml");
            if (sharedEntry == null)
                return Array.Empty<string>();

            var strings = new List<string>();
            using (var stream = sharedEntry.Open())
            using (var reader = XmlReader.Create(stream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "t")
                        strings.Add(reader.ReadElementContentAsString());
                }
            }

            return strings.ToArray();
        }

        private static string ReadCellValue(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "v")
                    return reader.ReadElementContentAsString();

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "c")
                    break;
            }

            return string.Empty;
        }
        #endregion

        #region XLS (BIFF) using ExcelDataReader
        private static IEnumerable<byte[]> ConvertXlsStreamToChunked(Stream xlsStream)
        {
            using (var reader = ExcelReaderFactory.CreateReader(xlsStream))
            {

                do
                {
                    foreach (var item in ProcessSheetBatch(reader))
                    {
                        yield return item;
                    }
                }
                while (reader.NextResult());
            }
        }

        private static IEnumerable<byte[]> ProcessSheetBatch(IExcelDataReader reader)
        {
            var sb = new StringBuilder();
            int count = 0;

            while (reader.Read())
            {
                string rowText = BuildCsvRow(reader);
                sb.AppendLine(rowText);
                count++;

                if (count >= ROW_BATCH_SIZE)
                {
                    yield return Encoding.UTF8.GetBytes(sb.ToString());
                    sb.Clear();
                    count = 0;
                }
            }

            if (sb.Length > 0)
                yield return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static string BuildCsvRow(IExcelDataReader reader)
        {
            var rowBuilder = new StringBuilder();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string value = EscapeCsvValue(reader.GetValue(i)?.ToString() ?? "");
                if (i > 0)
                    rowBuilder.Append(',');

                rowBuilder.Append(value);
            }

            return rowBuilder.ToString();
        }
        #endregion

        #region Create StreamContent
        public static StreamContent CreateFileStreamContentFromModel(this HttpPostedFileBase file, object model, string nameOfFileBase)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true))
            {
                writer.Write("{");

                var props = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                bool first = true;

                foreach (var prop in props)
                {
                    var value = prop.GetValue(model);
                    if (value == null && prop.Name != nameOfFileBase)
                    {
                        continue;
                    }

                    if (!first)
                    {
                        writer.Write(",");
                    }
                    first = false;

                    writer.Write($"\"{prop.Name}\":");

                    switch (value)
                    {
                        case string str:
                            WriteEscapedJsonString(writer, str);
                            break;
                        case int i:
                            writer.Write(i.ToString());
                            break;
                        case null when prop.Name == nameOfFileBase:
                            {
                                WriteEscapedJsonStringFromHttpPostedFileBase(writer, file);
                                break;
                            }
                        case StringBuilder sb:
                            WriteEscapedJsonStringFromStringBuilder(writer, sb);
                            break;
                        default:
                            writer.Write($"\"{value.ToString()}\""); // fallback
                            break;
                    }
                }

                writer.Write("}");
                writer.Flush();
            }

            stream.Position = 0;

            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(MEDIA_JSON_TYPE);

            return content;
        }

        private static void WriteEscapedJsonStringFromHttpPostedFileBase(StreamWriter writer, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("Invalid file upload.");

            writer.Write('"');

            byte[] leftover = new byte[2]; // Store leftover bytes that don't form complete 3-byte groups
            int leftoverCount = 0; // Number of leftover bytes
            char[] base64Buffer = ArrayPool<char>.Shared.Rent(BASE64_BUFFER_SIZE);

            try
            {
                foreach (var chunk in ConvertHttpPostedExcelFileToCsvChunked(file))
                {
                    int length = chunk.Length;

                    if (leftoverCount > 0)
                    {
                        // Combine leftover bytes with new chunk
                        byte[] combined = new byte[leftoverCount + length];
                        Buffer.BlockCopy(leftover, 0, combined, 0, leftoverCount);
                        Buffer.BlockCopy(chunk, 0, combined, leftoverCount, length);

                        // LOGIC 1: Round down to nearest multiple of 3 for Base64 processing
                        // Base64 processes data in groups of 3 bytes (3 bytes → 4 characters)
                        // We need to ensure we only process complete 3-byte groups
                        int fullLen = (leftoverCount + length) / 3 * 3;
                        ProcessBase64Chunk(writer, combined, 0, fullLen, base64Buffer);

                        // Calculate new leftover count after processing complete groups
                        leftoverCount = leftoverCount + length - fullLen;
                        if (leftoverCount > 0)
                        {
                            // Store remaining bytes for next iteration
                            Buffer.BlockCopy(combined, fullLen, leftover, 0, leftoverCount);
                        }
                    }
                    else
                    {
                        // LOGIC 1: Round down to nearest multiple of 3 for Base64 processing
                        // Process only complete 3-byte groups, leave remainder for next iteration
                        int fullLen = length / 3 * 3;
                        ProcessBase64Chunk(writer, chunk, 0, fullLen, base64Buffer);

                        // Calculate leftover bytes that don't form complete groups
                        leftoverCount = length - fullLen;
                        if (leftoverCount > 0)
                        {
                            // Store remaining bytes for next iteration
                            Buffer.BlockCopy(chunk, fullLen, leftover, 0, leftoverCount);
                        }
                    }
                }

                // Process final leftover bytes with padding
                if (leftoverCount > 0)
                {
                    int b64Len = Convert.ToBase64CharArray(leftover, 0, leftoverCount, base64Buffer, 0);
                    writer.Write(base64Buffer, 0, b64Len);
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(base64Buffer);
            }

            writer.Write('"');
            writer.Flush();
        }

        private static void ProcessBase64Chunk(StreamWriter writer, byte[] data, int startIndex, int length, char[] base64Buffer)
        {
            // LOGIC 2: Calculate maximum input bytes that fit in output buffer
            // Base64 ratio: 3 bytes → 4 characters
            // So 1 character → 3/4 bytes
            // We need to ensure output doesn't exceed buffer size
            const int MAX_CHUNK_SIZE = BASE64_BUFFER_SIZE * 3 / 4;

            int processed = 0;
            while (processed < length)
            {
                // Split large data into smaller chunks that fit in buffer
                // This prevents IndexOutOfRangeException when encoding
                int chunkSize = Math.Min(length - processed, MAX_CHUNK_SIZE);

                // Convert bytes to Base64 characters
                int b64Len = Convert.ToBase64CharArray(data, startIndex + processed, chunkSize, base64Buffer, 0);

                // Write Base64 string to output
                writer.Write(base64Buffer, 0, b64Len);
                writer.Flush(); // Force flush to reduce memory pressure

                processed += chunkSize;
            }
        }
        #endregion
    }
}