public class Board
{
    public int Width;
    public int Height;

    public int PlayerCursorX = 0;
    public int PlayerCursorY = 0;

    public bool IsFirstClick = true;

    public int BombCount;
    public int FlagCount;

    public bool[,] BombLocations;
    public bool[,] RevealedLocations;
    public bool[,] FlagLocations;

    public Board(int width, int height, int bombCount)
    {
        Width = width;
        Height = height;
        BombCount = bombCount;

        GenerateBoard();
    }

    private void GenerateBoard()
    {
        BombLocations = new bool[Width, Height];
        FlagLocations = new bool[Width, Height];
        RevealedLocations = new bool[Width, Height];

        var rand = new Random();
        var bombsPlaced = 0;

        // Ensure that we keep placing bombs until we've reached the number
        // requested
        while (bombsPlaced < BombCount)
        {
            var x = rand.Next(Width);
            var y = rand.Next(Height);

            if (!BombLocations[x, y])
            {
                BombLocations[x, y] = true;
                bombsPlaced++;
            }
        }
    }

    public void RevealBombs()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                RevealedLocations[x, y] = RevealedLocations[x, y] || BombLocations[x, y];
            }
        }
    }

    public void Render()
    {
        // Add the characters for each grid position to the buffer before
        // printing it to the terminal all at once
        var buffer = "";
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (x == PlayerCursorX && y == PlayerCursorY)
                {
                    buffer += "\u001b[43m" + GetGridChar(x, y) + "\u001b[0m ";
                }
                else
                {
                    buffer += GetGridChar(x, y) + " ";
                }
            }

            buffer += "\n";
        }

        Console.SetCursorPosition(0, 0);
        Console.Write(buffer);
    }

    public void MoveCursorUp()
    {
        PlayerCursorY = Math.Max(0, PlayerCursorY - 1);
    }

    public void MoveCursorLeft()
    {
        PlayerCursorX = Math.Max(0, PlayerCursorX - 1);
    }

    public void MoveCursorDown()
    {
        PlayerCursorY = Math.Min(Height - 1, PlayerCursorY + 1);
    }

    public void MoveCursorRight()
    {
        PlayerCursorX = Math.Min(Width - 1, PlayerCursorX + 1);
    }

    public void RevealAtCursor()
    {
        if (IsFirstClick)
        {
            IsFirstClick = false;

            while (
                BombLocations[PlayerCursorX, PlayerCursorY]
                || 0 < GetNeighboringBombCount(PlayerCursorX, PlayerCursorY)
            )
            {
                GenerateBoard();
            }
        }

        if (!FlagLocations[PlayerCursorX, PlayerCursorY])
        {
            RevealAt(PlayerCursorX, PlayerCursorY);
        }
    }

    public void ToggleFlagAtCursor()
    {
        if (!IsFirstClick && !RevealedLocations[PlayerCursorX, PlayerCursorY])
        {
            FlagCount += FlagLocations[PlayerCursorX, PlayerCursorY] ? -1 : 1;
            FlagLocations[PlayerCursorX, PlayerCursorY] = !FlagLocations[
                PlayerCursorX,
                PlayerCursorY
            ];
        }
    }

    public void RevealAt(int x, int y)
    {
        if (RevealedLocations[x, y])
        {
            return;
        }

        RevealedLocations[x, y] = true;
        if (0 < GetNeighboringBombCount(x, y))
        {
            return;
        }

        var x0 = Math.Max(0, x - 1);
        var x1 = Math.Min(x + 1, Width - 1);
        var y0 = Math.Max(0, y - 1);
        var y1 = Math.Min(y + 1, Height - 1);

        for (var cursorY = y0; cursorY <= y1; cursorY++)
        {
            for (var cursorX = x0; cursorX <= x1; cursorX++)
            {
                RevealAt(cursorX, cursorY);
            }
        }
    }

    public GameResult GetResult()
    {
        var isWin = true;
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (BombLocations[x, y] && RevealedLocations[x, y])
                {
                    return GameResult.Loss;
                }

                if (BombLocations[x, y] != FlagLocations[x, y])
                {
                    isWin = false;
                }
            }
        }

        return isWin ? GameResult.Win : GameResult.None;
    }

    private string GetGridChar(int x, int y)
    {
        var isRevealed = RevealedLocations[x, y];
        var isFlagged = FlagLocations[x, y];
        var isBomb = BombLocations[x, y];

        if (isFlagged)
        {
            return "\u001b[32mF\u001b[0m";
        }

        if (!isRevealed)
        {
            return "\u001b[90m.\u001b[0m";
        }

        if (isBomb)
        {
            // Return a red X
            return "\u001b[31mX\u001b[0m";
        }

        var bombCount = GetNeighboringBombCount(x, y);

        if (bombCount < 1)
        {
            return " ";
        }

        return bombCount.ToString();
    }

    private int GetNeighboringBombCount(int x, int y)
    {
        // Ensure we don't leave the board's bounds
        var x0 = Math.Max(0, x - 1);
        var x1 = Math.Min(x + 1, Width - 1);
        var y0 = Math.Max(0, y - 1);
        var y1 = Math.Min(y + 1, Height - 1);

        // Count the number of bombs neighboring this square
        var bombCount = 0;
        for (var cursorY = y0; cursorY <= y1; cursorY++)
        {
            for (var cursorX = x0; cursorX <= x1; cursorX++)
            {
                if (BombLocations[cursorX, cursorY])
                {
                    bombCount += 1;
                }
            }
        }

        return bombCount;
    }
}

public enum GameResult
{
    Win,
    Loss,
    None,
}
