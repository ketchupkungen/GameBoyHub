/// <summary>
/// Purpose:
/// This file exposes the Snake game as an IGameModule so the application can
/// discover and host the Snake game's view. Provides simple metadata used in
/// the games list UI.
/// </summary>

using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using GameBoyHub.Core.Interfaces;
namespace GameBoyHub.Games.Snake;

/// <summary>
/// Game module wrapper for the Snake game. Constructs the SnakeGameControl when requested.
/// </summary>
public class SnakeGameModule : IGameModule
{
    // Unique Id and the title/description in UI for the game
    public string Id => "snake";

    public string Title => "Snake";

    public string Description => "Classic snake game.";

    // Creates the view (UserControl) that hosts the Snake game UI.
    public FrameworkElement CreateView()
    {
        return new SnakeGameControl();
    }

    public Menu? CreateMenu(FrameworkElement view)
    {
        // Create a simple game-specific menu bar. Wire commands to the view when possible.
        var menu = new Menu();
        var gameMenu = new MenuItem { Header = "Game" };

        var playItem = new MenuItem { Header = "Play" };
        playItem.Click += (_, _) =>
        {
            if (view is SnakeGameControl ctrl) ctrl.StartGame();
        };

        var restartItem = new MenuItem { Header = "Restart" };
        restartItem.Click += (_, _) =>
        {
            if (view is SnakeGameControl ctrl) ctrl.RestartGame();
        };

        var pauseItem = new MenuItem { Header = "Pause", IsCheckable = true };
        pauseItem.Click += (_, _) =>
        {
            if (view is SnakeGameControl ctrl)
            {
                ctrl.TogglePause();
            }
        };

        var loadItem = new MenuItem { Header = "Load" };
        loadItem.Click += (_, _) => { if (view is SnakeGameControl ctrl) ctrl.LoadGame(); };

        var saveItem = new MenuItem { Header = "Save" };
        saveItem.Click += (_, _) => { if (view is SnakeGameControl ctrl) ctrl.SaveGame(); };

        var highscoreItem = new MenuItem { Header = "Highscore" };
        highscoreItem.Click += (_, _) => { if (view is SnakeGameControl ctrl) ctrl.ShowHighscore(); };

        var settingsItem = new MenuItem { Header = "Settings" };
        settingsItem.Click += (_, _) => { if (view is SnakeGameControl ctrl) ctrl.OpenSettings(); };

        var exitToMenuItem = new MenuItem { Header = "Exit to menu" };
        exitToMenuItem.Click += (_, _) =>
        {
            // Close the game window and return to the launcher
            Window.GetWindow(view)?.Close();
        };

        var exitAppItem = new MenuItem { Header = "Exit GameBoyHub" };
        exitAppItem.Click += (_, _) =>
        {
            // Exit the entire application
            Application.Current?.Shutdown();
        };

        gameMenu.Items.Add(playItem);
        gameMenu.Items.Add(restartItem);
        gameMenu.Items.Add(pauseItem);
        gameMenu.Items.Add(new Separator());
        gameMenu.Items.Add(loadItem);
        gameMenu.Items.Add(saveItem);
        gameMenu.Items.Add(new Separator());
        gameMenu.Items.Add(highscoreItem);
        gameMenu.Items.Add(settingsItem);
        gameMenu.Items.Add(new Separator());
        gameMenu.Items.Add(exitToMenuItem);
        gameMenu.Items.Add(exitAppItem);

        var viewMenu = new MenuItem { Header = "View" };
        var fullscreenItem = new MenuItem { Header = "Toggle Fullscreen (F11)" };
        fullscreenItem.Click += (_, _) =>
        {
            // Avoid referencing the App assembly from the game project. Use reflection
            // to locate a ToggleFullScreen method on the host window if present.
            var win = Window.GetWindow(view);
            if (win is not null)
            {
                MethodInfo? mi = win.GetType().GetMethod("ToggleFullScreen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                mi?.Invoke(win, null);
            }
        };
        viewMenu.Items.Add(fullscreenItem);

        menu.Items.Add(gameMenu);
        menu.Items.Add(viewMenu);

        return menu;
    }
}
