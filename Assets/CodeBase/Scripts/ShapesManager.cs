using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;

public class ShapesManager : MonoBehaviour
{
    // public Text ScoreText; // Текст для отображения счета
    public ShapesArray shapes; // Массив фигур
    private int score; // Счет игры
    private Vector2 _initialPoint = new Vector2(-1.8f, -2.6f); // Нижний правый угол игрового поля
    public readonly Vector2 CandySize = new Vector2(0.7f, 0.7f); // Размер фигур
    private GameState _state = GameState.None; // Текущее состояние игры
    private GameObject _hitGo = null; // Объект, по которому было совершено нажатие
    private Vector2[] _spawnPositions; // Позиции для появления новых фигур
    private IEnumerator _checkPotentialMatchesCoroutine; // Корутина для проверки возможных совпадений
    private IEnumerator _animatePotentialMatchesCoroutine; // Корутина для анимации возможных совпадений
    private IEnumerable<GameObject> _potentialMatches; // Коллекция объектов с возможными совпадениями
    private IAudioService _audioService; // Сервис звуков
    private IPersistentProgressService _progressService;

    private AudioSource _audioSource; // Источник звука

    private LevelStaticData _levelStaticData;

    public void Construct(Vector2 initialPoint, IAudioService audioService, IPersistentProgressService progressService, AudioSource audioSource, LevelStaticData levelStaticData)
    {
        _initialPoint = initialPoint;
        _levelStaticData = levelStaticData;
        _audioSource = audioSource;
        _audioService = audioService;
        _progressService = progressService;
    }
    public void Init()
    {
        InitializeTypesOnPrefabShapesAndBonuses();
        InitializeCandyAndSpawnPositions();
        StartCheckForPotentialMatches();
    }

    /// <summary>
    /// Инициализация типов для префабов фигур и бонусов
    /// </summary>
    private void InitializeTypesOnPrefabShapesAndBonuses()
    {
       for (int i = 0; i < _levelStaticData.PrefabsData.CandyPrefabs.Length; i++)
        {
            _levelStaticData.PrefabsData.CandyPrefabs[i].GetComponent<Shape>().Type = _levelStaticData.PrefabsData.CandyPrefabs[i].name;
        }

        for (int i = 0; i < _levelStaticData.PrefabsData.BonusPrefabs.Length; i++)
        {
            _levelStaticData.PrefabsData.BonusPrefabs[i].GetComponent<Shape>().Type = _levelStaticData.PrefabsData.CandyPrefabs
                .Where(x => x.GetComponent<Shape>().Type.Contains(_levelStaticData.PrefabsData.BonusPrefabs[i].name.Split('_')[1].Trim())).Single().name;
        }
    }

    /// <summary>
    /// Инициализация фигур и позиций для появления
    /// </summary>ss
     private void InitializeCandyAndSpawnPositions()
    {
        // InitializeVariables();
        if (shapes != null)
            DestroyAllCandy();
        shapes = new ShapesArray(_levelStaticData);
        _spawnPositions = new Vector2[_levelStaticData.Columns];
        for (int i = 0; i < _levelStaticData.Rows * _levelStaticData.Columns; i++)
        {
            int row = i / _levelStaticData.Columns;
            int column = i % _levelStaticData.Columns;
            GameObject newCandy = GetRandomCandy();
            //check if two previous horizontal are of the same type
            while (column >= 2 && shapes[row, column - 1].GetComponent<Shape>()
                .IsSameType(newCandy.GetComponent<Shape>())
                && shapes[row, column - 2].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>()))
            {
                newCandy = GetRandomCandy();
            }
            //check if two previous vertical are of the same type
            while (row >= 2 && shapes[row - 1, column].GetComponent<Shape>()
                .IsSameType(newCandy.GetComponent<Shape>())
                && shapes[row - 2, column].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>()))
            {
                newCandy = GetRandomCandy();
            }
            InstantiateAndPlaceNewCandy(row, column, newCandy);
        }
        SetupSpawnPositions();
    }

    /// <summary>
    /// Создание и размещение новой фигуры
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="newCandy"></param>
    private void InstantiateAndPlaceNewCandy(int row, int column, GameObject newCandy)
    {
        GameObject go = Instantiate(newCandy,
            _initialPoint + new Vector2(column * CandySize.x, row * CandySize.y), Quaternion.identity);

        //assign the specific properties
        go.GetComponent<Shape>().Assign(newCandy.GetComponent<Shape>().Type, row, column);
        shapes[row, column] = go;
    }

    /// <summary>
    /// Настройка позиций для появления новых фигур
    /// </summary>
     private void SetupSpawnPositions()
    {
        //create the spawn positions for the new shapes (will pop from the 'ceiling')
        for (int column = 0; column < _levelStaticData.Columns; column++)
        {
            _spawnPositions[column] = _initialPoint
                + new Vector2(column * CandySize.x, _levelStaticData.Rows * CandySize.y);
        }
    }

    /// <summary>
    /// Уничтожение всех фигур на поле
    /// </summary>
    private void DestroyAllCandy()
    {
        for (int i = 0; i < _levelStaticData.Rows * _levelStaticData.Columns; i++)
        {
            int row = i / _levelStaticData.Columns;
            int column = i % _levelStaticData.Columns;
            Destroy(shapes[row, column]);
        }
    }

     void Update()
    {
        if(_state == GameState.Win || _state == GameState.Lose)
            return;
            
        if (_state == GameState.None)
        {
            //user has clicked or touched
            if (Input.GetMouseButtonDown(0))
            {
                //get the hit position
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null) //we have a hit!!!
                {
                    _hitGo = hit.collider.gameObject;
                    _state = GameState.SelectionStarted;
                }
                
            }
        }
        else if (_state == GameState.SelectionStarted)
        {
            //user dragged
            if (Input.GetMouseButton(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                //we have a hit
                if (hit.collider != null && _hitGo != hit.collider.gameObject)
                {

                    //user did a hit, no need to show him hints 
                    StopCheckForPotentialMatches();

                    //if the two shapes are diagonally aligned (different row and column), just return
                    if (!Utilities.AreVerticalOrHorizontalNeighbors(_hitGo.GetComponent<Shape>(),
                        hit.collider.gameObject.GetComponent<Shape>()))
                    {
                        _state = GameState.None;
                    }
                    else
                    {
                        _state = GameState.Animating;
                        FixSortingLayer(_hitGo, hit.collider.gameObject);
                        StartCoroutine(FindMatchesAndCollapse(hit));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Изменение слоя сортировки для лучшего отображения при перетаскивании/анимации
    /// </summary>
    /// <param name="hitGo"></param>
    /// <param name="hitGo2"></param>
    private void FixSortingLayer(GameObject hitGo, GameObject hitGo2)
    {
        SpriteRenderer sp1 = hitGo.GetComponent<SpriteRenderer>();
        SpriteRenderer sp2 = hitGo2.GetComponent<SpriteRenderer>();
        if (sp1.sortingOrder <= sp2.sortingOrder)
        {
            sp1.sortingOrder = 1;
            sp2.sortingOrder = 0;
        }
    }

   private IEnumerator FindMatchesAndCollapse(RaycastHit2D hit2)
    {
        //get the second item that was part of the swipe
        GameObject hitGo2 = hit2.collider.gameObject;
        shapes.Swap(_hitGo, hitGo2);

        //move the swapped ones
        _hitGo.transform.DOMove(hitGo2.transform.position, ConstantsAnimation.AnimationDuration);
        hitGo2.transform.DOMove(_hitGo.transform.position, ConstantsAnimation.AnimationDuration);
        yield return new WaitForSeconds(ConstantsAnimation.AnimationDuration);

        //get the matches via the helper methods
        MatchesInfo hitGomatchesInfo = shapes.GetMatches(_hitGo);
        MatchesInfo hitGo2matchesInfo = shapes.GetMatches(hitGo2);

        var totalMatches = hitGomatchesInfo.MatchedCandy
            .Union(hitGo2matchesInfo.MatchedCandy).Distinct();

        //if user's swap didn't create at least a 3-match, undo their swap
        if (totalMatches.Count() < ConstantsGameLogic.MinimumMatches)
        {
            _hitGo.transform.DOMove(hitGo2.transform.position, ConstantsAnimation.AnimationDuration);
            hitGo2.transform.DOMove(_hitGo.transform.position, ConstantsAnimation.AnimationDuration);
            yield return new WaitForSeconds(ConstantsAnimation.AnimationDuration);

            shapes.UndoSwap();
        }

        //if more than 3 matches and no Bonus is contained in the line, we will award a new Bonus
        bool addBonus = totalMatches.Count() >= ConstantsGameLogic.MinimumMatchesForBonus &&
            !BonusTypeUtilities.ContainsDestroyWholeRowColumn(hitGomatchesInfo.BonusesContained) &&
            !BonusTypeUtilities.ContainsDestroyWholeRowColumn(hitGo2matchesInfo.BonusesContained);

        Shape hitGoCache = null;
        if (addBonus)
        {
            //get the game object that was of the same type
            GameObject sameTypeGo = hitGomatchesInfo.MatchedCandy.Count() > 0 ? _hitGo : hitGo2;
            hitGoCache = sameTypeGo.GetComponent<Shape>();
        }

        int timesRun = 1;
        while (totalMatches.Count() >= ConstantsGameLogic.MinimumMatches)
        {
            //increase score
            _progressService.Progress.WorldData.ScoreData.Add((totalMatches.Count() - 2) * ConstantsGameLogic.Match3Score);

            if (timesRun >= 2)
                _progressService.Progress.WorldData.ScoreData.Add(ConstantsGameLogic.SubsequentMatchScore);

            _audioService.PlayOneShotSound(SoundType.Match_Sound, _audioSource);

            foreach (GameObject item in totalMatches)
            {
                shapes.Remove(item);
                RemoveFromScene(item);
            }

            //check and instantiate Bonus if needed
            if (addBonus)
                CreateBonus(hitGoCache);

            addBonus = false;

            //get the columns that we had a collapse
            var columns = totalMatches.Select(go => go.GetComponent<Shape>().Column).Distinct();

            //the order the 2 methods below get called is important!!!
            //collapse the ones gone
            AlteredCandyInfo collapsedCandyInfo = shapes.Collapse(columns);
            //create new ones
            AlteredCandyInfo newCandyInfo = CreateNewCandyInSpecificColumns(columns);

            int maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);

            MoveAndAnimate(newCandyInfo.AlteredCandy, maxDistance);
            MoveAndAnimate(collapsedCandyInfo.AlteredCandy, maxDistance);

            //will wait for both of the above animations
            yield return new WaitForSeconds(ConstantsAnimation.MoveAnimationMinDuration * maxDistance);

            //search if there are matches with the new/collapsed items
            totalMatches = shapes.GetMatches(collapsedCandyInfo.AlteredCandy).
                Union(shapes.GetMatches(newCandyInfo.AlteredCandy)).Distinct();

            timesRun++;
        }

        _state = GameState.None;
        StartCheckForPotentialMatches();
    }

    private void CreateBonus(Shape hitGoCache)
    {
        GameObject Bonus = Instantiate(GetBonusFromType(hitGoCache.Type), _initialPoint
            + new Vector2(hitGoCache.Column * CandySize.x,
                hitGoCache.Row * CandySize.y), Quaternion.identity);
        shapes[hitGoCache.Row, hitGoCache.Column] = Bonus;
        Shape BonusShape = Bonus.GetComponent<Shape>();
        //will have the same type as the "normal" candy
        BonusShape.Assign(hitGoCache.Type, hitGoCache.Row, hitGoCache.Column);
        //add the proper Bonus type
        BonusShape.Bonus |= BonusType.DestroyWholeRowColumn;
    }

   private AlteredCandyInfo CreateNewCandyInSpecificColumns(IEnumerable<int> columnsWithMissingCandy)
    {
        AlteredCandyInfo newCandyInfo = new AlteredCandyInfo();

        //find how many null values the column has
        foreach (int column in columnsWithMissingCandy)
        {
            var emptyItems = shapes.GetEmptyItemsOnColumn(column);
            foreach (ShapeInfo item in emptyItems)
            {
                GameObject go = GetRandomCandy();
                GameObject newCandy = Instantiate(go, _spawnPositions[column], Quaternion.identity)
                    as GameObject;

                newCandy.GetComponent<Shape>().Assign(go.GetComponent<Shape>().Type, item.Row, item.Column);

                if (_levelStaticData.Rows - item.Row > newCandyInfo.MaxDistance)
                    newCandyInfo.MaxDistance = _levelStaticData.Rows - item.Row;

                shapes[item.Row, item.Column] = newCandy;
                newCandyInfo.AddCandy(newCandy);
            }
        }
        return newCandyInfo;
    }


    private void MoveAndAnimate(IEnumerable<GameObject> movedGameObjects, int distance)
    {
        foreach (GameObject item in movedGameObjects)
        {
            int row = item.GetComponent<Shape>().Row;
            int column = item.GetComponent<Shape>().Column;
            item.transform.DOMove(_initialPoint + new Vector2(column * CandySize.x, row * CandySize.y), 
                                    ConstantsAnimation.MoveAnimationMinDuration * distance);
        }
    }

    private void RemoveFromScene(GameObject item)
    {
        item.transform.DOScale(Vector3.one * 1.2f, ConstantsAnimation.ExplosionDuration / 2)
        .OnComplete(() =>
        {
            // Уменьшение объекта до минимального размера
            item.transform.DOScale(Vector3.zero, ConstantsAnimation.ExplosionDuration / 2)
                .OnComplete(() =>
                {
                    // Уничтожение объекта
                    Destroy(item);
                });
        });
    }


    private GameObject GetRandomCandy() => 
        _levelStaticData.PrefabsData.CandyPrefabs[Random.Range(0, _levelStaticData.PrefabsData.CandyPrefabs.Length)];

    private GameObject GetBonusFromType(string type)
    {
        string color = type.Split('_')[1].Trim();
        foreach (GameObject item in _levelStaticData.PrefabsData.BonusPrefabs)
        {
            if (item.GetComponent<Shape>().Type.Contains(color))
                return item;
        }
        throw new System.Exception("Wrong type");
    }

    private void StartCheckForPotentialMatches()
    {
        StopCheckForPotentialMatches();
        //get a reference to stop it later
        _checkPotentialMatchesCoroutine = CheckPotentialMatches();
        StartCoroutine(_checkPotentialMatchesCoroutine);
    }

    private void StopCheckForPotentialMatches()
    {
        if (_animatePotentialMatchesCoroutine != null)
            StopCoroutine(_animatePotentialMatchesCoroutine);
        if (_checkPotentialMatchesCoroutine != null)
            StopCoroutine(_checkPotentialMatchesCoroutine);
        ResetOpacityOnPotentialMatches();
    }

    private void ResetOpacityOnPotentialMatches()
    {
        if (_potentialMatches != null)
            foreach (GameObject item in _potentialMatches)
            {
                if (item == null) break;

                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = 1.0f;
                item.GetComponent<SpriteRenderer>().color = c;
            }
    }

    private IEnumerator CheckPotentialMatches()
    {
        yield return new WaitForSeconds(ConstantsGameLogic.WaitBeforePotentialMatchesCheck);
        _potentialMatches = Utilities.GetPotentialMatches(shapes, _levelStaticData);
        if (_potentialMatches != null)
        {
            while (true)
            {

                _animatePotentialMatchesCoroutine = Utilities.AnimatePotentialMatches(_potentialMatches);
                StartCoroutine(_animatePotentialMatchesCoroutine);
                yield return new WaitForSeconds(ConstantsGameLogic.WaitBeforePotentialMatchesCheck);
            }
        }
    }
}