namespace Fanex.NAP.CTS.Core.Extentions
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public static class Extentions
    {
        public static IEnumerable<IEnumerable<TValue>> Chunk2<TValue>(
            this IEnumerable<TValue> values,
            Int32 chunkSize)
        {
            return values
                .Select((v, i) => new { v, groupIndex = i / chunkSize })
                .GroupBy(x => x.groupIndex)
                .Select(g => g.Select(x => x.v));
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            var q = new Queue<T>();

            foreach (var item in source)
            {
                // check if q is full
                if (q.Count == chunksize)
                {
                    var ret = q;
                    q = new Queue<T>();

                    yield return ret;
                }

                // enqueue item
                q.Enqueue(item);
            }

            yield return q;
        }

        public static string CreateMD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static void SetPropertyValue(this object target, PropertyInfo property, object value, Type targetType = null)
        {
            MethodInfo setMethod = property.GetSetMethod(nonPublic: true);
            if (setMethod != null)
            {
                setMethod.Invoke(target, BindingFlags.Default, null, new object[1] { value }, CultureInfo.InvariantCulture);
            }
            else
            {
                string name = "<" + property.Name + ">i__Field";
                (targetType ?? target.GetType()).GetField(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic).GetSetter()(target, value);
            }
        }

        public static Action<object, object> GetSetter(this FieldInfo fieldInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(fieldInfo.Name, typeof(void), new Type[2]
            {
            typeof(object),
            typeof(object)
            }, fieldInfo.DeclaringType);
            ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
            LocalBuilder local = iLGenerator.DeclareLocal(fieldInfo.DeclaringType);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
            iLGenerator.Emit(OpCodes.Stloc_0, local);
            iLGenerator.Emit(OpCodes.Ldloc_0, local);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            if (fieldInfo.FieldType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Castclass, fieldInfo.FieldType);
            }

            iLGenerator.Emit(OpCodes.Stfld, fieldInfo);
            iLGenerator.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(Action<object, object>)) as Action<object, object>;
        }

        public static string CleanSpecialCharactor(this string input) =>
            string.IsNullOrEmpty(input) ? "" : input.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("__", "_").Trim();

        public static async Task WriteEscapedJsonStringFromStringBuilder(StreamWriter writer, StringBuilder sb, int bufferSize = 10 * 1024 * 1024) // chunk 10MB
        {
            var pool = ArrayPool<char>.Shared;
            char[] buffer = pool.Rent(bufferSize);
            char[] writeBuf = pool.Rent(bufferSize * 2);

            await writer.WriteAsync('"');
            int totalLength = sb.Length;

            try
            {
                for (int offset = 0; offset < totalLength; offset += bufferSize)
                {
                    int length = Math.Min(bufferSize, totalLength - offset);
                    sb.CopyTo(offset, buffer, 0, length);

                    int w = 0;
                    for (int i = 0; i < length; i++)
                    {
                        w = AppendEscapedJsonChar(buffer[i], writeBuf, w);

                        if (w >= writeBuf.Length - 8)
                        {
                            writer.Write(writeBuf, 0, w);
                            w = 0;
                        }
                    }

                    if (w > 0)
                        await writer.WriteAsync(writeBuf, 0, w);
                }
            }
            finally
            {
                pool.Return(buffer);
                pool.Return(writeBuf);
            }

            await writer.WriteAsync('"');
        }

        public static async Task WriteEscapedJsonString(StreamWriter writer, string value)
        {
            await writer.WriteAsync('"');

            int estimatedLength = value.Length + value.Length / 10;
            char[] buffer = ArrayPool<char>.Shared.Rent(estimatedLength);

            try
            {
                int w = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    w = AppendEscapedJsonChar(value[i], buffer, w);

                    if (w > buffer.Length - 8)
                    {
                        await writer.WriteAsync(buffer, 0, w);
                        w = 0;
                    }
                }

                if (w > 0)
                {
                    await writer.WriteAsync(buffer, 0, w);
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }

            await writer.WriteAsync('"');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AppendEscapedJsonChar(char c, char[] dest, int w)
        {
            switch (c)
            {
                case '\\': dest[w++] = '\\'; dest[w++] = '\\'; break;
                case '"': dest[w++] = '\\'; dest[w++] = '"'; break;
                case '\b': dest[w++] = '\\'; dest[w++] = 'b'; break;
                case '\f': dest[w++] = '\\'; dest[w++] = 'f'; break;
                case '\n': dest[w++] = '\\'; dest[w++] = 'n'; break;
                case '\r': dest[w++] = '\\'; dest[w++] = 'r'; break;
                case '\t': dest[w++] = '\\'; dest[w++] = 't'; break;
                default: dest[w++] = c; break;
            }
            return w;
        }
    }
}