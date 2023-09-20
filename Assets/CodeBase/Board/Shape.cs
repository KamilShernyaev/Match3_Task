using System;
using UnityEngine;

/// <summary> 
/// Класс Shape предоставляет возможность работать с фигурами в игре. 
/// </summary> 
/// <remarks> 
/// Этот класс содержит свойства для типа фигур, ее позиции в колонке и строке, а также свойство для хранения типа бонуса, связанного с фигурой. 
/// Класс также предоставляет методы для проверки, является ли фигура того же типа, что и другая фигура, а также для обмена свойствами двух фигур. 
/// </remarks>
public class Shape : MonoBehaviour
{
    public BonusType Bonus { get; set; } // Тип бонуса, связанного с фигурой 
    public int Column { get; set; } // Колона, в которой находится фигура 
    public int Row { get; set; } // Строка, в которой находится фигура 
    public string Type { get; set; } // Тип фигуры 

    public Shape() => 
        Bonus = BonusType.None;

    /// <summary> 
    /// Проверяет, является ли текущая фигура того же типа, что и параметр. 
    /// </summary> 
    /// <param name="otherShape">Другая фигура для сравнения</param> 
    /// <returns>True, если фигуры имеют одинаковый тип, иначе False</returns> 
    public bool IsSameType(Shape otherShape)
    {
        if (otherShape == null || !(otherShape is Shape))
            throw new ArgumentException("otherShape");

        return string.Compare(this.Type, (otherShape as Shape).Type) == 0;
    }

    // <summary> 
    /// Альтернативный конструктор для установки свойств фигуры. 
    /// </summary> 
    /// <param name="type">Тип фигуры</param> 
    /// <param name="row">Строка, в которой находится фигура</param> 
    /// <param name="column">Колонка, в которой находится фигура</param> 
    public void Assign(string type, int row, int column)
    {
        if (string.IsNullOrEmpty(type))
            throw new ArgumentException("type");

        Column = column;
        Row = row;
        Type = type;
    }

    /// <summary> 
    /// Обменивает свойства двух фигур. 
    /// </summary> 
    /// <param name="a">Первая фигура</param> 
    /// <param name="b">Вторая фигура</param> 
    public static void SwapColumnRow(Shape a, Shape b)
    {
        int temp = a.Row;
        a.Row = b.Row;
        b.Row = temp;

        temp = a.Column;
        a.Column = b.Column;
        b.Column = temp;
    }
}



