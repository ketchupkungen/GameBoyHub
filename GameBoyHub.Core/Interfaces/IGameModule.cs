/// <summary>
/// Purpose:
/// This file defines the contract that game modules must implement in order
/// to be hosted inside the main application. Each module exposes metadata
/// and a WPF view that the host can display.
/// </summary>

using System.Windows;

namespace GameBoyHub.Core.Interfaces;

/// <summary>
/// Contract for a game module that can be discovered and hosted by the application.
/// Implementations should provide identifying metadata and a method to create
/// the module's UI view.
/// </summary>
public interface IGameModule
{
    // Unique identifier for the module (used for routing or persistence).
    string Id { get; }

    // Game title of the module (diplayed in the UI).
    string Title { get; }

    // Short description (diplayed in the UI).
    string Description { get; }

    /// <summary>
    /// Creates and returns the WPF view (FrameworkElement) that hosts the game UI.
    /// The host is responsible for placing this element into the visual tree.
    /// </summary>
    FrameworkElement CreateView();
}
