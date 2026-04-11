using System.Collections.Generic;
using System.Globalization;
using Godot;

namespace ZooBuilder.ui.settings;

public class LocaleUtil
{
    public static List<(string locale, string name)> GetSortedListOfLocalesForLocale(string currentLocale)
    {
        string[] locales = TranslationServer.GetLoadedLocales();
        CultureInfo sortCulture;
        try
        {
            sortCulture = new CultureInfo(currentLocale.Replace('_', '-'));
        }
        catch
        {
            sortCulture = CultureInfo.InvariantCulture;
        }
        
        List<(string locale, string name)> entries = [];
        foreach (string locale in locales)
        {
            string nativeLanguageName = GetNativeLanguageName(locale);
            entries.Add((locale, nativeLanguageName));
        }

        entries.Sort((a, b) =>
            sortCulture.CompareInfo.Compare(
                a.name,
                b.name,
                CompareOptions.StringSort
            )
        );

        return entries;
    }

    private static string GetNativeLanguageName(string locale)
    {
        // TODO: culture.Parent.NativeName?
        try
        {
            locale = locale.Replace('_', '-');
            var culture = new CultureInfo(locale);
            return Capitalize(culture.NativeName);
        }
        catch (CultureNotFoundException)
        {
            return locale;
        }
    }

    private static string Capitalize(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return char.ToUpper(text[0]) + text.Substring(1);
    }
}