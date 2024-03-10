using System;
using System.Text.Json;

Console.WriteLine("Loading Puzzle...");
string text = File.ReadAllText(@"./resources/puzzle03.json");
Sudoku_Template temp;
temp = JsonSerializer.Deserialize<Sudoku_Template>(text)!;
if (temp == null) { return; }
Sudoku puzzle = new(temp.name, temp.difficulty, temp.data);
Console.WriteLine("Puzzle Loaded!");
Console.WriteLine("Name: " + puzzle.Name);
Console.WriteLine("Difficulty: " + puzzle.Difficulty);
DisplayHandler ui = new();
int prevent_infinite = 5;
while (puzzle.Is_unsolved() && prevent_infinite-- > 0)
{
    ui.DisplayGrid(puzzle.Cells);
    for (int y = 0; y < 9; y++)
        for (int x = 0; x < 9; x++)
            puzzle.Solve(x, y);
}

ui.DisplayGrid(puzzle.Cells);
if (prevent_infinite < 0)
{
    Console.WriteLine("Prevented infinite loop from unsolved puzzle ({0}/81 solved)", puzzle.CellsSolved);
    Console.WriteLine("If this was premature, raise the prevent_infinite threshold");
}