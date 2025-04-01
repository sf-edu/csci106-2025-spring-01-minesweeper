public class Board
{
    public int Width;
    public int Height;

    public int PlayerCursorX = 0;
    public int PlayerCursorY = 0;

    public bool[,] BombLocations;
    public bool[,] RevealedLocations;
    public bool[,] FlagLocations;

    public Board(int width, int height)
    {
        Width = width;
        Height = height;

        BombLocations = new bool[width, height];
        FlagLocations = new bool[width, height];
        RevealedLocations = new bool[width, height];
    }

    public void PlaceBombs(int bombsRequested)
    {
        var rand = new Random();
        var bombsPlaced = 0;

        // Ensure that we keep placing bombs until we've reached the number
        // requested
        while (bombsPlaced < bombsRequested)
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

    public void RevealEverything()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                RevealedLocations[x, y] = true;
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
        if (!FlagLocations[PlayerCursorX, PlayerCursorY])
        {
            RevealAt(PlayerCursorX, PlayerCursorY);
        }
    }

    public void ToggleFlagAtCursor()
    {
        if (!RevealedLocations[PlayerCursorX, PlayerCursorY])
        {
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
            return ".";
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
