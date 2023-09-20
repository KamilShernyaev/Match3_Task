using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;

/// <summary> 
/// Класс, отвечающий за логику игрового поля и управление фигурами 
/// </summary> 
public class Board : MonoBehaviour
{
    public ShapesArray shapes; // Массив фигур
    private Vector2 _initialPoint;// Нижний правый угол игрового поля
    public readonly Vector2 CandySize = new Vector2(0.7f, 0.7f); // Размер фигур
    private GameObject _hitGo = null; // Объект, по которому было совершено нажатие
    private Vector2[] _spawnPositions; // Позиции для появления новых фигур
    private IEnumerator _checkPotentialMatchesCoroutine; // Корутина для проверки возможных совпадений
    private IEnumerator _animatePotentialMatchesCoroutine; // Корутина для анимации возможных совпадений
    private IEnumerable<GameObject> _potentialMatches; // Коллекция объектов с возможными совпадениями
    private IAudioService _audioService; // Сервис звуков
    private IPersistentProgressService _progressService;
    private AudioSource _audioSource; // Источник звука
    private LevelStaticData _levelStaticData;
    private ResultManager _resultManager;

    public void Construct(Vector2 initialPoint, IAudioService audioService, IPersistentProgressService progressService, AudioSource audioSource, LevelStaticData levelStaticData)
    {
        _initialPoint = initialPoint;
        _levelStaticData = levelStaticData;
        _audioSource = audioSource;
        _audioService = audioService;
        _progressService = progressService;
    }
    public void Init(ResultManager resultManager)
    {
        _resultManager = resultManager;
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
        if (shapes != null)
            DestroyAllCandy();
        shapes = new ShapesArray(_levelStaticData);
        _spawnPositions = new Vector2[_levelStaticData.Columns];
        for (int i = 0; i < _levelStaticData.Rows * _levelStaticData.Columns; i++)
        {
            int row = i / _levelStaticData.Columns;
            int column = i % _levelStaticData.Columns;
            GameObject newCandy = GetRandomCandy();
            //Проверка того, что две предыдущие горизонтали имеют одинаковый тип
            while (column >= 2 && shapes[row, column - 1].GetComponent<Shape>()
                .IsSameType(newCandy.GetComponent<Shape>())
                && shapes[row, column - 2].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>()))
            {
                newCandy = GetRandomCandy();
            }
            //Проверка того, что две предыдущие вертикали имеют одинаковый тип
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
        //создаем места спавна для новых фигур, пояляются сверху как бы падая
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

    private void Update()
    {
        if (_resultManager.State == GameState.Win || _resultManager.State == GameState.Lose)
            return;
        
        if (_resultManager.State == GameState.None)
        {
            // Пользователь нажал или коснулся экрана
            if (Input.GetMouseButtonDown(0))
            {
                // Получаем позицию нажатия
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                
                if (hit.collider != null) // У нас есть попадание!!!
                {
                    _hitGo = hit.collider.gameObject;
                    _resultManager.State = GameState.SelectionStarted;
                }
            }
        }
        else if (_resultManager.State == GameState.SelectionStarted)
        {
            // Пользователь проводит пальцем или мышью
            if (Input.GetMouseButton(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                
                // У нас есть попадание
                if (hit.collider != null && _hitGo != hit.collider.gameObject)
                {
                    // Пользователь сделал попадание, не нужно показывать подсказки
                    StopCheckForPotentialMatches();
                    
                    // Если две фигуры находятся по диагонали (разные строки и столбцы), просто выходим из метода
                    if (!Utilities.AreVerticalOrHorizontalNeighbors(_hitGo.GetComponent<Shape>(),
                        hit.collider.gameObject.GetComponent<Shape>()))
                    {
                        _resultManager.State = GameState.None;
                    }
                    else
                    {
                        _resultManager.State = GameState.Animating;
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
        // Получаем второй элемент, который был частью свайпа
        GameObject hitGo2 = hit2.collider.gameObject;
        shapes.Swap(_hitGo, hitGo2);
        // Перемещаем элементы, которые были поменяны местами
        _hitGo.transform.DOMove(hitGo2.transform.position, ConstantsAnimation.AnimationDuration);
        hitGo2.transform.DOMove(_hitGo.transform.position, ConstantsAnimation.AnimationDuration);
        yield return new WaitForSeconds(ConstantsAnimation.AnimationDuration);
        // Получаем совпадения с помощью вспомогательных методов
        MatchesInfo hitGomatchesInfo = shapes.GetMatches(_hitGo);
        MatchesInfo hitGo2matchesInfo = shapes.GetMatches(hitGo2);
        var totalMatches = hitGomatchesInfo.MatchedCandy
            .Union(hitGo2matchesInfo.MatchedCandy).Distinct();
        // Если свайп пользователя не создал хотя бы 3 совпадения, отменяем свайп
        if (totalMatches.Count() < ConstantsGameLogic.MinimumMatches)
        {
            _hitGo.transform.DOMove(hitGo2.transform.position, ConstantsAnimation.AnimationDuration);
            hitGo2.transform.DOMove(_hitGo.transform.position, ConstantsAnimation.AnimationDuration);
            yield return new WaitForSeconds(ConstantsAnimation.AnimationDuration);
            shapes.UndoSwap();
        }
        // Если совпадений больше 3 и в линии нет бонуса, создаем новый бонус
        bool addBonus = totalMatches.Count() >= ConstantsGameLogic.MinimumMatchesForBonus &&
            !BonusTypeUtilities.ContainsDestroyWholeRowColumn(hitGomatchesInfo.BonusesContained) &&
            !BonusTypeUtilities.ContainsDestroyWholeRowColumn(hitGo2matchesInfo.BonusesContained);
        Shape hitGoCache = null;
        if (addBonus)
        {
            // Получаем объект, который имеет тот же тип
            GameObject sameTypeGo = hitGomatchesInfo.MatchedCandy.Count() > 0 ? _hitGo : hitGo2;
            hitGoCache = sameTypeGo.GetComponent<Shape>();
        }
        int timesRun = 1;
        while (totalMatches.Count() >= ConstantsGameLogic.MinimumMatches)
        {
            // Увеличиваем счет
            _progressService.Progress.WorldData.ScoreData.Add((totalMatches.Count() - 2) * ConstantsGameLogic.Match3Score);
            if (timesRun >= 2)
                _progressService.Progress.WorldData.ScoreData.Add(ConstantsGameLogic.SubsequentMatchScore);
            _audioService.PlayOneShotSound(SoundType.Match_Sound, _audioSource);
            foreach (GameObject item in totalMatches)
            {
                shapes.Remove(item);
                RemoveFromScene(item);
            }
            // Проверяем и создаем бонус при необходимости
            if (addBonus)
                CreateBonus(hitGoCache);
            addBonus = false;
            // Получаем столбцы, в которых произошел обрушение
            var columns = totalMatches.Select(go => go.GetComponent<Shape>().Column).Distinct();
            // Обрушаем ушедшие элементы
            AlteredCandyInfo collapsedCandyInfo = shapes.Collapse(columns);
            // Создаем новые элементы
            AlteredCandyInfo newCandyInfo = CreateNewCandyInSpecificColumns(columns);
            int maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);
            MoveAndAnimate(newCandyInfo.AlteredCandy, maxDistance);
            MoveAndAnimate(collapsedCandyInfo.AlteredCandy, maxDistance);
            // Ждем окончания анимаций
            yield return new WaitForSeconds(ConstantsAnimation.MoveAnimationMinDuration * maxDistance);
            // Проверяем, есть ли совпадения с новыми/обрушенными элементами
            totalMatches = shapes.GetMatches(collapsedCandyInfo.AlteredCandy).
                Union(shapes.GetMatches(newCandyInfo.AlteredCandy)).Distinct();
            timesRun++;
        }
        _resultManager.State = GameState.None;
        StartCheckForPotentialMatches();
    }

   /// <summary>
    /// Создание бонуса на основе типа фигуры
    /// </summary>
    /// <param name="hitGoCache"></param>
    private void CreateBonus(Shape hitGoCache)
    {
        // Создаем бонусный объект на основе типа фигуры
        GameObject Bonus = Instantiate(GetBonusFromType(hitGoCache.Type), _initialPoint
            + new Vector2(hitGoCache.Column * CandySize.x,
                hitGoCache.Row * CandySize.y), Quaternion.identity);
        shapes[hitGoCache.Row, hitGoCache.Column] = Bonus;
        Shape BonusShape = Bonus.GetComponent<Shape>();
        // Бонус будет иметь тот же тип, что и "обычная" фигура
        BonusShape.Assign(hitGoCache.Type, hitGoCache.Row, hitGoCache.Column);
        // Добавляем соответствующий тип бонуса
        BonusShape.Bonus |= BonusType.DestroyWholeRowColumn;
    }

   /// <summary>
    /// Создание новых фигур в указанных столбцах, где отсутствуют фигуры
    /// </summary>
    /// <param name="columnsWithMissingCandy">Столбцы, в которых отсутствуют фигуры</param>
    /// <returns>Информация о созданных новых фигурах</returns>
    private AlteredCandyInfo CreateNewCandyInSpecificColumns(IEnumerable<int> columnsWithMissingCandy)
    {
        AlteredCandyInfo newCandyInfo = new AlteredCandyInfo();
        // Определяем количество пустых значений в столбце
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

    /// <summary>
    /// Перемещение и анимация указанных игровых объектов на заданное расстояние
    /// </summary>
    /// <param name="movedGameObjects">Перемещаемые игровые объекты</param>
    /// <param name="distance">Расстояние перемещения</param>
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

    /// <summary>
    /// Удаление объекта из сцены с анимацией
    /// </summary>
    /// <param name="item">Удаляемый объект</param>
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

    /// <summary>
    /// Получение бонусного объекта на основе типа фигуры
    /// </summary>
    /// <param name="type">Тип фигуры</param>
    /// <returns>Бонусный объект</returns>
    private GameObject GetBonusFromType(string type)
    {
        // Получаем цвет из типа фигуры
        string color = type.Split('_')[1].Trim();
        
        // Проверяем каждый бонусный объект в списке префабов
        foreach (GameObject item in _levelStaticData.PrefabsData.BonusPrefabs)
        {
            // Если тип фигуры бонусного объекта содержит цвет, возвращаем этот объект
            if (item.GetComponent<Shape>().Type.Contains(color))
                return item;
        }
        
        // Если не найден подходящий бонусный объект, выбрасываем исключение
        throw new System.Exception("Wrong type");
    }

    /// <summary>
    /// Запуск проверки возможных совпадений
    /// </summary>
    private void StartCheckForPotentialMatches()
    {
        StopCheckForPotentialMatches();
        // Получаем ссылку для остановки позже
        _checkPotentialMatchesCoroutine = CheckPotentialMatches();
        StartCoroutine(_checkPotentialMatchesCoroutine);
    }

    /// <summary>
    /// Остановка проверки возможных совпадений
    /// </summary>
    private void StopCheckForPotentialMatches()
    {
        if (_animatePotentialMatchesCoroutine != null)
            StopCoroutine(_animatePotentialMatchesCoroutine);
        if (_checkPotentialMatchesCoroutine != null)
            StopCoroutine(_checkPotentialMatchesCoroutine);
        ResetOpacityOnPotentialMatches();
    }

    /// <summary>
    /// Сброс прозрачности на возможных совпадениях
    /// </summary>
    private void ResetOpacityOnPotentialMatches()
    {
        if (_potentialMatches != null)
        {
            foreach (GameObject item in _potentialMatches)
            {
                if (item == null) break;
                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = 1.0f;
                item.GetComponent<SpriteRenderer>().color = c;
            }
        }
    }

    /// <summary>
    /// Проверка возможных совпадений
    /// </summary>
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