
public struct RowData
{
    public readonly float rowAngle;
    public readonly int listLenght;
    public bool[] decorations;

    int _currentInd;
    public int currentIndex {
        get => _currentInd;
        set {
            if (value > listLenght - 1)
                _currentInd = 0;
            else if (value < 0)
                _currentInd = listLenght - 1;
            else
                _currentInd = value;
        }
    }

    public RowData(int rowNumber)
    {
        int sidesCount = rowNumber < 0 ? 4 : (rowNumber+1) * 4;
        rowAngle = 360 / sidesCount;
        listLenght = sidesCount;
        decorations = new bool[sidesCount];
        _currentInd = 0;
    }
}
