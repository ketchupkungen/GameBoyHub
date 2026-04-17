using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameBoyHub.Core.Interfaces;

namespace GameBoyHub.App;

public partial class GameWindow : Window
{
    private readonly IGameModule _module;
    private readonly FrameworkElement _view;
    private bool _isFullScreen;

    public GameWindow(IGameModule module)
    {
        InitializeComponent();

        _module = module;
        _view = _module.CreateView();

        // Let module provide its own menu if available
        Menu? menu = _module.CreateMenu(_view);
        if (menu is not null)
        {
            MenuHost.Content = menu;
        }

        GameHost.Content = _view;

        // Ensure this window is owned by the main application window so it appears
        // above the launcher and is centered when opened.
        if (Application.Current?.MainWindow is Window mw && mw != this)
        {
            Owner = mw;
        }

        // Do not auto-start the game here. The game's view (UserControl)
        // controls when it begins (Start button). Previously we invoked
        // StartGame reflexively which caused the game to run behind the
        // start menu; remove that behavior to keep the start panel modal.

        // Keyboard shortcut for fullscreen
        KeyDown += GameWindow_KeyDown;
    }

    private static void TryInvokeStartGame(FrameworkElement view)
    {
        try
        {
            var mi = view.GetType().GetMethod("StartGame", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            mi?.Invoke(view, null);
        }
        catch
        {
            // Swallow any exceptions; starting the game is a convenience.
        }
    }

    private void GameWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F11)
        {
            ToggleFullScreen();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape && _isFullScreen)
        {
            ToggleFullScreen();
            e.Handled = true;
        }
    }

    public void ToggleFullScreen()
    {
        if (!_isFullScreen)
        {
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            Topmost = true;
            _isFullScreen = true;
        }
        else
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Normal;
            Topmost = false;
            _isFullScreen = false;
        }
    }
}
