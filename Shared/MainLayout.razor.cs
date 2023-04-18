using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazored.LocalStorage;

namespace WhatsAppLink.Shared;

public partial class MainLayout
{
    //private MudTheme _theme = new();
    private bool _isDarkMode;
    private MudThemeProvider? _mudThemeProvider;

    // https://mudblazor.com/features/colors#material-colors-list-of-material-colors
    MudTheme _theme = new MudTheme()
    {
        Palette = new Palette()
        {
            Primary = MudBlazor.Colors.Green.Default,
            Secondary = MudBlazor.Colors.Green.Accent2,
            AppbarBackground = MudBlazor.Colors.Green.Accent4,
        },
        PaletteDark = new PaletteDark()
        {
            Primary = MudBlazor.Colors.Green.Darken4,
        },

        //LayoutProperties = new LayoutProperties()
        //{
        //    DrawerWidthLeft = "260px",
        //    DrawerWidthRight = "300px"
        //}
    };

    bool _drawerOpen = true;

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    [Inject]
    public ILocalStorageService LocalStorage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (await LocalStorage.ContainKeyAsync("_isDarkMode"))
        {
            string status = await LocalStorage.GetItemAsStringAsync("_isDarkMode");
            if (status != null && status == "True")
            {
                _isDarkMode = true;
                StateChanged();
            }
            else
            {
                _isDarkMode = false;
                StateChanged();
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // set them mode from system preference
        if (firstRender && string.IsNullOrEmpty(LocalStorage.ContainKeyAsync("_isDarkMode").ToString()))
        {
            _isDarkMode = await _mudThemeProvider.GetSystemPreference();
            StateChanged();
        }
    }

    string themModeText = "Switch to Dark Theme";
    void StateChanged()
    {
        if (_isDarkMode)
        {
            themModeText = "Switch to Light Theme";
            StateHasChanged();
        }
        else if (!_isDarkMode)
        {
            themModeText = "Switch to Dark Theme";
            StateHasChanged();
        }
    }

    async void OnThemeModeChange()
    {
        if (_isDarkMode)
        {
            _isDarkMode = false;
            StateChanged();
        }
        else if (!_isDarkMode)
        {
            _isDarkMode = true;
            StateChanged();
        }
        await LocalStorage.SetItemAsStringAsync("_isDarkMode", _isDarkMode.ToString());
    }
}
