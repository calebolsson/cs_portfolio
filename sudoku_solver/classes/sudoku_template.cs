//holds a 3x3 grid of a template object, used for a block and for the whole puzzle

public class Sudoku_Template
{
    public required string name { get; set; }
    public required string difficulty { get; set; }
    public required List<List<int>> data { get; set; }

};