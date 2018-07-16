﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NetSql.Entities;

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
                throw new ArgumentNullException($"{parameterName} is null");
        }

        /// <summary>
        /// 检测字符串是否为空
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parameterName"></param>
        public static void NotNull(string obj, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(obj))
                throw new ArgumentNullException($"{parameterName} is null");
        }

        /// <summary>
        /// 检测集合是否不为空且包含内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="parameterName"></param>
        public static void Collection<T>(IList<T> list, string parameterName)
        {
            NotNull(list, parameterName);

            if (!list.Any())
                throw new ArgumentException("集合不包含任何内容", parameterName);
        }
    }
}