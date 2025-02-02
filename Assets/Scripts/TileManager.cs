using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] int sizeColumn;
    [SerializeField] int sizeRow;

    [SerializeField] TileObject[,] tileMap;
    [SerializeField] GameObject prefabTile;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    private void Reset()
    {
        if (tileMap == null)
        {
            tileMap = new TileObject[sizeColumn, sizeRow];
        }

        for (int i = 0; i < sizeColumn; i++)
        {
            for (int j = 0; j < sizeRow; j++)
            {
                var newTile = GameObject.Instantiate(prefabTile).GetComponent<TileObject>();
                if (newTile != null)
                {
                    tileMap[i, j] = newTile;
                    newTile.Setup(i,j, Random.Range(1,10));
                    newTile.transform.parent = transform;
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
