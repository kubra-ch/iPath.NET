using MudBlazor;

namespace iPath.UI.Themes;

public static class iPathThemes
{
    public static MudTheme Theme1 = new MudTheme()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = Colors.Blue.Lighten2,
            Secondary = Colors.Teal.Default,
            Tertiary = Colors.DeepOrange.Default,
            AppbarBackground = Colors.Blue.Darken2,
        },
        PaletteDark = new PaletteDark()
        {
            Primary = Colors.Blue.Darken4,
            Secondary = Colors.Teal.Accent4,
            Tertiary = Colors.DeepOrange.Darken2,
            AppbarBackground = Colors.Blue.Darken4,
        },
        LayoutProperties = new LayoutProperties()
        {
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px"
        }
    };
}
