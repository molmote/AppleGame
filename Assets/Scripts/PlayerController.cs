using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool isDragging = false;
    [SerializeField] List<TileObject> tileObjects = new List<TileObject>();

    TileObject previousHit;
    void Update()
    {
        if (isDragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                 Debug.Log("Stopped Dragging");
                tileObjects.Clear();
                previousHit = null;
                isDragging = false;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                var newTile = hit.collider.GetComponent<TileObject>();
                if (hit.collider != null && previousHit != newTile)
                {
                    Debug.Log($"IsDragging over {hit}");
                    tileObjects.Add(newTile);
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
