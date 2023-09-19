using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class Board : MonoBehaviour
{
    [SerializeField] private TileTypeAsset[] tileTypes;
    [SerializeField] private List<Row> rows;
    private AudioSource _audioSource;
    [SerializeField] private float tweenDuration;
    [SerializeField] private Transform swappingOverlay;
    private readonly List<Tile> _selection = new List<Tile>();
    private bool _isSwapping;
    private bool _isMatching;
    private bool _isShuffling;
    private IAudioService _audioService;
    private int score;


    private TileData[,] Matrix
    {
        get
        {
            var width = rows.Max(row => row.tiles.Count);
            var height = rows.Count;
            var data = new TileData[width, height];
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    data[x, y] = GetTile(x, y).Data;
            return data;
        }
    }

    public void Construct(Row row, AudioSource audioSource)
    {
        rows.Add(row);
        _audioSource = audioSource;
        _audioService = AllServices.Container.Single<IAudioService>();
    }

    public void Init()
    {
        var maxLength = rows.Max(row => row.tiles.Count);
        for (var y = 0; y < rows.Count; y++)
        {
            for (var x = 0; x < maxLength; x++)
            {
                var tile = GetTile(x, y);
                tile.x = x;
                tile.y = y;
                tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];
                tile.button.onClick.AddListener(async () => await Select(tile));
            }
        }   
    }

    private Tile GetTile(int x, int y) => rows[y].tiles[x];

    private Tile[] GetTiles(IList<TileData> tileData)
    {
        var length = tileData.Count;
        var tiles = new Tile[length];
        for (var i = 0; i < length; i++) tiles[i] = GetTile(tileData[i].X, tileData[i].Y);
        return tiles;
    }

    private async Task Select(Tile tile)
    {
        // _audioService.PlayOneShotSound(SoundType.Select_Click_Sound, audioSource);
        tile.icon.transform.DOShakeScale(0.5f, 0.5f).Play();
        if (_isSwapping || _isMatching || _isShuffling) return;
        if (!_selection.Contains(tile))
        {
            if (_selection.Count > 0)
            {
                if (Math.Abs(tile.x - _selection[0].x) == 1 && Math.Abs(tile.y - _selection[0].y) == 0
                    || Math.Abs(tile.y - _selection[0].y) == 1 && Math.Abs(tile.x - _selection[0].x) == 0)
                    _selection.Add(tile);
            }
            else
            {
                _selection.Add(tile);
            }
        }
        if (_selection.Count < 2) return;
        await SwapAsync(_selection[0], _selection[1]);
        if (!await TryMatchAsync()) await SwapAsync(_selection[0], _selection[1]);
        var matrix = Matrix;
        while (TileDataMatrixUtility.FindPossibleMove(matrix) == null || TileDataMatrixUtility.FindPossibleMatch(matrix) != null)
        {
            Shuffle();
            matrix = Matrix;
        }
        _selection.Clear();
    }

    private async Task SwapAsync(Tile tile1, Tile tile2)
    {
        _isSwapping = true;
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;
        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;
        icon1Transform.SetParent(swappingOverlay);
        icon2Transform.SetParent(swappingOverlay);
        icon1Transform.SetAsLastSibling();
        icon2Transform.SetAsLastSibling();
        var sequence = DOTween.Sequence();
        sequence.Join(icon1Transform.DOMove(icon2Transform.position, tweenDuration).SetEase(Ease.OutBack))
            .Join(icon2Transform.DOMove(icon1Transform.position, tweenDuration).SetEase(Ease.OutBack));
        await sequence.Play()
            .AsyncWaitForCompletion();
        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);
        tile1.icon = icon2;
        tile2.icon = icon1;
        var tile1Item = tile1.Type;
        tile1.Type = tile2.Type;
        tile2.Type = tile1Item;
        _isSwapping = false;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Shuffle();
        }
    }

    private async Task<bool> TryMatchAsync()
    {
        var didMatch = false;
        _isMatching = true;
        var match = TileDataMatrixUtility.FindPossibleMatch(Matrix);
        while (match != null)
        {
            didMatch = true;
            var tiles = GetTiles(match.Tiles);
            var deflateSequence = DOTween.Sequence();
            foreach (var tile in tiles) deflateSequence.Join(tile.icon.transform.DOScale(Vector3.zero, tweenDuration).SetEase(Ease.InBack));
            _audioService.PlayOneShotSound(SoundType.Match_Sound, _audioSource);
            await deflateSequence.Play()
                .AsyncWaitForCompletion();
            var inflateSequence = DOTween.Sequence();
            foreach (var tile in tiles)
            {
                tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];
                inflateSequence.Join(tile.icon.transform.DOScale(Vector3.one, tweenDuration).SetEase(Ease.OutBack));
            }
            await inflateSequence.Play()
                .AsyncWaitForCompletion();
            match = TileDataMatrixUtility.FindPossibleMatch(Matrix);
        }
        _isMatching = false;
        return didMatch;
    }

    private void Shuffle()
    {
        _isShuffling = true;
        foreach (var row in rows)
            foreach (var tile in row.tiles)
                tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];
        _isShuffling = false;
    }
}