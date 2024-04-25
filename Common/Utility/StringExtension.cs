namespace Common.Utility;

public static class StringExtension
{
    public static bool IsBlank(this string str)
    {
        return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
    }
}   