using System;

/// <summary> 
/// Класс BonusType предоставляет возможность определить типы бонусов. 
/// </summary> 
/// <remarks> 
/// Это перечисление имеет флаговый атрибут [Flags], что позволяет использовать комбинации значений. 
/// В данном случае определены два возможных значения: None (отсутствие бонуса) и DestroyWholeRowColumn (уничтожение всей строки или столбца). 
/// </remarks>
[Flags] 
public enum BonusType 
{ 
    None,
    DestroyWholeRowColumn,
} 
