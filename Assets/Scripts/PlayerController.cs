using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isDragging = false;
    private List<TileObject> tileObjects = new List<TileObject>();
    private int sumTotal;

    [SerializeField] GameData gameData; // uses scriptable object for customized settings

    private TileObject lastHit;
    private TileObject firstHit;

    [SerializeField] TextMeshProUGUI uiTextTime;
    [SerializeField] TextMeshProUGUI uiTextScore;

    private float time = 0;
    private int score = 0;

    private void UpdateScore(int _score)
    {
        score += _score;

        uiTextScore.text = score.ToString();
    }

    private void UpdateTime(float time)
    {
        uiTextTime.text = ((int)time).ToString();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:사용되지 않는 private 멤버 제거", Justification = "<보류 중>")]
    void Update()
    {
        time += Time.deltaTime;
        UpdateTime(time);

        if (isDragging)
        {
            // dragging finished
            if (Input.GetMouseButtonUp(0))
            {
                MyLogger.Log("Stopped Dragging");
                // if scored, remove(hide) selected blocks 
                if (sumTotal == 10)
                {
                    UpdateScore (tileObjects.Count);
                    HideSelection();
                }
                // if not, just return them to unselected state
                else
                {
                    ClearSelection();
                }

                firstHit = lastHit = null;
                isDragging = false;
            }
            // dragging  
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                if (hit.collider == null)   // corresponding block is not active
                {
                    return;
                }
                var newTile = hit.collider.GetComponent<TileObject>();

                RefreshSelection(newTile);
            }
        }
        else
        {
            // dragging started
            if (Input.GetMouseButtonDown(0))
            {
                MyLogger.Log("Started Dragging");
                isDragging = true;
            }
        }
    }

    public void HideSelection()
    {
        foreach (var tileObject in tileObjects)
        {
            tileObject.SetActive(false);
        }

        tileObjects.Clear();
        sumTotal = 0;
    }

    public void ClearSelection()
    {
        foreach (var tileObject in tileObjects)
        {
            tileObject.ToggleSelected(false);
        }

        tileObjects.Clear();
        sumTotal = 0;
    }

    public void RefreshSelection(TileObject newTile) 
    {
        MyLogger.Log($"detected tile is at ({newTile.GetColumn()},{newTile.GetRow()})");

        // this is first selected tile
        if (firstHit == null)
        {
            firstHit = newTile;
            lastHit = newTile;

            newTile.ToggleSelected(true);
            tileObjects.Add(newTile);
            sumTotal += newTile.GetNumber();
        }
        // the selection area is changed, add/remove tiles correspondingly
        else if (lastHit != newTile)
        {
            // clear previous selection and sum total
            ClearSelection();

            lastHit = newTile;
            int fx = firstHit.GetColumn();
            int fy = firstHit.GetRow();
            int lx = lastHit.GetColumn();
            int ly = lastHit.GetRow();
            int startx = fx > lx ? lx : fx;
            int starty = fy > ly ? ly : fy;
            int endx = fx > lx ? fx : lx;
            int endy = fy > ly ? fy : ly;

            MyLogger.Log($"start looping from ({startx},{starty}) to ({endx},{endy})");
            for (int i = startx; i <= endx; i++)
            {
                for (int j = starty; j <= endy; j++)
                {
                    // MyLogger.Log($"try adding ({i},{j}) tile to the queue");
                    var tile = TileManager.Instance.GetTile(i, j);

                    // doesn't add hidden block 
                    if ( tile.IsActive() )
                    {
                        tile.ToggleSelected(true);
                        tileObjects.Add(tile);
                        sumTotal += tile.GetNumber();
                    }
                }
            }
        }
        // there is no change to selection area
        /*else
        {
            MyLogger.Log($"detected tile is same as previous");
        }*/
    }
}
