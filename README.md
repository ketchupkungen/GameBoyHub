# GameBoyHub

This "GameBoyHub" is just a small experimental application for testing out coding lightweight C#/WPF game modules.
The hub, currently only contains a minimal initial Snake-game.

## Purpose
- Learn coding small games using just C#/.NET 10.0/WPF UI Framework.
- Provide a simple, modular host (IGameModule) to add and run games.

## Projects and structure
- `GameBoyHub.App` — WPF host application and UI (main window, game registry).
- `GameBoyHub.Core` — Shared contracts/interfaces (`IGameModule`).
- `GameBoyHub.Games.Snake` — The snake game module: Snake (UI control, engine, models, assets).

## Key technologies
- .NET 10 (net10.0-windows)
- WPF (UseWPF enabled)
- C# 14

## Prerequisites
- Windows 10/11 with a .NET 10 SDK installed.
- Visual Studio 2022/2025/2026 (or newer) with .NET desktop development workload, or the dotnet CLI.

## How to open and run
1. Using Visual Studio
   - Open the solution (.sln) or open the folder in Visual Studio.
   - Set `GameBoyHub.App` as the startup project.
   - Press F5 to build and run.

2. Using dotnet CLI
   - From repository root run:

     ```powershell
     dotnet build
     dotnet run --project GameBoyHub.App\GameBoyHub.App.csproj
     ```

## How it works
- The app uses a simple `GameRegistry` that returns an in-memory list of `IGameModule` implementations.
- Each game module implements `IGameModule` and returns a `FrameworkElement` (typically a `UserControl`) that the host places into a hosting region (GameHost) in the main window.
- The Snake module shows a small engine (`SnakeGameEngine`) that contains game state and rules. The UI (`SnakeGameControl`) handles rendering and input, and advances the engine using a `DispatcherTimer`.

## Adding a new game (for future refrence)
1. Create a new project (WPF class library) for your game module or add a folder under `GameBoyHub.Games.*`.
2. Implement `IGameModule` (`Id`, `Title`, `Description`, `CreateView()`).
3. Add your visual control (`UserControl`) that returns a `FrameworkElement` from `CreateView()`.
4. Add the new module to `GameRegistry.GetGames()` or implement a discovery mechanism.

## Play controls (Snake)
- Arrow keys or WASD to move the snake.
- R or Enter to restart after game over.
- The UI includes a simple Start screen with Play and placeholder sliders for difficulty and grid size (sliders are currently functional).

## Notes about the Snake implementation
- The engine is separated from the UI (`SnakeGameEngine` contains rules/state). The UI only renders and handles input.
- Body sprites and head sprites are used; corner, straight and tail sprites are selected and rotated to match snake orientation. Dead state uses a special dead head sprite only.

## Future plans
- Add fullscreen support.
- Potentially separate the games list into its own launcher window and launch games in separate windows.
- For Snake: experiment with animations, improved graphics, smooth movement and corner/tail artwork.
- Add saving and loading of game state as JSON; later add a database/cloud-backed store.
- Add additional games (Pong, and more ambitious platformer-style games such as Metroid/Super Mario-ish prototypes).

## License
- This project is an experimental sample.