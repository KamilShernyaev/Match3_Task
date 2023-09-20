using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary> 
/// Класс MatchesInfo содержит информацию о совпадающих конфетах. 
/// </summary> 
/// <remarks> 
/// Этот класс содержит список совпадающих конфет, методы для добавления новых объектов в список, 
/// а также свойство для хранения типов бонусов, содержащихся в совпадающих конфетах. 
/// </remarks>
public class MatchesInfo
{
    private List<GameObject> matchedCandies;

    /// <summary> 
    /// Возвращает уникальный список совпадающих конфет. 
    /// </summary> 
    public IEnumerable<GameObject> MatchedCandy => 
        matchedCandies.Distinct();

    /// <summary> 
    /// Добавляет новый объект (конфету) в список совпадающих конфет. 
    /// </summary> 
    public void AddObject(GameObject go)
    {
        if (!matchedCandies.Contains(go))
            matchedCandies.Add(go);
    }

    /// <summary> 
    /// Добавляет диапазон объектов (конфет) в список совпадающих конфет. 
    /// </summary> 
    public void AddObjectRange(IEnumerable<GameObject> gos)
    {
        foreach (var item in gos)
        {
            AddObject(item);
        }
    }

    /// <summary> 
    /// Типы бонусов, содержащихся в совпадающих конфетах 
    /// </summary> 
    public BonusType BonusesContained { get; set; } 
    
    public MatchesInfo()
    {
        matchedCandies = new List<GameObject>();
        BonusesContained = BonusType.None;
    }
}

