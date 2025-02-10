using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private bool isSelecting = false;
    private List<TileObject> tileObjects = new List<TileObject>();

    [SerializeField] GameData gameData; // uses scriptable object for customized settings

    [SerializeField] TextMeshProUGUI uiTextScore;

    private int score = 0;

    private void UpdateScore(int _score)
    {
        score += _score;

        uiTextScore.text = $"score: {score}";
    }

    public void ResetVisited()
    {
        for (int i = 0; i < gameData.columnSIze; i++)
        {
            for (int j = 0; j < gameData.rowSIze; j++)
            {
                // iterationsForOneSearch++;

                var tile = TileManager.Instance.GetTile(i, j);
                    tile.IsVisited = (false);
            }
        }
    }

    public void SelectTilesBlindly(out int selectX, out int selectY)
    {
        for (int i = 0; i < gameData.columnSIze; i++)
        {
            for (int j = 0; j < gameData.rowSIze; j++)
            {
                // iterationsForOneSearch++;

                var tile = TileManager.Instance.GetTile(i, j);

                if (tile == null)
                {
                    selectX = -1;
                    selectY = -1;
                    return;
                }
                if (tile.IsActive() && !tile.IsVisited)
                {
                    selectX = i;
                    selectY = j;

                    tile.IsVisited = (true);

                    return;
                }
            }
        }

        selectX = -1;
        selectY = -1;
    }

    public bool CheckTilesBlindly(int selectX, int selectY)
    {
        var startTile = TileManager.Instance.GetTile(selectX, selectY);
        if (startTile == null)
        {
            return false;
        }
        int scoreTotal = startTile.GetNumber();

        startTile.ToggleSelectedOnly(true);
        tileObjects.Add(startTile);

        int compareRadius = 1;
        //check for 8 sides if sum is 10
        for (int x = -compareRadius; x < compareRadius; x++)
        {
            for (int y = -compareRadius; y < compareRadius; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int lx = selectX + x;
                int ly = selectY + y;

                if (lx < 0 || ly < 0)
                {
                    continue;
                }

                int startx = x>0 ? selectX : lx;
                int starty = y>0 ? selectY : ly;
                int endx = x>0 ? lx : selectX;
                int endy = y>0 ? ly : selectY;

                MyLogger.Log($"start looping from ({startx},{starty}) to ({endx},{endy})");
                for (int i = startx; i <= endx; i++)
                {
                    for (int j = starty; j <= endy; j++)
                    {
                        // MyLogger.Log($"try adding ({i},{j}) tile to the queue");
                        var tileCompare = TileManager.Instance.GetTile(i, j);
                        iterationsForOneSearch++;

                        // doesn't add hidden block 
                        if (tileCompare.IsActive() && startTile.GetNumber() + tileCompare.GetNumber() == 10)
                        {
                            tileCompare.ToggleSelectedOnly(true);
                            tileObjects.Add(tileCompare);

                            return true;
                        }
                    }
                }
            }
        }

        startTile.ToggleSelectedOnly(false);
        tileObjects.Clear();
        return false;
    }

    float delay;
    int iterationsForOneSearch = 0;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:사용되지 않는 private 멤버 제거", Justification = "<보류 중>")]
    void Update()
    {
        delay += Time.deltaTime;
        if (isSelecting && delay > gameData.delayForAISelect)
        {
            isSelecting = false;
            iterationsForOneSearch = 0;

            int selectY = 0, selectX = 0;
            bool nextSolution = true;
            while (nextSolution && selectX >= 0 && selectY >= 0)
            {
                SelectTilesBlindly(out selectX, out selectY);
                nextSolution = !CheckTilesBlindly(selectX, selectY);
            }

            ResetVisited();
            if (nextSolution)
            {
                Debug.LogError("Can't find solution");
            }
            else
            {
                Debug.Log($"Found the solution at {selectX}, {selectY}");
            }
            delay = 0;
        }
        else if (!isSelecting && delay > gameData.delayForAIScoreAnimation)
        {
            delay = 0;
            isSelecting = true;
            UpdateScore(2);
            HideSelection();
        }

    }

    public void HideSelection()
    {
        foreach (var tileObject in tileObjects)
        {
            tileObject.SetActive(false, true);
        }

        tileObjects.Clear();
    }

    /*public void ClearSelection()
    {
        foreach (var tileObject in tileObjects)
        {
            tileObject.ToggleSelected(false);
        }

        tileObjects.Clear();
        // sumTotal = 0;
    }*/
}
