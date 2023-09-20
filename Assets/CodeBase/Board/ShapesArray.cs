using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary> 
/// Класс ShapesArray представляет массив фигур в игре. 
/// </summary> 
public class ShapesArray
{   
    private LevelStaticData _levelStaticData; // Статические данные уровня 
    private GameObject[,] shapes; // Массив фигур 
    private GameObject backupG1, backupG2; // Резервные копии фигур для отмены обмена 

    public ShapesArray(LevelStaticData levelStaticData)
    {
        _levelStaticData = levelStaticData;
        shapes = new GameObject[_levelStaticData.Rows, _levelStaticData.Columns];
    }

    /// <summary> 
    /// Индексатор для доступа к элементам массива фигур. 
    /// </summary> 
    /// <param name="row">Строка</param> 
    /// <param name="column">Колонна</param> 
    /// <returns>фигура в указанной позиции</returns> 
    public GameObject this[int row, int column]
    {
        get
        {
            try
            {
                return shapes[row, column];
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw ex;
            }
        }
        set
        {
            shapes[row, column] = value;
        }
    }

    
    /// <summary> 
    /// Обмен фигурами. 
    /// </summary> 
    /// <param name="g1">Первая фигура</param> 
    /// <param name="g2">Вторая фигура</param> 
    public void Swap(GameObject g1, GameObject g2)
    {
        // Создание резервных копий в случае, если совпадение не будет найдено
        backupG1 = g1;
        backupG2 = g2;
        var g1Shape = g1.GetComponent<Shape>();
        var g2Shape = g2.GetComponent<Shape>();
        // Получение индексов в массиве
        int g1Row = g1Shape.Row;
        int g1Column = g1Shape.Column;
        int g2Row = g2Shape.Row;
        int g2Column = g2Shape.Column;
        // Обмен элементами в массиве
        var temp = shapes[g1Row, g1Column];
        shapes[g1Row, g1Column] = shapes[g2Row, g2Column];
        shapes[g2Row, g2Column] = temp;
        // Обмен соответствующих свойств
        Shape.SwapColumnRow(g1Shape, g2Shape);
    }

    /// <summary> 
    /// Отмена обмена фигурами. 
    /// </summary> 
    public void UndoSwap()
    {
        if (backupG1 == null || backupG2 == null)
            throw new Exception("Backup is null");

        Swap(backupG1, backupG2);
    }

    /// <summary> 
    /// Получение всех совпадений для указанных фигур. 
    /// </summary> 
    /// <param name="gos">Список фигур</param> 
    /// <returns>Список всех совпадающих фигур</returns> 
    public IEnumerable<GameObject> GetMatches(IEnumerable<GameObject> gos)
    {
        List<GameObject> matches = new List<GameObject>();
        foreach (var go in gos)
        {
            var goMatches = GetMatches(go);
            matches.AddRange(goMatches.MatchedCandy);
        }
        return matches.Distinct();
    }

    /// <summary> 
    /// Получение информации о совпадениях для указанной фигуры. 
    /// </summary> 
    /// <param name="go">Фигура</param> 
    /// <returns>Информация о совпадениях</returns> 
    public MatchesInfo GetMatches(GameObject go)
    {
        MatchesInfo matchesInfo = new MatchesInfo();

        var horizontalMatches = GetMatchesHorizontally(go);
        if (ContainsDestroyRowColumnBonus(horizontalMatches))
        {
            horizontalMatches = GetEntireRow(go);
            if (!BonusTypeUtilities.ContainsDestroyWholeRowColumn(matchesInfo.BonusesContained))
                matchesInfo.BonusesContained |= BonusType.DestroyWholeRowColumn;
        }
        matchesInfo.AddObjectRange(horizontalMatches);

        var verticalMatches = GetMatchesVertically(go);
        if (ContainsDestroyRowColumnBonus(verticalMatches))
        {
            verticalMatches = GetEntireColumn(go);
            if (!BonusTypeUtilities.ContainsDestroyWholeRowColumn(matchesInfo.BonusesContained))
                matchesInfo.BonusesContained |= BonusType.DestroyWholeRowColumn;
        }
        matchesInfo.AddObjectRange(verticalMatches);

        return matchesInfo;
    }

    /// <summary> 
    /// Проверка, содержится ли в совпадениях бонус для уничтожения строки или столбца. 
    /// </summary> 
    /// <param name="matches">Список совпадающих фигур</param> 
    /// <returns>True, если содержится бонус для уничтожения строки или столбца, иначе False</returns> 
    private bool ContainsDestroyRowColumnBonus(IEnumerable<GameObject> matches)
    {
        if (matches.Count() >= ConstantsGameLogic.MinimumMatches)
        {
            foreach (var go in matches)
            {
                if (BonusTypeUtilities.ContainsDestroyWholeRowColumn
                    (go.GetComponent<Shape>().Bonus))
                    return true;
            }
        }

        return false;
    }
     
    /// <summary> 
    /// Получение всех элементов в строке, содержащей указанную фигуру. 
    /// </summary> 
    /// <param name="go">Фигура</param> 
    /// <returns>Список всех фигур в строке</returns> 
    private IEnumerable<GameObject> GetEntireRow(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        int row = go.GetComponent<Shape>().Row;
        for (int column = 0; column < _levelStaticData.Columns; column++)
        {
            matches.Add(shapes[row, column]);
        }
        return matches;
    }

    /// <summary> 
    /// Получение всех элементов в столбце, содержащем указанную фигуру. 
    /// </summary> 
    /// <param name="go">Фигура</param> 
    /// <returns>Список всех фигур в столбце</returns> 
    private IEnumerable<GameObject> GetEntireColumn(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        int column = go.GetComponent<Shape>().Column;
        for (int row = 0; row < _levelStaticData.Rows; row++)
        {
            matches.Add(shapes[row, column]);
        }
        return matches;
    }

    /// <summary> 
    /// Получение всех горизонтальных совпадений для указанной Фигуры. 
    /// </summary> 
    /// <param name="go">Фигура</param> 
    /// <returns>Список всех горизонтальных совпадающих Фигур</returns> 
    private IEnumerable<GameObject> GetMatchesHorizontally(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<Shape>();
        // Проверка слева 
        if (shape.Column != 0)
            for (int column = shape.Column - 1; column >= 0; column--)
            {
                if (shapes[shape.Row, column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[shape.Row, column]);
                }
                else
                    break;
            }

        // Проверка справа 
        if (shape.Column != _levelStaticData.Columns - 1)
            for (int column = shape.Column + 1; column < _levelStaticData.Columns; column++)
            {
                if (shapes[shape.Row, column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[shape.Row, column]);
                }
                else
                    break;
            }

        // Проверка, чтобы было не менее трех совпадений
        if (matches.Count < ConstantsGameLogic.MinimumMatches)
            matches.Clear();

        return matches.Distinct();
    }

    /// <summary>
    /// Метод для получения всех объектов, совпадающих по вертикали с заданным объектом.
    /// </summary>
    /// <param name="go">Заданный объект</param>
    /// <returns>Коллекция объектов, совпадающих по вертикали</returns>
    private IEnumerable<GameObject> GetMatchesVertically(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<Shape>();

        // Проверяем совпадения внизу
        if (shape.Row != 0)
        {
            for (int row = shape.Row - 1; row >= 0; row--)
            {
                if (shapes[row, shape.Column] != null &&
                    shapes[row, shape.Column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[row, shape.Column]);
                }
                else
                {
                    break;
                }
            }
        }

        // Проверяем совпадения сверху
        if (shape.Row != _levelStaticData.Rows - 1)
        {
            for (int row = shape.Row + 1; row < _levelStaticData.Rows; row++)
            {
                if (shapes[row, shape.Column] != null && 
                    shapes[row, shape.Column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[row, shape.Column]);
                }
                else
                {
                    break;
                }
            }
        }

        // Если количество совпадений меньше минимального значения, очищаем список
        if (matches.Count < ConstantsGameLogic.MinimumMatches)
        {
            matches.Clear();
        }

        // Возвращаем уникальные объекты совпадений
        return matches.Distinct();
    }

    /// <summary>
    /// Метод для удаления объекта из массива shapes.
    /// </summary>
    /// <param name="item">Удаляемый объект</param>
    public void Remove(GameObject item) => 
        shapes[item.GetComponent<Shape>().Row, item.GetComponent<Shape>().Column] = null;

    /// <summary>
    /// Метод для сжатия элементов на заданных столбцах.
    /// </summary>
    /// <param name="columns">Столбцы, на которых происходит сжатие</param>
    /// <returns>Информация о сжатии элементов</returns>
    public AlteredCandyInfo Collapse(IEnumerable<int> columns)
    {
        AlteredCandyInfo collapseInfo = new AlteredCandyInfo();

        // Поиск в каждом столбце
        foreach (var column in columns)
        {
            // Начинаем с нижней строки
            for (int row = 0; row < _levelStaticData.Rows - 1; row++)
            {
                // Если находим пустой элемент
                if (shapes[row, column] == null)
                {
                    // Начинаем поиск первого непустого элемента
                    for (int row2 = row + 1; row2 < _levelStaticData.Rows; row2++)
                    {
                        // Если находим, перемещаем его вниз (заменяем на пустой элемент, который мы нашли)
                        if (shapes[row2, column] != null)
                        {
                            shapes[row, column] = shapes[row2, column];
                            shapes[row2, column] = null;

                            // Вычисляем наибольшее расстояние
                            if (row2 - row > collapseInfo.MaxDistance) 
                                collapseInfo.MaxDistance = row2 - row;

                            // Присваиваем новую строку и столбец (имя не изменяется)
                            shapes[row, column].GetComponent<Shape>().Row = row;
                            shapes[row, column].GetComponent<Shape>().Column = column;

                            collapseInfo.AddCandy(shapes[row, column]);
                            break;
                        }
                    }
                }
            }
        }

        return collapseInfo;
    }

    /// <summary>
    /// Метод для получения пустых элементов в заданном столбце.
    /// </summary>
    /// <param name="column">Столбец</param>
    /// <returns>Коллекция информации о пустых элементах</returns>
    public IEnumerable<ShapeInfo> GetEmptyItemsOnColumn(int column)
    {
        List<ShapeInfo> emptyItems = new List<ShapeInfo>();

        for (int row = 0; row < _levelStaticData.Rows; row++)
        {
            if (shapes[row, column] == null)
            {
                emptyItems.Add(new ShapeInfo() { Row = row, Column = column });
            }
        }

        return emptyItems;
    }
}

