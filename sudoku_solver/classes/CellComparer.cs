using System.Diagnostics.CodeAnalysis;

class CellComparer : IEqualityComparer<Cell>
{

    public bool Equals(Cell? x, Cell? y)
    {
        if (Object.ReferenceEquals(x, y)) return true;
        if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;
        return x.data == y.data;
    }

    public int GetHashCode([DisallowNull] Cell obj)
    {
        if (Object.ReferenceEquals(obj, null)) return 0;
        int hashCell = obj.data == null ? 0 : obj.data.GetHashCode();
        return hashCell;
    }
}