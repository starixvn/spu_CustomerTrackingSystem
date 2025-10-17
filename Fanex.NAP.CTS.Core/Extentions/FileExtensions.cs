namespace Fanex.NAP.CTS.Core.Extentions
{
    using ExcelDataReader;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Xml;

    public static class FileExtensions
    {
        private const int ChunkSize = 10 * 1024 * 1024; // 10MB chunks for Base64
        private const string CSV_Extension = ".csv";

        public static StringBuilder FileBaseToBase64StringChunked(this HttpPostedFileBase fileBase)
        {
            var sb = new StringBuilder();
            var buffer = new byte[ChunkSize];
            int bytesRead;

            while ((bytesRead = fileBase.InputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                // Convert chunk to Base64
                if (bytesRead == ChunkSize)
                {
                    sb.Append(Convert.ToBase64String(buffer));
                }
                else
                {
                    // Handle last chunk which may be smaller
                    var lastChunk = new byte[bytesRead];
                    Array.Copy(buffer, lastChunk, bytesRead);
                    sb.Append(Convert.ToBase64String(lastChunk));
                }
            }

            return sb;
        }

        /// <summary>
        /// Convert HttpPostedFileBase (xls or xlsx) to Base64 CSV
        /// </summary>
        public static StringBuilder ConvertExcelToCsvBase64Optimized(this HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("Invalid file upload.");

            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (ext == CSV_Extension)
            {
                return file.FileBaseToBase64StringChunked(); // existing method for csv
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var sb = new StringBuilder();
            var csvBuffer = new MemoryStream();
            var base64Chunks = new ConcurrentQueue<string>();

            switch (ext)
            {
                case ".xlsx":
                    {
                        ConvertXlsxStream(file.InputStream, csvBuffer, base64Chunks);
                        break;
                    }
                case ".xls":
                    {
                        ConvertXlsStream(file.InputStream, csvBuffer, base64Chunks);
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("Unsupported file type.");
                    }
            }

            // Flush remaining bytes
            if (csvBuffer.Length > 0)
            {
                AppendBase64Chunk(csvBuffer, base64Chunks, flushAll: true);
            }

            // Combine all Base64 chunks
            while (base64Chunks.TryDequeue(out var chunk))
            {
                sb.Append(chunk);
            }

            return sb;
        }

        #region XLSX (OpenXML, SAX)
        private static void ConvertXlsxStream(Stream xlsxStream, MemoryStream csvBuffer, ConcurrentQueue<string> base64Chunks)
        {
            using (var archive = new ZipArchive(xlsxStream, ZipArchiveMode.Read, leaveOpen: true))
            {
                string[] sharedStrings = LoadSharedStrings(archive);

                var sheetEntry = archive.GetEntry("xl/worksheets/sheet1.xml");
                if (sheetEntry == null)
                    throw new InvalidOperationException("sheet1.xml not found.");

                using (var sheetStream = sheetEntry.Open())
                using (var reader = XmlReader.Create(sheetStream, new XmlReaderSettings { IgnoreWhitespace = true }))
                {
                    // Write UTF-8 BOM once
                    if (csvBuffer.Length == 0)
                    {
                        var bom = Encoding.UTF8.GetPreamble();
                        csvBuffer.Write(bom, 0, bom.Length);
                    }

                    char delimiter = ',';

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "row")
                        {
                            var rowBuilder = new StringBuilder();

                            while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "row"))
                            {
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "c")
                                {
                                    string cellType = reader.GetAttribute("t");
                                    string value = ReadCellValue(reader);

                                    if (cellType == "s" && int.TryParse(value, out int ssi) && ssi >= 0 && ssi < sharedStrings.Length)
                                        value = sharedStrings[ssi];

                                    if (value != null)
                                    {
                                        if (value.Contains("\"") || value.Contains(delimiter) || value.Contains("\n") || value.Contains("\r"))
                                            value = "\"" + value.Replace("\"", "\"\"") + "\"";
                                    }
                                    else value = "";

                                    if (rowBuilder.Length > 0)
                                        rowBuilder.Append(delimiter);

                                    rowBuilder.Append(value);
                                }
                            }

                            byte[] lineBytes = Encoding.UTF8.GetBytes(rowBuilder.ToString() + "\r\n");
                            csvBuffer.Write(lineBytes, 0, lineBytes.Length);

                            if (csvBuffer.Length >= ChunkSize)
                                AppendBase64Chunk(csvBuffer, base64Chunks);
                        }
                    }
                }
            }
        }


        private static string[] LoadSharedStrings(ZipArchive archive)
        {
            var sharedEntry = archive.GetEntry("xl/sharedStrings.xml");
            if (sharedEntry == null) return Array.Empty<string>();

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
        private static void ConvertXlsStream(Stream xlsStream, MemoryStream csvBuffer, ConcurrentQueue<string> base64Chunks)
        {
            using (var reader = ExcelReaderFactory.CreateReader(xlsStream))
            {
                do
                {
                    while (reader.Read())
                    {
                        var rowBuilder = new StringBuilder();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var value = reader.GetValue(i)?.ToString().Replace("\"", "\"\"");
                            if (i > 0)
                                rowBuilder.Append(',');

                            rowBuilder.Append('"').Append(value).Append('"');
                        }

                        byte[] lineBytes = Encoding.UTF8.GetBytes(rowBuilder.ToString() + "\n");
                        csvBuffer.Write(lineBytes, 0, lineBytes.Length);

                        if (csvBuffer.Length >= ChunkSize)
                            AppendBase64Chunk(csvBuffer, base64Chunks);
                    }
                } while (reader.NextResult());
            }
        }
        #endregion

        #region Base64 Chunk
        private static void AppendBase64Chunk(MemoryStream csvBuffer, ConcurrentQueue<string> base64Chunks, bool flushAll = false)
        {
            csvBuffer.Position = 0;
            long totalBytes = csvBuffer.Length;
            long bytesToEncode = flushAll ? totalBytes : (totalBytes / 3) * 3;

            if (bytesToEncode > 0)
            {
                byte[] chunk = new byte[bytesToEncode];
                csvBuffer.Read(chunk, 0, (int)bytesToEncode);

                // Encode chunk parallel-safe
                string base64 = Convert.ToBase64String(chunk);
                base64Chunks.Enqueue(base64);
            }

            long remainingBytes = totalBytes - bytesToEncode;
            var remaining = new MemoryStream();
            if (remainingBytes > 0)
            {
                byte[] leftover = new byte[remainingBytes];
                csvBuffer.Read(leftover, 0, (int)remainingBytes);
                remaining.Write(leftover, 0, (int)remainingBytes);
            }

            csvBuffer.SetLength(0);
            remaining.Position = 0;
            remaining.CopyTo(csvBuffer);
        }
        #endregion
    }
}