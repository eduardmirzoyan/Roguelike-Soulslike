using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator instance;

    [Header("Components")]
    [SerializeField] private Minimap minimap;
    [SerializeField] private FogOfWar fogOfWar;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private RuleTile groundTile;
    
    [Header("Settings")]
    [SerializeField] private int sizeX;
    [SerializeField] private int sizeY;

    private void Awake() {
        // Singleton logic
        if(DungeonGenerator.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        // Get references
        minimap = GetComponentInChildren<Minimap>();
        fogOfWar = GetComponentInChildren<FogOfWar>();

        // Generate map
        minimap.generateMinimap(sizeX, sizeY);
        fogOfWar.generateFogOfWar(sizeX, sizeY);
    }

    private void generateDungeon() {
        // Fill box with tiles
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (i == 0 || j == 0 || i == sizeX - 1 || j == sizeY - 1)
                    groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTile);
            }
        }
    }
}
