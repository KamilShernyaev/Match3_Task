using System; 
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
 
public static class Utilities 
{ 
    private static float AnimationDelay = ConstantsAnimation.OpacityAnimationFrameDelay; 
 
    public static IEnumerator AnimatePotentialMatches(IEnumerable<GameObject> potentialMatches) 
    { 
        for (float i = 1f; i >= 0.3f; i -= 0.1f) 
        { 
            UpdateOpacity(potentialMatches, i); 
            yield return new WaitForSeconds(AnimationDelay); 
        } 
 
        for (float i = 0.3f; i <= 1f; i += 0.1f) 
        { 
            UpdateOpacity(potentialMatches, i); 
            yield return new WaitForSeconds(AnimationDelay); 
        } 
    } 
 
    private static void UpdateOpacity(IEnumerable<GameObject> objects, float opacity) 
    { 
        foreach (var item in objects) 
        { 
            Color c = item.GetComponent<SpriteRenderer>().color; 
            c.a = opacity; 
            item.GetComponent<SpriteRenderer>().color = c; 
        } 
    } 
 
    public static bool AreVerticalOrHorizontalNeighbors(Shape s1, Shape s2) 
    { 
        return (s1.Column == s2.Column || s1.Row == s2.Row) 
            && Mathf.Abs(s1.Column - s2.Column) <= 1 
            && Mathf.Abs(s1.Row - s2.Row) <= 1; 
    } 
 
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
                    return matches[UnityEngine.Random.Range(0, matches.Count - 1)]; 
 
                if (row >= levelStaticData.Rows / 2 && matches.Count > 0 && matches.Count <= 2) 
                    return matches[UnityEngine.Random.Range(0, matches.Count - 1)]; 
            } 
        } 
 
        return null; 
    } 
 
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