
public class Cell : IEquatable<Cell>
{
    public List<int> data;

    public Cell(int num)
    {
        if (num != 0) data = [num];
        else data = [1, 2, 3, 4, 5, 6, 7, 8, 9];
    }

    /// <summary>
    /// Overrides the default Equals method
    /// </summary>
    /// <param name="obj">The Cell object to be compared</param>
    /// <returns>True if this cell data matches the object's cell data</returns>
    public bool Equals(Cell? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        if (Object.ReferenceEquals(this, obj)) return true;
        if (this.data.Count != obj.data.Count) return false;
        for (int o = 0; o < obj.data.Count; o++)
            if (this.data[o] != obj.data[o]) return false;
        return true;
        //return base.Equals(obj);
    }

    /// <summary>
    /// Overrides the default GetHashCode method
    /// </summary>
    /// <returns>The integer representation of this cell's data</returns>
    public override int GetHashCode()
    {
        return Int32.Parse(this.debug_data());
    }

    /// <summary>
    /// A method to request the cell's data in an unsolved state
    /// </summary>
    /// <returns>An unformatted string, or "-" if there is an error</returns>
    public string debug_data()
    {
        string tostring = "";
        foreach (int note in data) tostring += (char)(48 + note);
        if (data != null) return tostring;
        return "-";
    }

    /// <summary>
    /// A method to request the cell's data in a solved state
    /// </summary>
    /// <returns>The integer value of the solved cell, or 0 if the cell is unsolved</returns>
    public int answer()
    {
        if (this.solved()) return data[0];
        else return 0;
    }

    /// <summary>
    /// Removes each unavailable value from the cell data
    /// </summary>
    /// <param name="cells">The cells whose values will be removed from this cell</param>
    public void remove(List<Cell> cells)
    {
        foreach (Cell cell in cells)
            foreach (int value in cell.data)
                this.data.Remove(value);
    }

    /// <summary>
    /// Checks to see if this cell has already been solved
    /// </summary>
    /// <returns>True if there is only one option in cell data</returns>
    public bool solved()
    {
        return data.Count == 1;
    }

};