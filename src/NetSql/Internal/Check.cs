using System;

namespace NetSql.Internal
{
    internal class Check
    {
        /// <summary>
        /// 检测对象是否为空
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parameterName"></param>
        public static void NotNull<T>(T obj, string parameterName)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// 检测字符串是否为空
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parameterName"></param>
        public static void NotNull(string obj, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(obj))
                throw new ArgumentNullException(parameterName);
        }
    }
}
