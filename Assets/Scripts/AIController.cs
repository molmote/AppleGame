using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private bool isSelecting = false;
    protected List<TileObject> tileObjects = new List<TileObject>();

    [SerializeField] protected GameData gameData; // uses scriptable object for customized settings

    [SerializeField] protected TextMeshProUGUI uiTextTime;
    [SerializeField] protected TextMeshProUGUI uiTextScore;

    protected Dictionary<Tuple<int, int>, int> valuesTable = new Dictionary<Tuple<int, int>, int>();

    protected int score = 0;

    [SerializeField] protected int targetStartX;
    [SerializeField] protected int targetStartY;
    [SerializeField] protected int targetEndX;
    [SerializeField] protected int targetEndY;

    private void UpdateScore(int _score)
    {
        score += _score;

        uiTextScore.text = $"score: {score}";
    }

    protected void ResetVisited()
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

    protected void SelectTilesBlindly(out int selectX, out int selectY)
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

    protected int FindCostRecursively (int x, int y, int sx, int sy, int baseCost, bool findAllRoutes)
    {
        iterationsForOneSearch++;
        var myTile = TileManager.Instance.GetTile(x, y);
        if (myTile == null)
        {
            return 0;
        }

        var key = new Tuple<int, int>(x, y);
        if (valuesTable.ContainsKey(key))
        {
            return valuesTable[key];
        }

        if (x == sx && y == sy)
        {
            // MyLogger.Log("returns 0");
            // valuesTable[key] = 0;
            return 0;
        }

        int modx = x - 1;
        int mody = y - 1;
        if (x <= sx)
        {
            modx = x + 1;
        }
        if (y <= sy)
        {
            mody = y + 1;
        }

        int myCost = myTile.GetNumber();

        if (!myTile.IsActive())
            myCost = 0;


        if (!findAllRoutes)
        {
            int firstSum = -1;
            int secondSum = -1;
            int thirdSum = -1;
            if (baseCost + myCost > 10)
            {
                valuesTable[key] = baseCost + myCost;
                myTile.SetTestText($"{myCost}");

                return baseCost + myCost;
            }
            /*else if (baseCost + myCost == 10)
            {
                myTile.ToggleSelectedOnly(true);
                tileObjects.Add(myTile);

                return baseCost + myCost;
            }*/

            if (x != sx)
            {
                firstSum = FindCostRecursively(modx, y, sx, sy, baseCost, findAllRoutes);
                myCost += firstSum;

                if (baseCost + myCost > 10)
                {
                    valuesTable[key] = baseCost + myCost;
                    myTile.SetTestText($"{myCost}");

                    //myTile.ToggleSelectedOnly(true);
                    //tileObjects.Add(myTile);
                    return baseCost + myCost;
                }
            }

            if (y != sy)
            {
                secondSum = FindCostRecursively(x, mody, sx, sy, baseCost, findAllRoutes);
                myCost += secondSum;

                if (baseCost + myCost > 10)
                {
                    valuesTable[key] = baseCost + myCost;
                    myTile.SetTestText($"{myCost}");

                    //myTile.ToggleSelectedOnly(true);
                    //tileObjects.Add(myTile);
                    return baseCost + myCost;
                }
            }


            if (x != sx && y != sy)
            {
                thirdSum = FindCostRecursively(modx, mody, sx, sy, baseCost, findAllRoutes);
                myCost -= thirdSum;
            }

            if (baseCost + myCost > 10)
            {
                valuesTable[key] = baseCost + myCost;
                myTile.SetTestText($"{myCost}");

                return baseCost + myCost;
            }
            
            if (baseCost + myCost == 10)
            {
                int j = 0;
            }

            myTile.ToggleSelectedOnly(true);
            tileObjects.Add(myTile);

            /*if (myCost >= 10)
            {
                valuesTable[key] = myCost;
                myTile.SetTestText($"{myCost}");
                return myCost;
            }*/
            valuesTable[key] = myCost;
            myTile.SetTestText($"{myCost}");
            return baseCost + myCost;
        }
        else
        {
            if (x != sx)
                myCost += FindCostRecursively(modx, y, sx, sy, baseCost, findAllRoutes);

            if (y != sy)
                myCost += FindCostRecursively(x, mody, sx, sy, baseCost, findAllRoutes);

            if (x != sx && y != sy)
                myCost -= FindCostRecursively(modx, mody, sx, sy, baseCost, findAllRoutes);
        }

        valuesTable[key] = myCost;
        myTile.SetTestText($"{myCost}");

        return myCost;
    }

    protected void StartAgain(TileObject start, ref int scoreTotal)
    {
        scoreTotal = start.GetNumber();
        start.ToggleSelectedOnly(true);

        foreach (var tile in tileObjects)
        {
            tile.ToggleSelectedOnly(false);
        }

        tileObjects.Clear();
        tileObjects.Add(start);
    }

    protected bool CheckTilesBlindly(int selectX, int selectY)
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

        StartAgain(startTile, ref scoreTotal);
        for (int d = 4; d < 8; d++)
        {
            for (int x = 0; x < compareRadius; x++)
            {
                for (int y = 0; y < compareRadius; y++)
                {
                    int lx = selectX + x * dir[d, 0];
                    int ly = selectY + y * dir[d, 1];

                    var tileCompare = TileManager.Instance.GetTile(lx, ly);
                    if (tileCompare == null || tileCompare == startTile || !tileCompare.IsActive())
                        continue;

                    if (selectX != 0 && selectX == targetStartX && selectY == targetStartY &&
                        lx == targetEndX && ly == targetEndY)
                    {
                        int j = 0;
                    }

                    if (lx < 0 || ly < 0 || lx >= gameData.columnSIze || ly >= gameData.rowSIze)
                    {
                        continue;
                    }

                    int cost = startTile.GetNumber();
                    var key = new Tuple<int, int>(lx, ly);
                    if (valuesTable.ContainsKey(key))
                    {
                        //cost += valuesTable[key];

                        //myTile.ToggleSelectedOnly(true);
                        //tileObjects.Add(myTile);

                        // continue;
                    }
                    else
                    {
                        cost = FindCostRecursively(lx, ly, selectX, selectY, cost, false);
                    }

                    if (cost == 10)
                    {
                        MyLogger.Log($"Found the solution at ({selectX},{selectY}) -> ({lx},{ly}) Following nodes were selected ...");
                        foreach(var nodes in tileObjects)
                        {
                            MyLogger.Log($"({nodes.GetColumn()},{nodes.GetRow()})");
                        }
                        Debug.Break();
                        return true;
                    }

                    StartAgain(startTile, ref scoreTotal);
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
            MyLogger.Log($"last iteration:{iterationsForOneSearch}");
            if (nextSolution)
            {
                Debug.LogError("Can't find solution");
            }
            else
            {
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
