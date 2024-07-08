using UnityEngine;
using System;
using System.Collections;

public class ChestController : InteractableItemController
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private float finishDelay;
    // events
    public event Action OnChestOpened;

    // const
    private const string OPEN_TRIGGER = "Open";

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        SetType(Definitions.InteractableItems.Open);
        base.Start();
    }

    protected override void OnInteractPressed()
    {
        if (isEntered && !isUsed)
        {
            isUsed = true;
            GetComponent<Animator>().SetTrigger(OPEN_TRIGGER);
            StartCoroutine(DelayFinish());
        }
    }

    private IEnumerator DelayFinish()
    {
        yield return new WaitForSeconds(finishDelay);
        OnChestOpened?.Invoke();
    }
}
