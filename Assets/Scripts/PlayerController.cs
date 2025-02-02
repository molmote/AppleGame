using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool isDragging = false;
    [SerializeField] List<TileObject> tileObjects = new List<TileObject>();
    [SerializeField] int sumTotal;

    TileObject previousHit;
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
                previousHit = null;
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
                if (hit.collider != null && previousHit != newTile)
                {
                    //Debug.Log($"IsDragging over {hit}");
                    newTile.ToggleSelected(true);
                    tileObjects.Add(newTile);
                    sumTotal += newTile.GetNumber();
                }
                previousHit = newTile;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Started Dragging");
                isDragging = true;
            }
        }
    }
}
