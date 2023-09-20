using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 

/// <summary>
/// Класс Utilities предоставляет различные утилиты и вспомогательные методы для игры.
/// </summary>
public static class Utilities 
{ 
    /// <summary>
    /// Метод для анимации потенциальных совпадений объектов.
    /// </summary>
    /// <param name="potentialMatches">Коллекция объектов-потенциальных совпадений</param>
    /// <returns>IEnumerator для корутины</returns>
    public static IEnumerator AnimatePotentialMatches(IEnumerable<GameObject> potentialMatches) 
    { 
        for (float i = 1f; i >= 0.3f; i -= 0.1f) 
        { 
            UpdateOpacity(potentialMatches, i); 
            yield return new WaitForSeconds(ConstantsAnimation.OpacityAnimationFrameDelay); 
        } 
 
        for (float i = 0.3f; i <= 1f; i += 0.1f) 
        { 
            UpdateOpacity(potentialMatches, i); 
            yield return new WaitForSeconds(ConstantsAnimation.OpacityAnimationFrameDelay); 
        } 
    } 
    
    /// <summary>
    /// Метод для обновления прозрачности объектов.
    /// </summary>
    /// <param name="objects">Коллекция объектов</param>
    /// <param name="opacity">Значение прозрачности</param>
    private static void UpdateOpacity(IEnumerable<GameObject> objects, float opacity) 
    { 
        foreach (var item in objects) 
        { 
            Color c = item.GetComponent<SpriteRenderer>().color; 
            c.a = opacity; 
            item.GetComponent<SpriteRenderer>().color = c; 
        } 
    } 

    /// <summary>
    /// Метод для проверки, являются ли две фигуры вертикальными или горизонтальными соседями.
    /// </summary>
    /// <param name="s1">Первая фигура</param>
    /// <param name="s2">Вторая фигура</param>
    /// <returns>True, если фигуры являются вертикальными или горизонтальными соседями, иначе False</returns>
    public static bool AreVerticalOrHorizontalNeighbors(Shape s1, Shape s2) 
    { 
        return (s1.Column == s2.Column || s1.Row == s2.Row) 
            && Mathf.Abs(s1.Column - s2.Column) <= 1 
            && Mathf.Abs(s1.Row - s2.Row) <= 1; 
    } 

    /// <summary>
    /// Метод для получения потенциальных совпадений объектов на игровом поле.
    /// </summary>
    /// <param name="shapes">Массив фигур на игровом поле</param>
    /// <param name="levelStaticData">Статические данные уровня</param>
    /// <returns>Коллекция объектов-потенциальных совпадений</returns>
    public static IEnumerable<GameObject> GetPotentialMatches(ShapesArray shapes, LevelStaticData levelStaticData) 
    { 
        List<List<GameObject>> matches = new List<List<GameObject>>(); 
 
        for (int row = 0; row < levelStaticData.Rows; row++) 
        { 
            for (int column = 0; column < levelStaticData.Columns; column++) 
            { 
                var horizontalMatches = CheckHorizontalMatches(row, column, shapes, levelStaticData); 
                var verticalMatches = CheckVerticalMatches(row, column, shapes, levelStaticData); 
 
                if (horizontalMatches != null) matches.Add(horizontalMatches); 
                if (verticalMatches != null) matches.Add(verticalMatches); 
 
                if (matches.Count >= 3) 
                    return matches[Random.Range(0, matches.Count - 1)]; 
 
                if (row >= levelStaticData.Rows / 2 && matches.Count > 0 && matches.Count <= 2) 
                    return matches[Random.Range(0, matches.Count - 1)]; 
            } 
        } 
 
        return null; 
    } 

    /// <summary>
    /// Метод для проверки горизонтальных совпадений фигур.
    /// </summary>
    /// <param name="row">Номер строки</param>
    /// <param name="column">Номер столбца</param>
    /// <param name="shapes">Массив фигур на игровом поле</param>
    /// <param name="levelStaticData">Статические данные уровня</param>
    /// <returns>Список объектов, образующих горизонтальное совпадение, или null, если совпадение отсутствует</returns>
    private static List<GameObject> CheckHorizontalMatches(int row, int column, ShapesArray shapes, LevelStaticData levelStaticData) 
    { 
        if (column <= levelStaticData.Columns - 2) 
        { 
            if (shapes[row, column].GetComponent<Shape>().IsSameType(shapes[row, column + 1].GetComponent<Shape>())) 
            { 
                var matches = new List<GameObject>() { shapes[row, column], shapes[row, column + 1] }; 
 
                if (row >= 1 && column >= 1 && shapes[row, column].GetComponent<Shape>().IsSameType(shapes[row - 1, column - 1].GetComponent<Shape>())) 
                    matches.Add(shapes[row - 1, column - 1]); 
 
                if (row <= levelStaticData.Rows - 2 && column >= 1 && shapes[row, column].GetComponent<Shape>().IsSameType(shapes[row + 1, column - 1].GetComponent<Shape>())) 
                    matches.Add(shapes[row + 1, column - 1]); 
 
                if (matches.Count >= 3) 
                    return matches; 
            } 
        } 
 
        return null; 
    } 

    /// <summary>
    /// Метод для проверки вертикальных совпадений фигур.
    /// </summary>
    /// <param name="row">Номер строки</param>
    /// <param name="column">Номер столбца</param>
    /// <param name="shapes">Массив фигур на игровом поле</param>
    /// <param name="levelStaticData">Статические данные уровня</param>
    /// <returns>Список объектов, образующих вертикальное совпадение, или null, если совпадение отсутствует</returns>
    private static List<GameObject> CheckVerticalMatches(int row, int column, ShapesArray shapes, LevelStaticData levelStaticData) 
    { 
        if (row <= levelStaticData.Rows - 2) 
        { 
            if (shapes[row, column].GetComponent<Shape>().IsSameType(shapes[row + 1, column].GetComponent<Shape>())) 
            { 
                var matches = new List<GameObject>() { shapes[row, column], shapes[row + 1, column] }; 
 
                if (column >= 1 && row >= 1 && shapes[row, column].GetComponent<Shape>().IsSameType(shapes[row - 1, column - 1].GetComponent<Shape>())) 
                    matches.Add(shapes[row - 1, column - 1]); 
 
                if (column <= levelStaticData.Columns - 2 && row >= 1 && shapes[row, column].GetComponent<Shape>().IsSameType(shapes[row - 1, column + 1].GetComponent<Shape>())) 
                    matches.Add(shapes[row - 1, column + 1]); 
 
                if (matches.Count >= 3) 
                    return matches; 
            } 
        } 
 
        return null; 
    } 
} 