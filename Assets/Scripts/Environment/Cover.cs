using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cover : MonoBehaviour, ICover
{
    ///////////////////////////////////////////
    // Fields
    private List<Transform> variants = new List<Transform>();
    private int index;

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            variants.Add(child);
        }   
    }

    public void ChangeVariant(int damage)
    {
        if (index < variants.Count - 1)
        {
            variants[index].gameObject.SetActive(false);
            if (damage > Definitions.HALF_HEALTH_POINT * 2) index += 3;
            else if (damage > Definitions.HALF_HEALTH_POINT) index += 2;
            else index++;

            if (index >= variants.Count) index = variants.Count - 1;
            variants[index].gameObject.SetActive(true);
        }
        else
        {
            StartCoroutine(Utilities.DisableObjectWithFade(variants[index], false));
            transform.GetComponent<Collider2D>().enabled = false;

            Transform coin = transform.Find(Definitions.COIN_PICK_UP);
            if (coin != null)
            {
                coin.gameObject.SetActive(true);
                coin.parent = null;
            }

            StartCoroutine(RemoveComponents(coin));
        }
    }

    private IEnumerator RemoveComponents(Transform coin)
    {
        yield return new WaitForSeconds(Definitions.SPRITE_FADE_TIME);

        foreach (Transform child in transform) Destroy(child.gameObject);
        foreach (Component component in transform.GetComponents<Component>())
        {
            if (!(component is Transform)) Destroy(component);
        }
        
        if (coin != null)
        {
            GameObject coinCopy = new GameObject();
            coinCopy.name = coin.name;
            coinCopy.transform.SetParent(transform);
            coinCopy.SetActive(false);
        }
    }
}
