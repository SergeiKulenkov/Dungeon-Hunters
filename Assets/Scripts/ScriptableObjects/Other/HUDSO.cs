using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = Definitions.HUD_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.HUD_CONFIG)]
public class HUDSO : ScriptableObject
{
    public Sprite FullHeart;
    public Sprite HalfHeart;
    public Sprite EmptyHeart;

    public List<Image> weaponImages;
}
