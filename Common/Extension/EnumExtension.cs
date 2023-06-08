using System.ComponentModel;

namespace Common.Extension
{
    public static class EnumExtension
    {
        public static string GetDescription(this System.Enum enu)
        {
            var type = enu.GetType();

            var memInfo = type.GetMember(enu.ToString());

            if (memInfo.Length <= 0) return enu.ToString();
            var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : enu.ToString();
        }

        public static List<KeyValuePair<int, string>> GetEnumList(this System.Enum enu)
        {
            var li = new List<KeyValuePair<int, string>>();

            foreach (var item in enu.GetType().GetEnumValues())
            {
                li.Add(new KeyValuePair<int, string>(int.Parse(((System.Enum)item).ToString("D")), ((System.Enum)item).GetDescription()));
            }

            return li;
        }

        public static Dictionary<int, string> GetEnumDictionary<T>()
        {
            var li = new Dictionary<int, string>();
            try
            {
                var enumType = typeof(T);
                if (enumType.BaseType != typeof(System.Enum))
                    throw new ArgumentException("T must be of type System.Enum");

                foreach (var item in enumType.GetEnumValues())
                {
                    li.Add(int.Parse(((System.Enum)item).ToString("D")), ((System.Enum)item).GetDescription());
                }

                return li;
            }
            catch
            {
                return li;
            }
        }

        public static List<EnumObject> EnumToList<T>()
        {
            var enumType = typeof(T);
            if (enumType.BaseType != typeof(System.Enum))
                throw new ArgumentException("T must be of type System.Enum");

            var li = new List<EnumObject>();
            foreach (var item in enumType.GetEnumValues())
            {
                li.Add(new EnumObject((System.Enum)item));
            }
            return li;
        }
    }

    public class EnumObject
    {
        public EnumObject(System.Enum valueMember)
        {
            ValueMember = valueMember;
        }

        public System.Enum ValueMember { get; set; }

        public int IntValueMember => int.Parse(ValueMember.ToString("D"));

        public string StringValueMember => ValueMember.ToString("");

        public string DisplayMember => ValueMember.GetDescription();
    }
}
