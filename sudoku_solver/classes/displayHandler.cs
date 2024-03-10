public class DisplayHandler
{
    private string top = "╔═══╤═══╤═══╦═══╤═══╤═══╦═══╤═══╤═══╗\n";
    private string mid = "╠═══╪═══╪═══╬═══╪═══╪═══╬═══╪═══╪═══╣\n";
    private string sub = "╟───┼───┼───╫───┼───┼───╫───┼───┼───╢\n";
    private string bot = "╚═══╧═══╧═══╩═══╧═══╧═══╩═══╧═══╧═══╝";
    private string displayRow(List<Cell> row)
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
    public void displayGrid(List<List<Cell>> data)
    {
        string theGrid = top;
        for (int i = 0; i < 9; i++)
        {
            theGrid += displayRow(data[i]);
            if (i < 8)
            {
                if ((i + 1) % 3 == 0) theGrid += mid;
                else theGrid += sub;
            }
        }
        Console.WriteLine(theGrid += bot);
    }
}