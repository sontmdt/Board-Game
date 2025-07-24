using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject
{
    public int BoardSizeX = 5;
    
    public int BoardSizeY = 5;

    public int MatchesMin = 3;

    public int PointForOneStar = 10;

    public int PointForTwoStar = 40;

    public int PointForThreeStar = 100;

    public int PointPerCandy = 2;

    public int LevelTurns = 16;

    public float LevelTimer = 30f;

    public float TimeForHint = 5f;
}
