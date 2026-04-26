using System.Collections.Generic;
using System.Globalization;
using Godot;

namespace ZooBuilder.ui.settings;

public partial class LocaleUtil : Node
{
    public static LocaleUtil Instance { get; private set; }
    
    public override void _Ready()
    {
        if (Instance != null)
        {
            QueueFree();
        }
        
        GD.Print("Set the singleton");
        Instance = this;
    }

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
    
    public string GetLocalizedLabelForKeyCode(Key key)
    {
        // Not sure what is going on in this AI generated madness
        // But it should ensure that on a Danish layout, buttons like ÆØÅ are printed correctly, and symbols like .,^¨ are printed as symbols, but space bar specifically is printed as a localized word, not " ". 
        // Supposedly this should also work on other keyboards and languages. Time will tell. 
        
        Key localizedKeyboardKey = DisplayServer.KeyboardGetLabelFromPhysical(key);
        string text;
        if (localizedKeyboardKey == Key.Space)
        {
            text = Tr("SPACE");
        }
        else if ((int)localizedKeyboardKey < 128 || char.IsLetterOrDigit((char)localizedKeyboardKey) || (char)localizedKeyboardKey > 127)
        {
            text = char.ConvertFromUtf32((int)localizedKeyboardKey);
        }
        else
        {
            text = OS.GetKeycodeString(localizedKeyboardKey);
        }

        return text;
    }
}