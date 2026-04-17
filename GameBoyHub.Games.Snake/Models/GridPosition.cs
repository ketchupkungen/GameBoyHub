/// <summary>
/// Purpose:
/// This file defines a lightweight value type representing a coordinate on
/// the snake game grid. It stores the X (column) and Y (row) indices of a cell.
/// </summary>

namespace GameBoyHub.Games.Snake.Models;

/// <summary>
/// Immutable value type representing a position on the snake game grid.
/// X is the column index (0..Columns-1) and Y is the row index (0..Rows-1).
/// </summary>
public readonly record struct GridPosition(int X, int Y);
