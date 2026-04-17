/// <summary>
/// Purpose: 
/// This file encapsulates the game logic for the Snake game. Responsible for
/// storing the snake's body, moving the snake on each step, handling collisions,
/// spawning food and tracking score and game state.
/// </summary>

using GameBoyHub.Games.Snake.Models;

namespace GameBoyHub.Games.Snake;

/// <summary>
/// Core engine that runs the snake game rules. This class is intentionally
/// lightweight and does not contain any UI code; it only exposes state that
/// the UI layer can render.
/// </summary>
public sealed class SnakeGameEngine
{
    // Source of randomness used for food placement.
    private readonly Random _random = new();

    // Internal list representing the snake body; head is at index 0.
    private List<GridPosition> _snake = new();

    // Prevent more than one direction change between steps (avoids instant reversals).
    private bool _directionChanged;

    // Number of columns in the logical game grid.
    public int Columns { get; } = 20;

    // Number of rows in the logical game grid.
    public int Rows { get; } = 20;

    // Read-only view of the snake body positions.
    public IReadOnlyList<GridPosition> Snake => _snake;

    // Location of the current food item on the grid.
    public GridPosition Food { get; private set; } = new GridPosition(0, 0);

    // Current direction of movement for the snake.
    public SnakeDirection Direction { get; private set; } = SnakeDirection.Right;

    // Whether the game is over (collision occurred).
    public bool IsGameOver { get; private set; }

    // Current player score (number of food items eaten).
    public int Score { get; private set; }

    // Creates a new engine instance and initializes state.
    public SnakeGameEngine()
    {
        Reset();
    }

    // Resets the game to its initial state with a short snake and fresh food.
    public void Reset()
    {
        _snake = new List<GridPosition>
        {
            new(5, 10),
            new(4, 10),
            new(3, 10)
        };

        Direction = SnakeDirection.Right;
        _directionChanged = false;
        Score = 0;
        IsGameOver = false;
        SpawnFood();
    }

    /// <summary>
    /// Attempts to change the snake's direction. Multiple calls between two
    /// Step executions are ignored to avoid illegal instant reversals.
    /// </summary>
    public void ChangeDirection(SnakeDirection newDirection)
    {
        if (_directionChanged)
        {
            return; // only one direction change allowed between steps
        }

        if (IsOpposite(Direction, newDirection))
        {
            return;
        }

        Direction = newDirection;
        _directionChanged = true;
    }

    /// <summary>
    /// Advances the game by one step: computes the next head position,
    /// checks collisions, moves the snake, and handles eating food.
    /// </summary>
    public void Step()
    {
        if (IsGameOver)
        {
            return;
        }

        // Allow direction changes again for the next step
        _directionChanged = false;

        GridPosition currentHead = _snake[0];
        GridPosition nextHead = Direction switch
        {
            SnakeDirection.Up => new GridPosition(currentHead.X, currentHead.Y - 1),
            SnakeDirection.Right => new GridPosition(currentHead.X + 1, currentHead.Y),
            SnakeDirection.Down => new GridPosition(currentHead.X, currentHead.Y + 1),
            SnakeDirection.Left => new GridPosition(currentHead.X - 1, currentHead.Y),
            _ => currentHead
        };

        bool hitsWall = nextHead.X < 0 || nextHead.X >= Columns || nextHead.Y < 0 || nextHead.Y >= Rows;
        bool hitsSelf = _snake.Contains(nextHead);

        if (hitsWall || hitsSelf)
        {
            IsGameOver = true;
            return;
        }

        _snake.Insert(0, nextHead);

        if (nextHead == Food)
        {
            Score++;
            SpawnFood();
        }
        else
        {
            _snake.RemoveAt(_snake.Count - 1);
        }
    }

    // Choose a random cell not occupied by the snake.
    private void SpawnFood()
    {
        GridPosition candidate;

        do
        {
            candidate = new GridPosition(_random.Next(Columns), _random.Next(Rows));
        }
        while (_snake.Contains(candidate));

        Food = candidate;
    }

    // Helper that checks whether two directions are exact opposites.
    private static bool IsOpposite(SnakeDirection current, SnakeDirection next)
    {
        return (current == SnakeDirection.Up && next == SnakeDirection.Down)
            || (current == SnakeDirection.Down && next == SnakeDirection.Up)
            || (current == SnakeDirection.Left && next == SnakeDirection.Right)
            || (current == SnakeDirection.Right && next == SnakeDirection.Left);
    }
}
