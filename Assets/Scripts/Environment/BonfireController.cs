using System;

public class BonfireController : InteractableItemController
{
    ///////////////////////////////////////////
    // Fields
    // events
    public static event Action<bool> OnSaved;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        SetType(Definitions.InteractableItems.Save);
        base.Start();
    }

    protected override void OnInteractPressed()
    {
        if (isEntered && !isUsed)
        {
            isUsed = true;
            bool isSaved = (Utilities.IsLastLevel() || Utilities.IsFinalLevel()) ? true : SaveLoad.SaveRooms();
            if (isSaved) SoundManager.Instance.PlaySound(Definitions.Sounds.Item);
            OnSaved?.Invoke(isSaved);
        }
    }
}
