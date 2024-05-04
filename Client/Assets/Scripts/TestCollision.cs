using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestCollision : MonoBehaviour
{
    public Tilemap _tilemap;
    public Tile _tile;

    private void Start() 
    {
        _tilemap.SetTile(new Vector3Int(0,0,0),_tile);

        List<Vector3Int> blocked = new List<Vector3Int>();

        foreach (var pos in _tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _tilemap.GetTile(pos);
            if (!tile)
            {
                Debug.Log($"{pos} is Blocked");
                blocked.Add(pos);
            }
        }

    }

    private void Update() {

    }

}
