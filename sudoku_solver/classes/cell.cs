
public class Cell : IEquatable<Cell>
{
    public List<int> data;

    public Cell(int num)
    {
        if (num != 0) data = [num];
        else data = [1, 2, 3, 4, 5, 6, 7, 8, 9];
    }

    // override object.Equals
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

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return Int32.Parse(this.debug_data());
    }

    public string debug_data()
    {
        string tostring = "";
        foreach (int note in data) tostring += (char)(48 + note);
        if (data != null) return tostring;
        return "-";
    }

    public int answer()
    {
        if (this.solved()) return data[0];
        else return 0;
    }

    public void remove(List<Cell> cells)
    {
        foreach (Cell cell in cells)
            foreach (int value in cell.data)
                this.data.Remove(value);
    }

    public bool solved()
    {
        return data.Count == 1;
    }

};