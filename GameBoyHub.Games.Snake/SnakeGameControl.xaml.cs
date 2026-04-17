using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using GameBoyHub.Games.Snake.Models;

/// <summary>
/// Purpose:
/// This file contains the code-behind for the SnakeGameControl UserControl.
/// It handles user input, rendering the game canvas, and wiring the
/// DispatcherTimer to the SnakeGameEngine. The control is responsible for
/// creating and placing visual elements but contains no game rule logic.
/// </summary>

namespace GameBoyHub.Games.Snake;

/// <summary>
/// UserControl that hosts the Snake game UI. Responsible for wiring the
/// DispatcherTimer, handling keyboard input and rendering the game state
/// produced by <see cref="SnakeGameEngine"/>.
/// </summary>
public partial class SnakeGameControl : UserControl
{
    private const int CellSize = 24;

    private readonly SnakeGameEngine _engine = new();
    private readonly DispatcherTimer _gameTimer;
    private readonly BitmapImage? _headAsset;
    private readonly BitmapImage? _bodyAsset;
    private readonly BitmapImage? _deadHeadAsset;
    private readonly BitmapImage? _bodyTurnAsset;
    private readonly BitmapImage? _bodyButtAsset;
    private readonly BitmapImage? _foodAsset;

    public SnakeGameControl()
    {
        InitializeComponent();

        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(140)
        };
        _gameTimer.Tick += GameTimer_Tick;

        _headAsset = TryLoadBitmap("Assets/head.png");
        _bodyAsset = TryLoadBitmap("Assets/body.png");
        _deadHeadAsset = TryLoadBitmap("Assets/head_dead.png");
        _bodyTurnAsset = TryLoadBitmap("Assets/body_turn.png");
        _bodyButtAsset = TryLoadBitmap("Assets/body_butt.png");
        _foodAsset = TryLoadBitmap("Assets/food.png");
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        Focus();
        // Do not auto-start the game; show the start panel instead.
        StartPanel.Visibility = Visibility.Visible;
    }

    private void UserControl_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
            case Key.W:
                _engine.ChangeDirection(SnakeDirection.Up);
                e.Handled = true;
                break;

            case Key.Right:
            case Key.D:
                _engine.ChangeDirection(SnakeDirection.Right);
                e.Handled = true;
                break;

            case Key.Down:
            case Key.S:
                _engine.ChangeDirection(SnakeDirection.Down);
                e.Handled = true;
                break;

            case Key.Left:
            case Key.A:
                _engine.ChangeDirection(SnakeDirection.Left);
                e.Handled = true;
                break;

            case Key.R:
                if (_engine.IsGameOver)
                {
                    StartNewGame();
                }
                e.Handled = true;
                break;

            case Key.Enter:
                if (_engine.IsGameOver)
                {
                    StartNewGame();
                }
                e.Handled = true;
                break;
        }
    }

    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: use DifficultySlider and GridSizeSlider values to configure engine
        StartPanel.Visibility = Visibility.Collapsed;
        StartNewGame();
        Focus();
    }

    private void RestartButton_Click(object sender, RoutedEventArgs e)
    {
        StartNewGame();
        Focus();
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        _engine.Step();
        Render();

        if (_engine.IsGameOver)
        {
            _gameTimer.Stop();
            OverlayScoreTextBlock.Text = $"Final score: {_engine.Score}";
            OverlayPanel.Visibility = Visibility.Visible;
            // ensure the control has focus so keyboard restart works
            Focus();
        }
    }

    private void StartNewGame()
    {
        _engine.Reset();
        OverlayPanel.Visibility = Visibility.Collapsed;
        Render();
        _gameTimer.Start();
    }

    private void Render()
    {
        GameCanvas.Children.Clear();
        DrawGrid();
        DrawFood();
        DrawSnake();

        ScoreTextBlock.Text = _engine.Score.ToString();
    }

    private void DrawGrid()
    {
        for (int row = 0; row < _engine.Rows; row++)
        {
            for (int col = 0; col < _engine.Columns; col++)
            {
                Rectangle tile = new()
                {
                    Width = CellSize,
                    Height = CellSize,
                    Stroke = new SolidColorBrush(Color.FromRgb(84, 84, 122)),
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(Color.FromRgb(47, 47, 74))
                };

                Canvas.SetLeft(tile, col * CellSize);
                Canvas.SetTop(tile, row * CellSize);
                GameCanvas.Children.Add(tile);
            }
        }
    }

    private void DrawFood()
    {
        AddSpriteOrFallback(_engine.Food, _foodAsset, CreateFoodFallback());
    }

    private void DrawSnake()
    {
        for (int index = 0; index < _engine.Snake.Count; index++)
        {
            GridPosition part = _engine.Snake[index];
            bool isHead = index == 0;

            if (isHead)
            {
                // Use a special dead head sprite when game is over; fall back to the regular head asset
                BitmapImage? headSprite = _engine.IsGameOver ? (_deadHeadAsset ?? _headAsset) : _headAsset;
                FrameworkElement headElement = CreateSnakePartElement(headSprite, CreateHeadFallback(_engine.IsGameOver));

                headElement.RenderTransformOrigin = new Point(0.5, 0.5);
                headElement.RenderTransform = new RotateTransform(GetHeadRotation(_engine.Direction));
                PlaceElement(headElement, part);
                GameCanvas.Children.Add(headElement);
            }
            else
            {
                AddBodySprite(part, index);
            }
        }
    }

    private void AddSpriteOrFallback(GridPosition position, BitmapImage? sprite, FrameworkElement fallback)
    {
        FrameworkElement element = CreateSnakePartElement(sprite, fallback);
        PlaceElement(element, position);
        GameCanvas.Children.Add(element);
    }

    // Adds a body sprite (straight, corner or tail) and rotates it according to
    // the neighboring segments so the visual matches the snake's shape.
    private void AddBodySprite(GridPosition position, int index)
    {
        BitmapImage? sprite = null;
        FrameworkElement fallback = CreateBodyFallback(_engine.IsGameOver);
        double rotation = 0;

        int count = _engine.Snake.Count;

        // Tail segment: use butt asset and orient it pointing away from the previous segment
        if (index == count - 1)
        {
            GridPosition prev = _engine.Snake[index - 1];
            int dx = prev.X - position.X;
            int dy = prev.Y - position.Y;
            SnakeDirection dirPrev = DirectionFromDelta(dx, dy);

            // Always use alive butt asset for tail; dead state only affects head visual
            sprite = _bodyButtAsset;
            rotation = GetHeadRotation(dirPrev);
        }
        else
        {
            // Interior segment: determine relationship between previous (towards head)
            // and next (towards tail) segments.
            GridPosition prev = _engine.Snake[index - 1];
            GridPosition next = _engine.Snake[index + 1];

            int dxPrev = prev.X - position.X;
            int dyPrev = prev.Y - position.Y;
            int dxNext = next.X - position.X;
            int dyNext = next.Y - position.Y;

            SnakeDirection dirPrev = DirectionFromDelta(dxPrev, dyPrev);
            SnakeDirection dirNext = DirectionFromDelta(dxNext, dyNext);

            if (DirectionsAreOpposite(dirPrev, dirNext))
            {
                // Straight segment (prev and next point in opposite directions)
                sprite = _bodyAsset;
                rotation = GetHeadRotation(dirPrev);
            }
            else
            {
                // Corner segment (prev and next are perpendicular)
                sprite = _bodyTurnAsset;
                rotation = GetCornerRotation(dirPrev, dirNext);
            }
        }

        FrameworkElement element = CreateSnakePartElement(sprite, fallback);

        if (element is Image)
        {
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            element.RenderTransform = new RotateTransform(rotation);
        }

        PlaceElement(element, position);
        GameCanvas.Children.Add(element);
    }

    private static SnakeDirection DirectionFromDelta(int dx, int dy)
    {
        if (dx == 1) return SnakeDirection.Right;
        if (dx == -1) return SnakeDirection.Left;
        if (dy == 1) return SnakeDirection.Down;
        return SnakeDirection.Up;
    }

    private static double GetCornerRotation(SnakeDirection a, SnakeDirection b)
    {
        // Normalize unordered pair to match base corner image that connects Up->Right (0°)
        // {Up, Right} => 0
        if ((a == SnakeDirection.Up && b == SnakeDirection.Right) || (a == SnakeDirection.Right && b == SnakeDirection.Up))
            return 0;

        // {Right, Down} => 90
        if ((a == SnakeDirection.Right && b == SnakeDirection.Down) || (a == SnakeDirection.Down && b == SnakeDirection.Right))
            return 90;

        // {Down, Left} => 180
        if ((a == SnakeDirection.Down && b == SnakeDirection.Left) || (a == SnakeDirection.Left && b == SnakeDirection.Down))
            return 180;

        // {Left, Up} => 270
        if ((a == SnakeDirection.Left && b == SnakeDirection.Up) || (a == SnakeDirection.Up && b == SnakeDirection.Left))
            return 270;

        return 0;
    }

    private static bool DirectionsAreOpposite(SnakeDirection a, SnakeDirection b)
    {
        return (a == SnakeDirection.Up && b == SnakeDirection.Down)
            || (a == SnakeDirection.Down && b == SnakeDirection.Up)
            || (a == SnakeDirection.Left && b == SnakeDirection.Right)
            || (a == SnakeDirection.Right && b == SnakeDirection.Left);
    }

    private FrameworkElement CreateSnakePartElement(BitmapImage? sprite, FrameworkElement fallback)
    {
        if (sprite is not null)
        {
            return new Image
            {
                Width = CellSize,
                Height = CellSize,
                Source = sprite,
                Stretch = Stretch.Fill,
                SnapsToDevicePixels = true
            };
        }

        return fallback;
    }

    private static void PlaceElement(FrameworkElement element, GridPosition position)
    {
        Canvas.SetLeft(element, position.X * CellSize);
        Canvas.SetTop(element, position.Y * CellSize);
    }

    private static FrameworkElement CreateFoodFallback()
    {
        return new Ellipse
        {
            Width = CellSize - 6,
            Height = CellSize - 6,
            Fill = new SolidColorBrush(Color.FromRgb(248, 113, 113)),
            Margin = new Thickness(3)
        };
    }

    private static FrameworkElement CreateBodyFallback(bool isDead)
    {
        return new Rectangle
        {
            Width = CellSize - 4,
            Height = CellSize - 4,
            RadiusX = 4,
            RadiusY = 4,
            Fill = new SolidColorBrush(isDead ? Color.FromRgb(140, 140, 140) : Color.FromRgb(134, 239, 172)),
            Margin = new Thickness(2)
        };
    }

    private static FrameworkElement CreateHeadFallback(bool isDead)
    {
        Grid grid = new()
        {
            Width = CellSize,
            Height = CellSize
        };

        Rectangle baseRect = new()
        {
            Width = CellSize - 2,
            Height = CellSize - 2,
            RadiusX = 5,
            RadiusY = 5,
            Fill = new SolidColorBrush(isDead ? Color.FromRgb(140, 140, 140) : Color.FromRgb(134, 239, 172))
        };

        grid.Children.Add(baseRect);
        return grid;
    }

    private static double GetHeadRotation(SnakeDirection direction)
    {
        return direction switch
        {
            SnakeDirection.Up => 270,
            SnakeDirection.Right => 0,
            SnakeDirection.Down => 90,
            SnakeDirection.Left => 180,
            _ => 0
        };
    }

    private static BitmapImage? TryLoadBitmap(string relativePath)
    {
        try
        {
            return new BitmapImage(new Uri($"pack://application:,,,/GameBoyHub.Games.Snake;component/{relativePath}", UriKind.Absolute));
        }
        catch
        {
            return null;
        }
    }
}
