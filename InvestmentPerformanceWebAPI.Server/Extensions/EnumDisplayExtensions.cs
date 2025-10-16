using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace InvestmentPerformanceWebAPI.Server.Extensions {
    public static class EnumDisplayExtensions {
        public static string GetDisplayName(this Enum value) {
            Type type = value.GetType();
            string name = Enum.GetName(type, value) ?? string.Empty;
            if (string.IsNullOrEmpty(name)) {
                return string.Empty;
            }

            MemberInfo? member = type.GetMember(name).FirstOrDefault();
            if (member is null) {
                return name;
            }

            var display = member.GetCustomAttribute<DisplayAttribute>();
            return display?.GetName() ?? name;
        }
    }
}
