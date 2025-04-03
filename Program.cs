// See https://aka.ms/new-console-template for more information
var board = new Board(8, 8, 8);

Console.CursorVisible = false;
Console.Clear();

while (true)
{
    board.Render();
    Console.WriteLine(
        $"Move: ←↑↓→, Reveal: space, Flag: <f>, Quit: <q>, {board.FlagCount} / {board.BombCount}"
    );

    switch (board.GetResult())
    {
        case GameResult.Win:
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You won!");
            Console.ResetColor();
            return;

        case GameResult.Loss:
            board.RevealBombs();
            board.Render();
            Console.WriteLine(
                $"Move: ←↑↓→, Reveal: space, Flag: <f>, Quit: <q>, {board.FlagCount} / {board.BombCount}"
            );
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("You lost.");
            Console.ResetColor();
            return;
    }

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
