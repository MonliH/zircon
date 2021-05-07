using System.ComponentModel;

namespace ZirconLang
{
    public enum Color
    {
        [Description("\u001b[0m")] Reset,
        [Description("\u001b[1m")] Bold,

        [Description("\u001b[31m")] Red,

        [Description("\u001b[38;5;60m")] NicePurple,
    }

    public static class ColorExtentions
    {
        public static string ToS(this Color val)
        {
            DescriptionAttribute[]? attributes = (DescriptionAttribute[]?) val
                .GetType()
                .GetField(val.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes == null ? "" : attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}