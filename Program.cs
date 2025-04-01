// See https://aka.ms/new-console-template for more information
var board = new Board(32, 32);

Console.CursorVisible = false;
Console.Clear();

// board.RevealEverything();
board.PlaceBombs(128);

while (true)
{
    board.Render();

    Console.WriteLine("Move: ←↑↓→, Reveal: space, Flag: <f>, Quit: <q>");

    switch (Console.ReadKey(true).Key)
    {
        case ConsoleKey.UpArrow:
            board.MoveCursorUp();
            break;

        case ConsoleKey.LeftArrow:
            board.MoveCursorLeft();
            break;

        case ConsoleKey.DownArrow:
            board.MoveCursorDown();
            break;

        case ConsoleKey.RightArrow:
            board.MoveCursorRight();
            break;

        case ConsoleKey.Spacebar:
            board.RevealAtCursor();
            break;

        case ConsoleKey.F:
            board.ToggleFlagAtCursor();
            break;

        case ConsoleKey.Q:
            Console.CursorVisible = true;
            return;
    }
}
