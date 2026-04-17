/// <summary>
/// Purpose:
/// This file exposes the Snake game as an IGameModule so the application can
/// discover and host the Snake game's view. Provides simple metadata used in
/// the games list UI.
/// </summary>

using System.Windows;
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
}
