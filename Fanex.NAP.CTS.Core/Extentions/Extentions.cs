namespace Fanex.NAP.CTS.Core.Extentions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

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

        public static string FileBaseToBase64String(this HttpPostedFileBase fileBase)
        {
            using (var binaryReader = new BinaryReader(fileBase.InputStream))
            {
                var fileBytes = binaryReader.ReadBytes(fileBase.ContentLength);
                var base64 = Convert.ToBase64String(fileBytes);

                return base64;
            }
        }
    }
}