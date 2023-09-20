using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary> 
/// Класс AlteredCandyInfo содержит информацию об измененных конфетах. 
/// </summary> 
/// <remarks> 
/// Этот класс содержит список новых конфет, которые были изменены, а также максимальное расстояние, на котором конфеты могут быть изменены. 
/// Класс также предоставляет методы для добавления новой конфеты в список измененных конфет и получения уникального списка измененных конфет. 
/// </remarks>
public class AlteredCandyInfo
{
    private List<GameObject> newCandy { get; set; } /// Список новых конфет, которые были изменены. 
    public int MaxDistance { get; set; } /// Максимальное расстояние, на котором конфеты могут быть изменены. 

    /// <summary>
    /// Возвращает уникальный список измененных конфет. 
    /// </summary>
    public IEnumerable<GameObject> AlteredCandy => 
        newCandy.Distinct();

    /// <summary> 
    /// Добавляет новую конфету в список измененных конфет. 
    /// </summary> 
    public void AddCandy(GameObject go)
    {
        if (!newCandy.Contains(go))
            newCandy.Add(go);
    }

    public AlteredCandyInfo() => 
        newCandy = new List<GameObject>();
}
