public static class BonusTypeUtilities
{
    public static bool ContainsDestroyWholeRowColumn(BonusType bt)
    {
        return (bt & BonusType.DestroyWholeRowColumn) 
            == BonusType.DestroyWholeRowColumn;
    }
}
