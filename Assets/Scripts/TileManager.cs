using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    //[SerializeField] int sizeColumn;
    //[SerializeField] int sizeRow;

    [SerializeField] TileObject[,] tileMap;
    [SerializeField] GameObject prefabTile;

    [SerializeField] GameData gameData;

    public static TileManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Reset();
    }

    private void Reset()
    {
        if (tileMap == null)
        {
            tileMap = new TileObject[gameData.rowSIze, gameData.columnSIze];
        }

        for (int i = 0; i < gameData.rowSIze; i++)
        {
            for (int j = 0; j < gameData.columnSIze; j++)
            {
                var newTile = GameObject.Instantiate(prefabTile).GetComponent<TileObject>();
                if (newTile != null)
                {
                    tileMap[i,j] = newTile;
                    newTile.Setup(j,i, Random.Range(1,10));
                    newTile.transform.parent = transform;
                }
            }
        }

    }

    public TileObject GetTile(int col, int row)
    {
        return tileMap[row, col];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
