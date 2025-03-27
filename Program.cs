// See https://aka.ms/new-console-template for more information
var board = new Board(16, 16);

Console.Clear();

board.RevealEverything();
board.PlaceBombs(32);
board.Render();

Console.ReadKey(true);
