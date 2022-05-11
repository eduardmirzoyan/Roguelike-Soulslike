using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Minimap : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap groundTileMap;
    [SerializeField] private Tilemap minimapTileMap;
    [SerializeField] private Camera minimapCamera;

    [Header("Tiles")]
    [SerializeField] private Tile groundTile;
    [SerializeField] private Tile emptyTile;
    
    private void Awake() {
        minimapCamera = GetComponentInChildren<Camera>();
    }

    public void generateMinimap(int sizeX, int sizeY) {
        // Generate minimap tiles based on ground tilemap
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (groundTileMap.HasTile(new Vector3Int(i, j, 0))) {
                    minimapTileMap.SetTile(new Vector3Int(i, j, 0), groundTile);
                }
                else {
                    minimapTileMap.SetTile(new Vector3Int(i, j, 0), emptyTile);
                }
            }
        }
        
        // Setup camera afterwards
        setupCamera(sizeX, sizeY);
    }

    private void setupCamera(int sizeX, int sizeY) {
        // Set position of camera
        minimapCamera.transform.position = new Vector3(sizeX / 2, sizeY / 2, -10);

        // Set zoom
        minimapCamera.orthographicSize = sizeX / 2;
    }

    
}
