using System.Collections.Generic;
using UnityEngine;

public class FinalLevelMapManager : LastLevelMapManager
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private List<Transform> enemies;
    
    // const 
    private const string FINAL_LEVEL_ROOM = "FinalLevelRoom";

    ///////////////////////////////////////////
    // Methods
    
    protected override void Start()
    {
        enemyObjects = enemies;
        room = transform.Find(FINAL_LEVEL_ROOM);
        base.Start();
    }
}
