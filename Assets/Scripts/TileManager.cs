using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    //[SerializeField] int sizeColumn;
    //[SerializeField] int sizeRow;

    [SerializeField] TileObject[,] tileMap;
    [SerializeField] GameObject prefabTile;

    [SerializeField] GameData gameData;
    [SerializeField] TextMeshProUGUI utTextSeed;

    public static TileManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        ResetTiles();
    }

    private void ResetTiles(int seed = -1)
    {
        if (tileMap == null)
        {
            tileMap = new TileObject[gameData.rowSIze, gameData.columnSIze];
        }

        // if not set
        if (seed == -1)
        {
            seed = Math.Abs((int)DateTime.Now.Ticks % 65536);
        }
        UnityEngine.Random.InitState(seed);
        utTextSeed.text = $"seed: {seed}";

        for (int i = 0; i < gameData.rowSIze; i++)
        {
            for (int j = 0; j < gameData.columnSIze; j++)
            {
                var newTile = GameObject.Instantiate(prefabTile).GetComponent<TileObject>();
                if (newTile != null)
                {
                    tileMap[i,j] = newTile;
                    newTile.Setup(j,i, UnityEngine.Random.Range(1,10));
                    newTile.transform.parent = transform;
                }
            }
        }

    }

    public TileObject GetTile(int col, int row)
    {
        if (row < 0 || col < 0 || row >= gameData.rowSIze || col >=  gameData.columnSIze)
            return null;

        return tileMap[row, col];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
