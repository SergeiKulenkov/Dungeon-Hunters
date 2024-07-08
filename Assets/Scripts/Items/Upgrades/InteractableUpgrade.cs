using UnityEngine;
using System;

public abstract class InteractableUpgrade : InteractableItemController
{
    ///////////////////////////////////////////
    // Fields
    protected int price;

    public static event Action<int> OnUpgraded;

    // const
    private const string PRICE_TEXT = "Canvas/PriceText";
    
    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        transform.Find(PRICE_TEXT).GetComponent<TMPro.TextMeshProUGUI>().text = price.ToString();
        
        SetType(Definitions.InteractableItems.Buy);

        base.Start();
    }

    protected void SetImage(Sprite image) => transform.Find(Definitions.SPRITE).GetComponent<SpriteRenderer>().sprite = image;

    protected abstract void SendUpgradedEvent();

    protected override void OnInteractPressed()
    {
        if (isEntered && !isUsed &&
            (price <= GameState.Coins))
        {
            isUsed = true;
            SendUpgradedEvent();
            OnUpgraded?.Invoke(price);
            Destroy(gameObject);
        }
    }
}
