using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap fogOfWarTileMap;

    [Header("Settings")]
    [SerializeField] private Tile fogTile;
    [SerializeField] private Transform discoverer;
    [SerializeField] private int discoveryRadius = 11;

    private void Start() {
        discoverer = GameManager.instance.GetPlayer().transform;
    }

    // Update discovered tiles
    private void LateUpdate() {
        // Get location in map
        Vector2Int location = (Vector2Int) fogOfWarTileMap.WorldToCell(discoverer.transform.position);

        // Go through every location around
        for (int i = location.x - discoveryRadius; i <= location.x + discoveryRadius; i++)
        {
            for (int j = location.y - discoveryRadius; j <= location.y + discoveryRadius; j++)
            {
                
                if ((i - location.x) * (i - location.x) + (j - location.y) * (j - location.y) < discoveryRadius * discoveryRadius) { 
                    fogOfWarTileMap.SetTile(new Vector3Int(i, j, 0), null);
                }
                
            }
        }
    }

    public void generateFogOfWar(int sizeX, int sizeY) {
        // Create a fog tile in every location
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                fogOfWarTileMap.SetTile(new Vector3Int(i, j, 0), fogTile);
            }
        }
    }

}
