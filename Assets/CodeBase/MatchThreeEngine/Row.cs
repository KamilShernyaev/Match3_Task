using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Row : MonoBehaviour
{
    public List<Tile> tiles;

    public void Construct(Tile tile)
    {
        tiles.Add(tile);
    }
}

