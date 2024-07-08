using System.Collections.Generic;
using UnityEngine;

public class LastLevelSpawnerSO : ScriptableObject
{
    [Header("Enemies")]
    public int EnemyCount;
    public float EnemySpawnDelay;
    public float EnemyCountCheckDelay;
    public List<int> EnemyTypeProbability;

    [Header("Traps, count is halved here")]
    public int TrapCount;
    public int TrapsChangeMin;
    [Header("MaxExclusive")]
    public int TrapsChangeMax;

    [Header("Covers, count is halved here")]
    public int CoverCount;
    public int CoversChangeMin;
    [Header("MaxExclusive")]
    public int CoversChangeMax;

    [Header("Spawn Area")]
    public float NoOverlapRadius;
    public Vector2 NewPositionOffset;
    
    public int MaxNumberOfTriesToMoveObject;
    [Header("Percent values from number of tries")]
    public float WhenMoveFromSideToSideFirstTime;
    public float WhenMoveFromSideToSideSecondTime;
}
