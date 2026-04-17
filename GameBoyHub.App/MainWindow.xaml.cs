/// <summary>
/// Purpose:
/// This file hosts the main window for the application. It wires the game
/// registry to the UI and displays the selected game's view inside the
/// GameHost region. The code-behind handles selection changes from the
/// games list and swaps the displayed module view.
/// </summary>

using System.Windows;
using GameBoyHub.App.Services;
using GameBoyHub.Core.Interfaces;

namespace GameBoyHub.App;

/// <summary>
/// Main application window. Displays a list of available IGameModule instances
/// and shows the selected game's view inside the GameHost content area.
/// </summary>
public partial class MainWindow : Window
{
    // Backing list of available game modules displayed in the UI.
    private readonly List<IGameModule> _games;

    /// <summary>
    /// Initializes the main window, loads available games from the registry
    /// and binds them to the GamesList UI element.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        _games = GameRegistry.GetGames();
        GamesList.ItemsSource = _games;
    }

    // Open the selected game in its own window when the user double-clicks an entry.
    private void GamesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (GamesList.SelectedItem is IGameModule game)
        {
            var gameWindow = new GameWindow(game);
            gameWindow.Title = game.Title;
            gameWindow.Show();
        }
    }
}
