using System.Windows;

public static class ThemeManager
{
    public static void ApplyTheme(string themeName)
    {
        // Remove previous theme
        var existingTheme = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Theme.xaml"));

        if (existingTheme != null)
            Application.Current.Resources.MergedDictionaries.Remove(existingTheme);

        var dict = new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/DesktopTool.App.Themes;component/{themeName}")
        };

        Application.Current.Resources.MergedDictionaries.Add(dict);
    }
}
