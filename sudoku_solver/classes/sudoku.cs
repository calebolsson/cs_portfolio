using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

public class Sudoku
{
    public string name { get; set; }
    public string difficulty { get; set; }
    public List<List<Cell>> cells = [[], [], [], [], [], [], [], [], []];
    public int cells_solved { get; set; }

    public DisplayHandler ui = new();

    public Sudoku(string name, string difficulty, List<List<int>> src)
    {
        this.name = name;
        this.difficulty = difficulty;
        this.cells_solved = 0;
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                cells[i].Add(new Cell(src[i][j]));
                if (cells[i][j].solved()) cells_solved++;
                //Console.WriteLine("[{0},{1}] ({2}) {3}/81", i, j, cells[i][j].answer(), cells_solved);
            }
    }

    public bool is_unsolved()
    {
        if (cells_solved < 81) return true;
        return false;
    }

    public void solve(int x, int y)
    {
        if (cells[x][y].solved()) return;
        else if (updateCell(x, y)) explode(x, y);
        return;
    }

    private bool updateCell(int x, int y)
    {
        List<List<Cell>> ranges = getRanges(x, y);
        foreach (List<Cell> range in ranges)
            cells[x][y].remove(range.Distinct().ToList());
        if (cells[x][y].solved()) return true;
        //Console.WriteLine("[" + x + "," + y + "] (" + cells[x][y].debug_data() + ")");
        scan(x, y);
        return false;
    }

    private void explode(int x, int y)
    {
        if (this.is_unsolved())
        {
            cells_solved++;
            //ui.displayGrid(cells);
            Console.WriteLine("[" + x + "," + y + "] Solved! " + cells[x][y].answer() + " {" + cells_solved + "/81}");
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

    private void scan(int x, int y)
    {
        List<List<Cell>> ranges = getRanges(x, y, true);
        for (int r = 0; r < ranges.Count; r++)
        {
            foreach (int n in cells[x][y].data)
                if (this.is_unsolved() && ranges[r].FindAll(range => range.data.Contains(n)).Count == 1)
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
    private void updateSq(int x, int y)
    {
        x = x / 3 * 3;
        y = y / 3 * 3;
        for (int i = x; i < x + 3; i++)
            for (int j = y; j < y + 3; j++)
                if (!cells[i][j].solved())
                    solve(i, j);
        return;
    }
    private void updateRow(int r)
    {
        for (int j = 0; j < 9; j++) solve(r, j);
        return;
    }
    private void updateCol(int c)
    {
        for (int i = 0; i < 9; i++) solve(i, c);
        return;
    }
    #endregion Set Solver Functions

    #region Set Retrieval Functions
    private List<List<Cell>> getRanges(int x, int y, bool return_unsolved = false)
    {
        List<List<Cell>> ranges = [];
        ranges.Add(getSq(x, y, return_unsolved));
        ranges.Add(getRow(x, return_unsolved));
        ranges.Add(getCol(y, return_unsolved));
        return ranges;
    }
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
                if (!cells[i][j].solved())
                {
                    if (cells[i][j].data.Count == 2 && unsolved.Contains(cells[i][j]))
                        pairs.Add(cells[i][j].data);
                    unsolved.Add(cells[i][j]);
                    map.Add([i, j]);
                }
                else if (cells[i][j].solved())
                    solved.Add(cells[i][j]);
            }
        if (pairs.Count > 0) solved.AddRange(pairRemove(map, pairs));
        if (return_unsolved) return unsolved;
        return solved;
    }
    private List<Cell> getRow(int i, bool return_unsolved = false)
    {
        List<Cell> solved = [];
        List<Cell> unsolved = [];
        List<List<int>> map = [];
        List<List<int>> pairs = [];
        for (int j = 0; j < 9; j++)
        {
            if (!cells[i][j].solved())
            {
                if (cells[i][j].data.Count == 2 && unsolved.Contains(cells[i][j]))
                    pairs.Add(cells[i][j].data);
                unsolved.Add(cells[i][j]);
                map.Add([i, j]);
            }
            else if (cells[i][j].solved())
                solved.Add(cells[i][j]);
        }
        if (pairs.Count > 0) solved.AddRange(pairRemove(map, pairs));
        if (return_unsolved) return unsolved;
        return solved;
    }
    private List<Cell> getCol(int j, bool return_unsolved = false)
    {
        List<Cell> solved = [];
        List<Cell> unsolved = [];
        List<List<int>> map = [];
        List<List<int>> pairs = [];
        for (int i = 0; i < 9; i++)
        {
            if (!cells[i][j].solved())
            {
                if (cells[i][j].data.Count == 2 && unsolved.Contains(cells[i][j]))
                    pairs.Add(cells[i][j].data);
                unsolved.Add(cells[i][j]);
                map.Add([i, j]);
            }
            else if (cells[i][j].solved())
                solved.Add(cells[i][j]);
        }
        if (pairs.Count > 0) solved.AddRange(pairRemove(map, pairs));
        if (return_unsolved) return unsolved;
        return solved;
    }
    #endregion Set Retrieval Functions

    #region Set Completion Functions
    private void completeSq(int x, int y, int n)
    {
        x = x / 3 * 3;
        y = y / 3 * 3;
        for (int i = x; i < x + 3; i++)
            for (int j = y; j < y + 3; j++)
            {
                if (!cells[i][j].solved() && cells[i][j].data.Contains(n))
                {
                    cells[i][j].data = [n];
                    Console.WriteLine("*Set scan solved a cell!*");
                    explode(i, j);
                    return;
                }
            }
        return;
    }
    private void completeRow(int r, int n)
    {
        for (int j = 0; j < 9; j++)
            if (!cells[r][j].solved() && cells[r][j].data.Contains(n))
            {
                cells[r][j].data = [n];
                Console.WriteLine("*Set scan solved a cell!*");
                explode(r, j);
                return;
            }
        return;
    }
    private void completeCol(int c, int n)
    {
        for (int i = 0; i < 9; i++)
            if (!cells[i][c].solved() && cells[i][c].data.Contains(n))
            {
                cells[i][c].data = [n];
                Console.WriteLine("*Set scan solved a cell!*");
                explode(i, c);
                return;
            }
        return;
    }
    #endregion Set Completion Functions

    #region Pair Handler Functions
    private List<Cell> pairRemove(List<List<int>> map, List<List<int>> pairs)
    {
        List<Cell> solved = [];
        foreach (List<int> pair in pairs)
            foreach (List<int> xy in map)
            {
                if (this.is_unsolved() && preventSelfDelete(xy[0], xy[1], pair))
                {
                    Console.WriteLine("> Removing {0}{1} from [{2},{3}] {4}", pair[0], pair[1], xy[0], xy[1], cells[xy[0]][xy[1]].debug_data());
                    cells[xy[0]][xy[1]].data.Remove(pair[0]);
                    cells[xy[0]][xy[1]].data.Remove(pair[1]);
                    if (cells[xy[0]][xy[1]].solved())
                    {
                        Console.WriteLine("*Pairs helped solve a cell!*");
                        solved.Add(cells[xy[0]][xy[1]]);
                        explode(xy[0], xy[1]);
                    }
                }
            }
        return solved;
    }
    private bool preventSelfDelete(int x, int y, List<int> pair)
    {
        if (cells[x][y].data.Count == 2)
            if (cells[x][y].data.Contains(pair[0]))
                if (cells[x][y].data.Contains(pair[1]))
                    return false;
        if (cells[x][y].data.Contains(pair[0])) return true;
        if (cells[x][y].data.Contains(pair[1])) return true;
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