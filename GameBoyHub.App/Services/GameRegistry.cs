/// <summary>
/// Purpose:
/// This file provides a simple registry of available game modules. It is a
/// centralized place for the host to obtain IGameModule instances. In a
/// production system this could be replaced with a discovery mechanism.
/// </summary>

using GameBoyHub.Core.Interfaces;
using GameBoyHub.Games.Snake;

namespace GameBoyHub.App.Services;

/// <summary>
/// Simple registry that returns the list of available IGameModule instances
/// the host application can present to the user.
/// </summary>
public static class GameRegistry
{
    // Returns a list of available game modules. Currently hard-coded.
    public static List<IGameModule> GetGames()
    {
        return new List<IGameModule>
        {
            new SnakeGameModule()
        };
    }
}
