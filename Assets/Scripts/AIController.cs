using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private bool isSelecting = false;
    private List<TileObject> tileObjects = new List<TileObject>();

    [SerializeField] GameData gameData; // uses scriptable object for customized settings

    [SerializeField] TextMeshProUGUI uiTextScore;

    Dictionary<Tuple<int, int>, int> valuesTable = new Dictionary<Tuple<int, int>, int>();

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

    private int FindCostRecursively (int x, int y, int baseCost)
    {
        var key = new Tuple<int, int>(x, y);
        if (valuesTable.ContainsKey(key) )
        {
            return valuesTable[key];
        }

        if (x == 0 && y == 0)
        {
            MyLogger.Log("returns 0");
            valuesTable[key] = 0;
            return 0;
        }

        if (x == 0)
            return 0;

        int modx = x - 1;
        int mody = y - 1;
        if (x < 0)
        {
            modx = x + 1;
        }
        if (y < 0)
        {
            mody = y + 1;
        }

        var myTile = TileManager.Instance.GetTile(x, y);
        int myCost = FindCostRecursively(modx, y, baseCost) + FindCostRecursively(x, mody, baseCost)
            - FindCostRecursively(modx, mody, baseCost) + myTile.GetNumber();

        return myCost;
    }

    private void StartAgain(TileObject start, ref int scoreTotal)
    {
        scoreTotal = start.GetNumber();
        start.ToggleSelectedOnly(true);
        tileObjects.Clear();
        tileObjects.Add(start);
    }

    public bool CheckTilesBlindly(int selectX, int selectY)
    {
        var startTile = TileManager.Instance.GetTile(selectX, selectY);
        if (startTile == null)
        {
            return false;
        }

        int compareRadius = 10;

        valuesTable.Clear();

        int scoreTotal = 0;
        StartAgain(startTile, ref scoreTotal);

        int[,] dir = new int[,] { { -1, 0 }, {0,-1}, {1,0}, {0,1}, 
            { -1, 1 }, {-1,-1}, {1,-1}, {1,1} };

        for (int d = 0; d < 4; d++)
        {
            for (int x = 1; x < compareRadius; x++)
            {
                int lx = selectX + x * dir[d,0];
                int ly = selectY + x * dir[d,1];

                if (lx < 0 || ly < 0 || lx >= gameData.columnSIze || ly >= gameData.rowSIze)
                {
                    continue;
                }

                var key = new Tuple<int, int>(lx, ly);
                // int cost = FindCostRecursively(lx, ly, scoreTotal);

                var tileCompare = TileManager.Instance.GetTile(lx, ly);

                if (tileCompare == null || tileCompare == startTile || !tileCompare.IsActive())
                    continue;

                scoreTotal += tileCompare.GetNumber();
                if ( !valuesTable.ContainsKey(key) )
                {
                    valuesTable[key] = scoreTotal;
                }

                if(scoreTotal > 10)
                {
                    scoreTotal = startTile.GetNumber();

                    foreach (var tile in tileObjects)
                    {
                        tile.ToggleSelectedOnly(false);
                    }
                    tileObjects.Clear();
                    tileObjects.Add(startTile);

                    break;
                }

                tileCompare.ToggleSelectedOnly(true);
                tileObjects.Add(tileCompare);

                if (scoreTotal == 10)
                {
                    return true;
                }
            }

            StartAgain(startTile, ref scoreTotal);
        }

        /*for (int d = 4; d < 8; d++)
        {
            for (int x = 0; x < compareRadius; x++)
            {
                for (int y = 0; y < compareRadius; y++)
                {
                    int lx = selectX + x * dir[d, 0];
                    int ly = selectY + y * dir[d, 1];

                    if (lx < 0 || ly < 0)
                    {
                        continue;
                    }

                    var key = new Tuple<int, int>(lx, ly);
                    if (valuesTable.ContainsKey(key))
                    {

                    }
                }
            }
        }*/

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
            MyLogger.Log($"last iteration:{iterationsForOneSearch}");
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

            foreach (var tile in tileObjects)
            {
                 tile.SetActive(false, true);
            }
        }
        else if (!isSelecting && delay > gameData.delayForAIScoreAnimation)
        {

            delay = 0;
            isSelecting = true;
            UpdateScore(tileObjects.Count);
            HideSelection();
        }

    }

    public void HideSelection()
    {
        foreach (var tileObject in tileObjects)
        {
            tileObject.SetActive(false);
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
