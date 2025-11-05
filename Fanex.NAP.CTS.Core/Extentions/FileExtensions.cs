namespace Fanex.NAP.CTS.Core.Extentions
{
    using Sylvan.Data.Excel;
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using static Fanex.NAP.CTS.Core.Extentions.Extentions;

    public static class FileExtensions
    {
        private const int CHUNK_SIZE = 10 * 1024 * 1024; // 10MB chunks for Base64 for csv processing

        private const int ROW_BATCH_SIZE = 10_000; // number of rows per batch for xls, xlsx processing
        private const int BASE64_BUFFER_SIZE = 10 * 1024 * 1024; // 10MB chunks for ROW_BATCH_SIZE processing
        private const int FLUSH_THRESHOLD = 10 * 1024 * 1024; // 10MB

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
                        foreach (var item in ConvertExcelToCsvChunked(file.InputStream, ExcelWorkbookType.ExcelXml))
                        {
                            yield return item;
                        }
                        break;

                    case XLS_EXTENSION:
                        foreach (var item in ConvertExcelToCsvChunked(file.InputStream, ExcelWorkbookType.Excel))
                        {
                            yield return item;
                        }
                        break;

                    default:
                        throw new InvalidOperationException("Unsupported file type.");
                }
            }
        }

        #region EXCEL (Sylvan)
        private static IEnumerable<byte[]> ConvertExcelToCsvChunked(Stream xlsxStream, ExcelWorkbookType excelType)
        {
            using (var reader = ExcelDataReader.Create(xlsxStream, excelType))
            {
                yield return WriteUtf8BomOnce();

                foreach (var chunk in ProcessExcelRowsBatch(reader))
                {
                    yield return chunk;
                }
            }
        }

        private static IEnumerable<byte[]> ProcessExcelRowsBatch(DbDataReader reader)
        {
            var sb = new StringBuilder();
            int rowCount = 0;

            // Write header once
            WriteHeader(reader, sb);

            while (reader.Read())
            {
                WriteRow(reader, sb);
                rowCount++;

                if (rowCount >= ROW_BATCH_SIZE)
                {
                    yield return Encoding.UTF8.GetBytes(sb.ToString());
                    sb.Clear();
                    rowCount = 0;
                }
            }

            // flush remaining rows
            if (sb.Length > 0)
                yield return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static void WriteHeader(DbDataReader reader, StringBuilder sb)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(EscapeCsv(reader.GetName(i)));
            }
            sb.AppendLine();
        }

        private static void WriteRow(DbDataReader reader, StringBuilder sb)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(EscapeCsv(reader.GetValue(i)?.ToString() ?? ""));
            }
            sb.AppendLine();
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
                return $"\"{value.Replace("\"", "\"\"")}\"";

            return value;
        }

        private static byte[] WriteUtf8BomOnce()
        {
            return Encoding.UTF8.GetPreamble();
        }
        #endregion

        #region Create StreamContent
        public static HttpContent CreateFileHttpContentFromModel(this HttpPostedFileBase file, object model, string nameOfFileBase)
        {
            return new PushStreamContent(async (outputStream, _, __) =>
            {
                StreamWriter writer = null;
                try
                {
                    using (writer = new StreamWriter(outputStream, Encoding.UTF8, 32 * 1024, leaveOpen: true))
                    {
                        await writer.WriteAsync("{");

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
                                await writer.WriteAsync(",");
                            }
                            first = false;

                            await writer.WriteAsync($"\"{prop.Name}\":");

                            switch (value)
                            {
                                case string str:
                                    await WriteEscapedJsonString(writer, str);
                                    break;
                                case int i:
                                    await writer.WriteAsync(i.ToString());
                                    break;
                                case null when prop.Name == nameOfFileBase:
                                    {
                                        await WriteEscapedJsonStringFromHttpPostedFileBase(writer, file);
                                        break;
                                    }
                                case StringBuilder sb:
                                    await WriteEscapedJsonStringFromStringBuilder(writer, sb);
                                    break;
                                default:
                                    await writer.WriteAsync($"\"{value.ToString()}\""); // fallback
                                    break;
                            }
                        }

                        await writer.WriteAsync("}");
                        await writer.FlushAsync();
                    }
                }
                finally
                {
                    writer?.Dispose();
                    outputStream.Close();
                }

            }, MEDIA_JSON_TYPE);
        }

        private static async Task WriteEscapedJsonStringFromHttpPostedFileBase(StreamWriter writer, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("Invalid file upload.");

            await writer.WriteAsync('"');

            byte[] leftover = new byte[2]; // Store leftover bytes that don't form complete 3-byte groups
            int leftoverCount = 0; // Number of leftover bytes
            char[] base64Buffer = ArrayPool<char>.Shared.Rent(BASE64_BUFFER_SIZE);

            long totalWrittenSinceFlush = 0;

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
                        await ProcessBase64ChunkAsync(writer, combined, 0, fullLen, base64Buffer);

                        // Calculate new leftover count after processing complete groups
                        leftoverCount = leftoverCount + length - fullLen;
                        if (leftoverCount > 0)
                        {
                            // Store remaining bytes for next iteration
                            Buffer.BlockCopy(combined, fullLen, leftover, 0, leftoverCount);
                        }

                        totalWrittenSinceFlush += fullLen;
                    }
                    else
                    {
                        // LOGIC 1: Round down to nearest multiple of 3 for Base64 processing
                        // Process only complete 3-byte groups, leave remainder for next iteration
                        int fullLen = length / 3 * 3;
                        await ProcessBase64ChunkAsync(writer, chunk, 0, fullLen, base64Buffer);

                        // Calculate leftover bytes that don't form complete groups
                        leftoverCount = length - fullLen;
                        if (leftoverCount > 0)
                        {
                            // Store remaining bytes for next iteration
                            Buffer.BlockCopy(chunk, fullLen, leftover, 0, leftoverCount);
                        }

                        totalWrittenSinceFlush += fullLen;
                    }

                    if (totalWrittenSinceFlush >= FLUSH_THRESHOLD)
                    {
                        await writer.FlushAsync();
                        totalWrittenSinceFlush = 0;
                    }
                }

                // Process final leftover bytes with padding
                if (leftoverCount > 0)
                {
                    int b64Len = Convert.ToBase64CharArray(leftover, 0, leftoverCount, base64Buffer, 0);
                    await writer.WriteAsync(base64Buffer, 0, b64Len);
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(base64Buffer);
            }

            await writer.WriteAsync('"');
            await writer.FlushAsync();
        }

        private static async Task ProcessBase64ChunkAsync(StreamWriter writer, byte[] data, int startIndex, int length, char[] base64Buffer)
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
                chunkSize = (chunkSize / 3) * 3;
                if (chunkSize == 0)
                {
                    break;
                }

                // Convert bytes to Base64 characters
                int b64Len = Convert.ToBase64CharArray(data, startIndex + processed, chunkSize, base64Buffer, 0);

                // Write Base64 string to output
                await writer.WriteAsync(base64Buffer, 0, b64Len);

                processed += chunkSize;
            }
        }
        #endregion
    }
}