using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public int timer;
    public int columnSIze;
    public int rowSIze;

    // delay1 >> select tiles >> animation for delay2 >> delay1 >> select...
    public float delayForAISelect;          // time takes from score to select next tiles
    public float delayForAIScoreAnimation;  // time between select and before select

    public void Reset()
    {

    }
}
