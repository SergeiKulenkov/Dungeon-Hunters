using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = Definitions.FINAL_LEVEL_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.FINAL_LOCATION_PATH + Definitions.FINAL_LEVEL_CONFIG)]
public class FinalLevelSpawnerSO : LastLevelSpawnerSO
{
    public List<Transform> Covers;
    public List<Transform> Traps;
}
