using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
/// <summary>
/// Class <c>Sudoku</c> models a sudoku puzzle
/// </summary>
public class Sudoku
{
    public string Name { get; set; }
    public string Difficulty { get; set; }
    public List<List<Cell>> Cells = [[], [], [], [], [], [], [], [], []];
    public int CellsSolved { get; set; }

    public DisplayHandler ui = new();

    /// <summary>
    /// Constructs a Sudoku object by converting a list of type int into a list of type Cell
    /// </summary>
    /// <param name="name">The title of the puzzle</param>
    /// <param name="difficulty">One of five ranks of difficulty: Easy, Medium, Hard, Expert, Master</param>
    /// <param name="src">The 2D list of int to be read into the puzzle's <c>Cell</c> array</param>
    public Sudoku(string name, string difficulty, List<List<int>> src)
    {
        this.Name = name;
        this.Difficulty = difficulty;
        this.CellsSolved = 0;
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                Cells[i].Add(new Cell(src[i][j]));
                if (Cells[i][j].solved()) CellsSolved++;
                //Console.WriteLine("[{0},{1}] ({2}) {3}/81", i, j, cells[i][j].answer(), cells_solved);
            }
    }
    /// <summary>
    /// A method to check on the completion state of the puzzle.
    /// </summary>
    /// <returns>True if the puzzle has not been solved yet</returns>
    public bool Is_unsolved()
    {
        if (CellsSolved < 81) return true;
        return false;
    }
    /// <summary>
    /// A request to solve the Cell at location (x,y)
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    public void Solve(int x, int y)
    {
        if (Cells[x][y].solved()) return;
        else if (UpdateCell(x, y)) Explode(x, y);
        return;
    }
    /// <summary>
    /// A request to change the contents of the Cell based on Square/Row/Column elimination
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    /// <returns>True if the Cell was in an unsolved state and is now in a solved states</returns>
    private bool UpdateCell(int x, int y)
    {
        List<List<Cell>> ranges = GetRanges(x, y);
        foreach (List<Cell> range in ranges)
            Cells[x][y].remove(range.Distinct().ToList());
        if (Cells[x][y].solved()) return true;
        //Console.WriteLine("[" + x + "," + y + "] (" + cells[x][y].debug_data() + ")");
        Scan(x, y);
        return false;
    }
    /// <summary>
    /// A command from a recently solved Cell to go solve cells that share a Square/Row/Column with it
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    private void Explode(int x, int y)
    {
        if (this.Is_unsolved())
        {
            CellsSolved++;
            //ui.displayGrid(cells);
            Console.WriteLine("[" + x + "," + y + "] Solved! " + Cells[x][y].answer() + " {" + CellsSolved + "/81}");
            //Console.WriteLine("Checking Square of [" + x + "," + y + "]");
            updateSq(x, y);
            //Console.WriteLine("Checking Row of [" + x + "," + y + "]");
            updateRow(x);
            //Console.WriteLine("Checking Column of [" + x + "," + y + "]");
            updateCol(y);
            //Console.WriteLine("Updated S/R/C of [" + x + "," + y + "]");
        }
        return;
    }
    /// <summary>
    /// A command from a recently unsolved Cell to look for solvable cells using set completion
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    private void Scan(int x, int y)
    {
        List<List<Cell>> ranges = GetRanges(x, y, true);
        for (int r = 0; r < ranges.Count; r++)
        {
            foreach (int n in Cells[x][y].data)
                if (this.Is_unsolved() && ranges[r].FindAll(range => range.data.Contains(n)).Count == 1)
                {
                    switch (r)
                    {
                        case 0: completeSq(x, y, n); break;
                        case 1: completeRow(x, n); break;
                        case 2: completeCol(y, n); break;
                        default: break;
                    }
                }
        }
        return;
    }

    #region Set Solver Functions
    /// <summary>
    /// A request to solve cells within the titular range
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    private void updateSq(int x, int y)
    {
        x = x / 3 * 3;
        y = y / 3 * 3;
        for (int i = x; i < x + 3; i++)
            for (int j = y; j < y + 3; j++)
                if (!Cells[i][j].solved())
                    Solve(i, j);
        return;
    }
    /// <summary>
    /// A request to solve cells within the titular range
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    private void updateRow(int r)
    {
        for (int j = 0; j < 9; j++) Solve(r, j);
        return;
    }
    /// <summary>
    /// A request to solve cells within the titular range
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    private void updateCol(int c)
    {
        for (int i = 0; i < 9; i++) Solve(i, c);
        return;
    }
    #endregion Set Solver Functions

    #region Set Retrieval Functions
    /// <summary>
    /// A get request that collects all three areas (Square/Row/Column) within range of the Cell
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    /// <param name="return_unsolved">By default is false</param>
    /// <returns>Three lists of solved cells, or three lists of unsolved cells if return_unsolved is true</returns>
    private List<List<Cell>> GetRanges(int x, int y, bool return_unsolved = false)
    {
        List<List<Cell>> ranges = [];
        ranges.Add(getSq(x, y, return_unsolved));
        ranges.Add(getRow(x, return_unsolved));
        ranges.Add(getCol(y, return_unsolved));
        return ranges;
    }
    /// <summary>
    /// A get request that collects all Cells in the same Square as the Cell
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    /// <param name="return_unsolved">By default is false</param>
    /// <returns>A list of solved cells, or a list of unsolved cells if return_unsolved is true</returns>
    private List<Cell> getSq(int x, int y, bool return_unsolved = false)
    {
        List<Cell> solved = [];
        List<Cell> unsolved = [];
        List<List<int>> map = [];
        List<List<int>> pairs = [];
        x = x / 3 * 3;
        y = y / 3 * 3;
        for (int i = x; i < x + 3; i++)
            for (int j = y; j < y + 3; j++)
            {
                if (!Cells[i][j].solved())
                {
                    if (Cells[i][j].data.Count == 2 && unsolved.Contains(Cells[i][j]))
                        pairs.Add(Cells[i][j].data);
                    unsolved.Add(Cells[i][j]);
                    map.Add([i, j]);
                }
                else if (Cells[i][j].solved())
                    solved.Add(Cells[i][j]);
            }
        if (pairs.Count > 0) solved.AddRange(pairRemove(map, pairs));
        if (return_unsolved) return unsolved;
        return solved;
    }
    /// <summary>
    /// A get request that collects all Cells in the same Row as the Cell
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    /// <param name="return_unsolved">By default is false</param>
    /// <returns>A list of solved cells, or a list of unsolved cells if return_unsolved is true</returns>
    private List<Cell> getRow(int i, bool return_unsolved = false)
    {
        List<Cell> solved = [];
        List<Cell> unsolved = [];
        List<List<int>> map = [];
        List<List<int>> pairs = [];
        for (int j = 0; j < 9; j++)
        {
            if (!Cells[i][j].solved())
            {
                if (Cells[i][j].data.Count == 2 && unsolved.Contains(Cells[i][j]))
                    pairs.Add(Cells[i][j].data);
                unsolved.Add(Cells[i][j]);
                map.Add([i, j]);
            }
            else if (Cells[i][j].solved())
                solved.Add(Cells[i][j]);
        }
        if (pairs.Count > 0) solved.AddRange(pairRemove(map, pairs));
        if (return_unsolved) return unsolved;
        return solved;
    }
    /// <summary>
    /// A get request that collects all Cells in the same Column as the Cell
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    /// <param name="return_unsolved">By default is false</param>
    /// <returns>A list of solved cells, or a list of unsolved cells if return_unsolved is true</returns>
    private List<Cell> getCol(int j, bool return_unsolved = false)
    {
        List<Cell> solved = [];
        List<Cell> unsolved = [];
        List<List<int>> map = [];
        List<List<int>> pairs = [];
        for (int i = 0; i < 9; i++)
        {
            if (!Cells[i][j].solved())
            {
                if (Cells[i][j].data.Count == 2 && unsolved.Contains(Cells[i][j]))
                    pairs.Add(Cells[i][j].data);
                unsolved.Add(Cells[i][j]);
                map.Add([i, j]);
            }
            else if (Cells[i][j].solved())
                solved.Add(Cells[i][j]);
        }
        if (pairs.Count > 0) solved.AddRange(pairRemove(map, pairs));
        if (return_unsolved) return unsolved;
        return solved;
    }
    #endregion Set Retrieval Functions

    #region Set Completion Functions
    /// <summary>
    /// Will set-solve a cell in this Square if is only allowed to be placed in one cell of the Square
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    /// <param name="n">The number that might be found to have only one valid location in this set</param>
    private void completeSq(int x, int y, int n)
    {
        x = x / 3 * 3;
        y = y / 3 * 3;
        for (int i = x; i < x + 3; i++)
            for (int j = y; j < y + 3; j++)
            {
                if (!Cells[i][j].solved() && Cells[i][j].data.Contains(n))
                {
                    Cells[i][j].data = [n];
                    Console.WriteLine("*Set scan solved a cell!*");
                    Explode(i, j);
                    return;
                }
            }
        return;
    }
    // <summary>
    /// Will set-solve a cell in this Row if is only allowed to be placed in one cell of the Row
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    /// <param name="n">The number that might be found to have only one valid location in this set</param>
    private void completeRow(int r, int n)
    {
        for (int j = 0; j < 9; j++)
            if (!Cells[r][j].solved() && Cells[r][j].data.Contains(n))
            {
                Cells[r][j].data = [n];
                Console.WriteLine("*Set scan solved a cell!*");
                Explode(r, j);
                return;
            }
        return;
    }
    // <summary>
    /// Will set-solve a cell in this Column if is only allowed to be placed in one cell of the Column
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    /// <param name="n">The number that might be found to have only one valid location in this set</param>
    private void completeCol(int c, int n)
    {
        for (int i = 0; i < 9; i++)
            if (!Cells[i][c].solved() && Cells[i][c].data.Contains(n))
            {
                Cells[i][c].data = [n];
                Console.WriteLine("*Set scan solved a cell!*");
                Explode(i, c);
                return;
            }
        return;
    }
    #endregion Set Completion Functions

    #region Pair Handler Functions
    /// <summary>
    /// Looks through the mapped cells and removes the pairs from their cell data
    /// </summary>
    /// <param name="map">A list of the (x,y) coordinates</param>
    /// <param name="pairs">A list of pairs of numbers</param>
    /// <returns></returns>
    private List<Cell> pairRemove(List<List<int>> map, List<List<int>> pairs)
    {
        List<Cell> solved = [];
        foreach (List<int> pair in pairs)
            foreach (List<int> xy in map)
            {
                if (this.Is_unsolved() && preventSelfDelete(xy[0], xy[1], pair))
                {
                    Console.WriteLine("> Removing {0}{1} from [{2},{3}] {4}", pair[0], pair[1], xy[0], xy[1], Cells[xy[0]][xy[1]].debug_data());
                    Cells[xy[0]][xy[1]].data.Remove(pair[0]);
                    Cells[xy[0]][xy[1]].data.Remove(pair[1]);
                    if (Cells[xy[0]][xy[1]].solved())
                    {
                        Console.WriteLine("*Pairs helped solve a cell!*");
                        solved.Add(Cells[xy[0]][xy[1]]);
                        Explode(xy[0], xy[1]);
                    }
                }
            }
        return solved;
    }
    /// <summary>
    /// Checks to make sure the deletion will be productive, and prevents unproductive deletions
    /// </summary>
    /// <param name="x">The first coordinate of the Cell</param>
    /// <param name="y">The second coordinate of the Cell</param>
    /// <param name="pair">The pair of numbers</param>
    /// <returns>False if a deletion would leave a cell with no possible options</returns>
    private bool preventSelfDelete(int x, int y, List<int> pair)
    {
        if (Cells[x][y].data.Count == 2)
            if (Cells[x][y].data.Contains(pair[0]))
                if (Cells[x][y].data.Contains(pair[1]))
                    return false;
        if (Cells[x][y].data.Contains(pair[0])) return true;
        if (Cells[x][y].data.Contains(pair[1])) return true;
        return false;
    }
    #endregion Pair Handler Functions

    // private List<Pointer> GetPointers(int x, int y, string mode)
    // {
    //     List<Pointer> zone;
    //     switch (mode)
    //     {
    //         case "sq":
    //             x = x / 3 * 3;
    //             y = y / 3 * 3;
    //             for (int i = x; i < x + 3; i++)
    //                 for (int j = y; j < y + 3; j++)
    //                     zone.Add(&cells[i][j]);
    //             break;
    //         case "row": break;
    //         case "col": break;
    //         default: break;
    //     }
    // }

};