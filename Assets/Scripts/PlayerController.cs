using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool isDragging = false;
    [SerializeField] List<TileObject> tileObjects = new List<TileObject>();
    [SerializeField] int sumTotal;

    [SerializeField] GameData gameData;

    TileObject lastHit;
    TileObject firstHit;

    void Update()
    {
        if (isDragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (sumTotal == 10)
                {
                    foreach (var tileObject in tileObjects)
                    {
                        tileObject.SetActive(false);
                    }
                }
                else
                {
                    foreach (var tileObject in tileObjects)
                    {
                        tileObject.ToggleSelected(false);
                    }
                }

                Debug.Log("Stopped Dragging");
                tileObjects.Clear();
                sumTotal = 0;
                firstHit = lastHit = null;
                isDragging = false;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                if (hit.collider == null)
                {
                    return;
                }
                var newTile = hit.collider.GetComponent<TileObject>();

                SelectTile(newTile);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Started Dragging");
                isDragging = true;

                /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                if (hit.collider == null)
                {
                    return;
                }
                var newTile = hit.collider.GetComponent<TileObject>();

                SelectTile(newTile);*/
            }
        }
    }

    public void SelectTile(TileObject newTile) 
    {
        Debug.Log($"detected tile is at ({newTile.GetColumn()},{newTile.GetRow()})");

        if (firstHit == null)
        {
            firstHit = newTile;
            lastHit = newTile;

            newTile.ToggleSelected(true);
            tileObjects.Add(newTile);
            sumTotal += newTile.GetNumber();
        }

        else if (lastHit != newTile)
        {
            lastHit = newTile;
            Debug.Log($"detected tile nor the first or the last, start the logic");
            int fx = firstHit.GetColumn();
            int fy = firstHit.GetRow();
            int lx = lastHit.GetColumn();
            int ly = lastHit.GetRow();
            int startx = fx > lx ? lx : fx;
            int starty = fy > ly ? ly : fy;
            int endx = fx > lx ? fx : lx;
            int endy = fy > ly ? fy : ly;

            Debug.Log($"start looping from ({startx},{starty}) to ({endx},{endy})");
            for (int i = startx; i <= endx; i++)
            {
                for (int j = starty; j <= endy; j++)
                {
                    Debug.Log($"try adding ({i},{j}) tile to the queue");
                    var tile = TileManager.Instance.GetTile(i, j);
                    if (!tile.IsSelected()) // skip if already selected
                    {
                        tile.ToggleSelected(true);
                        tileObjects.Add(tile);
                        sumTotal += tile.GetNumber();
                    }
                }
            }
        }
        /*else
        {
            Debug.Log($"detected tile is same as previous");
        }*/
    }
}
