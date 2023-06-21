using System.ComponentModel;
using System.Reflection;

namespace JT808Server.Utility.Common
{
    public class EnumHelper
    {
        public class EnumItem
        {
            public string Text { set; get; }
            public int Value { set; get; }
        }
        public class EnumItem2
        {
            public string Text { set; get; }
            public string Value { set; get; }
        }
        /// <summary>
        /// 获取枚举值上的Description特性的说明
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="obj">枚举值</param>
        /// <returns>特性的说明</returns>
        public static string GetEnumDescription<T>(T obj)
        {
            if (obj == null || obj.ToString() == "0")
            {
                return string.Empty;
            }
            var type = obj.GetType();
            FieldInfo field = type.GetField(System.Enum.GetName(type, obj));
            DescriptionAttribute descAttr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (descAttr == null)
            {
                return string.Empty;
            }
            return descAttr.Description;
        }

        /// <summary>
        /// 获取枚举值，形成列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<EnumItem> GetTextValueList<T>()
        {
            List<EnumItem> itemList = new List<EnumItem>();
            foreach (var item in System.Enum.GetValues(typeof(T)))
            {
                EnumItem ei = new EnumItem();
                ei.Text = GetEnumDescription<T>((T)item);
                ei.Value = (int)item;
                itemList.Add(ei);
            }
            return itemList;
        }

        public static List<EnumItem2> GetTextValueList2<T>()
        {
            List<EnumItem2> itemList = new List<EnumItem2>();
            foreach (var item in System.Enum.GetValues(typeof(T)))
            {
                EnumItem2 ei = new EnumItem2();
                ei.Text = GetEnumDescription<T>((T)item);
                ei.Value = item.ToString();
                itemList.Add(ei);
            }
            return itemList;
        }

        /// <summary>
        /// 获取枚举值，形成列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exceptions">排除某些值</param>
        /// <returns></returns>
        public static List<EnumItem> GetTextValueList<T>(List<int> exceptions)
        {
            if (exceptions == null)
                return GetTextValueList<T>();
            else
                return GetTextValueList<T>().Where(c => !exceptions.Contains(c.Value)).ToList();
        }
        /// <summary>
        /// 获取枚举值，形成列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exception">排除某个值</param>
        /// <returns></returns>
        public static List<EnumItem> GetTextValueList<T>(int exception)
        {
            return GetTextValueList<T>().Where(c => exception != c.Value).ToList();
        }
        /// <summary>
        /// 根据枚举中的某值 获取对应的枚举int value
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="obj">枚举值</param>
        /// <returns>特性的说明</returns>
        public static T GetEnumValue<T>(int value)
        {
            //List<string> enumIdList = ((int[])enumType.GetEnumValues()).Select(i => i.ToString()).ToList();
            foreach (var item in System.Enum.GetValues(typeof(T)))
            {
                if (value == (int)item)
                {
                    return (T)item;
                }
            }
            return default(T);
        }
        /// <summary>
        /// 根据枚举中的某值 获取对应的枚举string value
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="obj">枚举值</param>
        /// <returns>特性的说明</returns>
        public static T GetEnumValue<T>(string value)
        {
            //List<string> enumIdList = ((int[])enumType.GetEnumValues()).Select(i => i.ToString()).ToList();
            foreach (var item in System.Enum.GetValues(typeof(T)))
            {
                if (value == item.ToString())
                {
                    return (T)item;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 根据int值  获取枚举值上的Description特性的说明
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="obj">枚举值</param>
        /// <returns>特性的说明</returns>
        public static string GetEnumDescription<T>(int? value)
        {
            if (value == null) return "";
            return GetEnumDescription(GetEnumValue<T>((int)value));
        }

        /// <summary>
        /// 根据bool值  获取枚举值上的Description特性的说明
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="obj">枚举值</param>
        /// <returns>特性的说明</returns>
        public static string GetEnumDescription<T>(bool? value)
        {
            if (value == null) return "";
            return GetEnumDescription(GetEnumValue<T>(value.Value ? 1 : 0));
        }

        public static string GetEnumDescription<T>(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            try
            {
                var tmp = (T)Enum.Parse(typeof(T), value);
                return GetEnumDescription(tmp);
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }
    }
    public class EnumItemComparition : IComparer<EnumHelper.EnumItem>
    {
        private bool IsAsc = true;

        public EnumItemComparition(bool isAsc = true)
        {
            IsAsc = isAsc;
        }

        public int Compare(EnumHelper.EnumItem x, EnumHelper.EnumItem y)
        {
            if (IsAsc)
            {
                return x.Value - y.Value;
            }
            else
            {
                return y.Value - x.Value;
            }
        }
    }
}
