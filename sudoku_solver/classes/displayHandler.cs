/// <summary>
/// A class that stores the structure of the display
/// </summary>
public class DisplayHandler
{
    private string top = "╔═══╤═══╤═══╦═══╤═══╤═══╦═══╤═══╤═══╗\n";
    private string mid = "╠═══╪═══╪═══╬═══╪═══╪═══╬═══╪═══╪═══╣\n";
    private string sub = "╟───┼───┼───╫───┼───┼───╫───┼───┼───╢\n";
    private string bot = "╚═══╧═══╧═══╩═══╧═══╧═══╩═══╧═══╧═══╝";
    /// <summary>
    /// Takes a list of cells and formats it to fit inside of the puzzle frame
    /// </summary>
    /// <param name="row">The list of cells from the nth row of the puzzle array</param>
    /// <returns>The formatted string of the nth row of the puzzle</returns>
    private static string DisplayRow(List<Cell> row)
    {
        string thisRow = "║ ";
        for (int i = 0; i < 9; i++)
        {
            if (row[i].solved()) thisRow += row[i].answer();
            else thisRow += ' ';
            if ((i + 1) % 3 == 0) thisRow += " ║ ";
            else thisRow += " │ ";
        }
        return thisRow + "\n";
    }
    /// <summary>
    /// Takes the data of the entire puzzle, applies the frame format, and prints the puzzle
    /// </summary>
    /// <param name="data">The full puzzle array, to be read in rows</param>
    public void DisplayGrid(List<List<Cell>> data)
    {
        string theGrid = top;
        for (int i = 0; i < 9; i++)
        {
            theGrid += DisplayRow(data[i]);
            if (i < 8)
            {
                if ((i + 1) % 3 == 0) theGrid += mid;
                else theGrid += sub;
            }
        }
        Console.WriteLine(theGrid += bot);
    }
}