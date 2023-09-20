using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapesArray
{   
    private LevelStaticData _levelStaticData;
    private GameObject[,] shapes;
    private GameObject backupG1, backupG2;

    public ShapesArray(LevelStaticData levelStaticData)
    {
        _levelStaticData = levelStaticData;
        shapes = new GameObject[_levelStaticData.Rows, _levelStaticData.Columns];
    }

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

    public void UndoSwap()
    {
        if (backupG1 == null || backupG2 == null)
            throw new Exception("Backup is null");

        Swap(backupG1, backupG2);
    }

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

    private IEnumerable<GameObject> GetMatchesHorizontally(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<Shape>();
        //check left
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

        //check right
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

        //we want more than three matches
        if (matches.Count < ConstantsGameLogic.MinimumMatches)
            matches.Clear();

        return matches.Distinct();
    }

    private IEnumerable<GameObject> GetMatchesVertically(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<Shape>();
        //check bottom
        if (shape.Row != 0)
            for (int row = shape.Row - 1; row >= 0; row--)
            {
                if (shapes[row, shape.Column] != null &&
                    shapes[row, shape.Column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[row, shape.Column]);
                }
                else
                    break;
            }

        //check top
        if (shape.Row != _levelStaticData.Rows - 1)
            for (int row = shape.Row + 1; row < _levelStaticData.Rows; row++)
            {
                if (shapes[row, shape.Column] != null && 
                    shapes[row, shape.Column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[row, shape.Column]);
                }
                else
                    break;
            }


        if (matches.Count < ConstantsGameLogic.MinimumMatches)
            matches.Clear();

        return matches.Distinct();
    }

    public void Remove(GameObject item) => 
        shapes[item.GetComponent<Shape>().Row, item.GetComponent<Shape>().Column] = null;

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

    public IEnumerable<ShapeInfo> GetEmptyItemsOnColumn(int column)
    {
        List<ShapeInfo> emptyItems = new List<ShapeInfo>();
        for (int row = 0; row < _levelStaticData.Rows; row++)
        {
            if (shapes[row, column] == null)
                emptyItems.Add(new ShapeInfo() { Row = row, Column = column });
        }
        return emptyItems;
    }
}

