/// <summary>
/// Класс BonusTypeUtilities предоставляет утилиты для работы с типами бонусов.
/// </summary>
public static class BonusTypeUtilities
{
    /// <summary>
    /// Метод для проверки, содержит ли тип бонуса DestroyWholeRowColumn.
    /// </summary>
    /// <param name="bt">Тип бонуса</param>
    /// <returns>True, если тип бонуса содержит DestroyWholeRowColumn, иначе False</returns>
    public static bool ContainsDestroyWholeRowColumn(BonusType bt)
    {
        return (bt & BonusType.DestroyWholeRowColumn) 
            == BonusType.DestroyWholeRowColumn;
    }
}