using UnityEngine;
using System.Collections.Generic;

public class SpawnersSO : ScriptableObject
{
    public int RoomCounnt;
    [Header("Types: Size6, Size5, Size8, LongHor, LongVert, Last")]
    public int NumberOfRoomTypes;
    [Header("EnemyTypes: Dummy, Statue, Skeleton, Rogue")]
    public int NumberOfEnemyTypes;
    public List<int> EnemyTypeProbability;

    [Header("Number of objects per room")]
    [Header("Enemies")]
    public int EnemyCount;
    public int EnemiesChangeMin;
    [Header("MaxExclusive")]
    public int EnemiesChangeMax;

    [Header("Traps")]
    public int TrapCount;
    public int TrapsChangeMin;
    [Header("MaxExclusive")]
    public int TrapsChangeMax;

    [Header("Covers")]
    public int CoverCount;
    public int CoversChangeMin;
    [Header("MaxExclusive")]
    public int CoversChangeMax;
}
