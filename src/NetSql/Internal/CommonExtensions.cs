using System;

namespace NetSql.Internal
{
    /// <summary>
    /// 通用扩展方法
    /// </summary>
    public static class CommonExtensions
    {
        #region ==数据转换扩展==

        /// <summary>
        /// 转换成Byte
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns></returns>
        public static byte ToByte(this object s)
        {
            if (s == null || s == DBNull.Value)
                return 0;

            byte.TryParse(s.ToString(), out byte result);
            return result;
        }

        /// <summary>
        /// 转换成short/Int16
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static short ToShort(this object s)
        {
            if (s == null || s == DBNull.Value)
                return 0;

            short.TryParse(s.ToString(), out short result);
            return result;
        }

        /// <summary>
        /// 转换成Int/Int32
        /// </summary>
        /// <param name="s"></param>
        /// <param name="round">是否四舍五入，默认false</param>
        /// <returns></returns>
        public static int ToInt(this object s, bool round = false)
        {
            if (s == null || s == DBNull.Value)
                return 0;
            if (s.GetType().IsEnum)
            {
                return (int) s;
            }

            if (Int32.TryParse(s.ToString(), out int result))
                return result;
            var f = s.ToFloat();
            return round ? Convert.ToInt32(f) : (int)f;
        }

        /// <summary>
        /// 转换成Long/Int64
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long ToLong(this object s)
        {
            if (s == null || s == DBNull.Value)
                return 0L;

            long.TryParse(s.ToString(), out long result);
            return result;
        }

        /// <summary>
        /// 转换成Float/Single
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float ToFloat(this object s)
        {
            if (s == null || s == DBNull.Value)
                return 0f;

            float.TryParse(s.ToString(), out float result);
            return result;
        }

        /// <summary>
        /// 转换成Double/Single
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double ToDouble(this object s)
        {
            if (s == null || s == DBNull.Value)
                return 0d;

            double.TryParse(s.ToString(), out double result);
            return result;
        }

        /// <summary>
        /// 转换成Decimal
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object s)
        {
            if (s == null || s == DBNull.Value) return 0m;
            Decimal.TryParse(s.ToString(), out decimal result);
            return result;
        }

        /// <summary>
        /// 转换成DateTime
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object s)
        {
            if (s == null || s == DBNull.Value)
                return DateTime.MinValue;

            DateTime.TryParse(s.ToString(), out DateTime result);
            return result;
        }

        /// <summary>
        /// 转换成Date
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ToDate(this object s)
        {
            return s.ToDateTime().Date;
        }

        /// <summary>
        /// 转换成Boolean
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ToBool(this object s)
        {
            if (s == null) return false;
            s = s.ToString().ToLower();
            if (s.Equals(1) || s.Equals("1") || s.Equals("true") || s.Equals("是") || s.Equals("yes"))
                return true;
            if (s.Equals(0) || s.Equals("0") || s.Equals("false") || s.Equals("否") || s.Equals("no"))
                return false;

            Boolean.TryParse(s.ToString(), out bool result);
            return result;
        }

        /// <summary>
        /// 布尔值转换为字符串1或者0
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToIntString(this bool b)
        {
            return b ? "1" : "0";
        }

        /// <summary>
        /// 布尔值转换为整数1或者0
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }

        /// <summary>
        /// 泛型转换，转换失败会抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T To<T>(this object s)
        {
            return (T)Convert.ChangeType(s, typeof(T));
        }

        #endregion

        #region ==字符串操作==

        /// <summary>
        /// 判断字符串是否为空(封装string.IsNullOrWhiteSpace)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNull(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// 判断字符串是否不为空(封装string.IsNullOrWhiteSpace)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool NotNull(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// 与字符串进行比较，忽略大小写
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string s, string value)
        {
            return s.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
