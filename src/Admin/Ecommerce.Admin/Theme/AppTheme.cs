
using MudBlazor;

namespace Ecommerce.Admin.Theme;

public static class AppTheme
{
    public static readonly MudTheme Theme = new()
    {
        PaletteLight = new PaletteLight
        {
            // Brand colors
            Primary = "#F59E0B",
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#374151",
            SecondaryContrastText = "#FFFFFF",

            // Backgrounds
            Background = "#F8F9FA",
            Surface = "#FFFFFF",

            // AppBar
            AppbarBackground = "#FFFFFF",
            AppbarText = "#171717",

            // Drawer
            DrawerBackground = "#FFFFFF",
            DrawerText = "#374151",

            // Typography colors
            TextPrimary = "#171717",
            TextSecondary = "#737373",

            // Borders
            Divider = "#EAEAEA",

            // Status colors
            Success = "#15803D",
            Warning = "#D97706",
            Error = "#DC2626",
            Info = "#2563EB",

            // Action colors
            ActionDefault = "#737373",
            ActionDisabled = "#A3A3A3",
            ActionDisabledBackground = "#F5F5F5"
        },

        LayoutProperties = new LayoutProperties
        {
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "260px"
        },

        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[]
                {
                    "Inter",
                    "Segoe UI",
                    "sans-serif"
                },
                FontSize = "0.875rem",
                FontWeight = "400",
                LineHeight = "1.5"
            },

            H4 = new H4Typography
            {
                FontFamily = new[]
                {
                    "Inter",
                    "Segoe UI",
                    "sans-serif"
                },
                FontSize = "1.5rem",
                FontWeight = "650",
                LineHeight = "1.3"
            },

            H5 = new H5Typography
            {
                FontWeight = "600"
            },

            H6 = new H6Typography
            {
                FontWeight = "600"
            },

            Button = new ButtonTypography
            {
                FontFamily = new[]
                {
                    "Inter",
                    "Segoe UI",
                    "sans-serif"
                },
                FontWeight = "500",
                TextTransform = "none"
            }
        },

        ZIndex = new ZIndex
        {
            Drawer = 1100,
            AppBar = 1200,
            Dialog = 1300,
            Popover = 1400,
            Snackbar = 1500,
            Tooltip = 1600
        }
    };
}
