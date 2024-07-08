using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utilities
{
    ///////////////////////////////////////////
    // Fields

    ///////////////////////////////////////////
    // Methods

    public static void SetIgnoredLayerCollisions()
    {
        Physics2D.IgnoreLayerCollision(Definitions.LAYER_PROJECTILE, Definitions.LAYER_ROOM, true);
        Physics2D.IgnoreLayerCollision(Definitions.LAYER_PROJECTILE, Definitions.LAYER_TRAP, true);
        Physics2D.IgnoreLayerCollision(Definitions.LAYER_PROJECTILE, Definitions.LAYER_ITEM, true);
    }

    public static Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static bool ShouldDropFullHealth()
    {
        return Random.Range(0, 100) < Definitions.FULL_HEALTH_DROP_PROBABILITY;
    }

    public static bool ShouldDropHalfHealth()
    {
        return Random.Range(0, 100) < Definitions.HALF_HEALTH_DROP_PROBABILITY;
    }

    public static bool ShouldDropCoin()
    {
        return Random.Range(0, 100) < Definitions.COIN_DROP_PROBABILITY;
    }

    public static bool IsFirstLocation()
    {
        return GameState.CurrentScene.Contains(Definitions.FIRST_LOCATION);
    }

    public static bool IsSecondLocation()
    {
        return GameState.CurrentScene.Contains(Definitions.SECOND_LOCATION);
    }

    public static bool IsFirstLevel()
    {
        return GameState.CurrentLevelName.Contains(Definitions.LEVEL_1);
    }

    public static bool IsLastLevel()
    {
        return GameState.CurrentScene.Contains(Definitions.LAST_LEVEL) ||
                GameState.CurrentLevelName.Contains(Definitions.LAST_LEVEL_INDEX.ToString());
    }

    public static bool IsFinalLevel() => GameState.CurrentScene.Contains(Definitions.FINAL_LEVEL);

    // public static bool IsLevelSaved()
    // {
    //     return (!string.IsNullOrEmpty(GameState.PlayerSettings.LastSavedScene) && GameState.PlayerSettings.LastSavedScene.Contains(Definitions.LAST_LEVEL)) ||
    //             SaveLoad.IsLevelSaved();
    // }

    ///////////////////////////////////////////////////////////

    public static string RemoveCloneString(string text) => text.Remove(text.IndexOf("("), Definitions.CLONE_STRING_LENGTH);
    
    public static Definitions.Sides GetRandomSide()
    {
        return (Definitions.Sides) Random.Range((int) Definitions.Sides.Up, (int) Definitions.Sides.Limit);
    }

    public static Definitions.Sides RevertSide(Definitions.Sides side)
    {
        Definitions.Sides reverted = side;
        switch (side)
        {
            case Definitions.Sides.Up: reverted = Definitions.Sides.Down;
                break;
            case Definitions.Sides.Right: reverted = Definitions.Sides.Left;
                break;
            case Definitions.Sides.Down: reverted = Definitions.Sides.Up;
                break;
            case Definitions.Sides.Left: reverted = Definitions.Sides.Right;
                break;
        }
        return reverted;
    }

    public static Definitions.Sides GetNewSide(Definitions.Sides prevSide)
    {
        bool isOkay = false;
        Definitions.Sides newSide = GetRandomSide();
        while (!isOkay)
        {
            if (newSide != prevSide) isOkay = true;
            else newSide = GetRandomSide();
        }

        return newSide;
    }

    public static Definitions.Sides GetSideByName(string name)
    {
        Definitions.Sides side = Definitions.Sides.Up;

        if (name.Contains(Definitions.UP)) side = Definitions.Sides.Up;
        else if (name.Contains(Definitions.DOWN)) side = Definitions.Sides.Down;
        else if (name.Contains(Definitions.LEFT)) side = Definitions.Sides.Left;        
        else if (name.Contains(Definitions.RIGHT)) side = Definitions.Sides.Right;

        return side;
    }

    public static string GetNameOfSide(Definitions.Sides side)
    {
        string name = "";
        switch (side)
        {
            case Definitions.Sides.Up: name = Definitions.UP;
                break;
            case Definitions.Sides.Right: name = Definitions.RIGHT;
                break;
            case Definitions.Sides.Down: name = Definitions.DOWN;
                break;
            case Definitions.Sides.Left: name = Definitions.LEFT;
                break;
        }

        return name;
    }

    public static string GetSideNameFromObjectName(string name)
    {
        string sideName = "";
        if (name.Contains(Definitions.UP)) sideName = Definitions.UP;
        else if (name.Contains(Definitions.DOWN)) sideName = Definitions.DOWN;
        else if (name.Contains(Definitions.LEFT)) sideName = Definitions.LEFT;
        else if (name.Contains(Definitions.RIGHT)) sideName = Definitions.RIGHT;

        return sideName;
    }

    public static Definitions.Sides ConvertVectorToSide(Vector2 direction)
    {
        Definitions.Sides side = Definitions.Sides.Limit;
        if (direction == Vector2.left) side = Definitions.Sides.Left;
        else if (direction == Vector2.right) side = Definitions.Sides.Right;
        else if (direction == Vector2.down) side = Definitions.Sides.Down;
        else if (direction == Vector2.up) side = Definitions.Sides.Up;

        return side;
    }
    
    public static Vector2 ConvertSideToVector(Definitions.Sides side)
    {
        Vector2 offset = new Vector2();
        switch (side)
        {
            case Definitions.Sides.Up: offset = Vector2.up;
                break;
            case Definitions.Sides.Down: offset = Vector2.down;
                break;
            case Definitions.Sides.Right: offset = Vector2.right;
                break;
            case Definitions.Sides.Left: offset = Vector2.left;
                break;
        }

        return offset;
    }

    // public static List<Vector2> GetNewDirectionsList(Definitions.Sides prevSide)
    // {
    //     List<Vector2> directions = new List<Vector2>();
    //     if (prevSide != Definitions.Sides.Down) directions.Add(Vector2.down);
    //     if (prevSide != Definitions.Sides.Left) directions.Add(Vector2.left);
    //     if (prevSide != Definitions.Sides.Right) directions.Add(Vector2.right);
    //     if (prevSide != Definitions.Sides.Up) directions.Add(Vector2.up);

    //     return directions;
    // }

    public static Vector2 GetBlockingWallOffset(Definitions.Sides side)
    {
        Vector2 offset = new Vector2();
        switch (side)
        {
            case Definitions.Sides.Up: offset.y += 0.5f;
                break;
            case Definitions.Sides.Right:
                offset.x += 0.5f;
                offset.y -= 0.1f;
                break;
            case Definitions.Sides.Down: offset.y -= 0.5f;
                break;
            case Definitions.Sides.Left:
                offset.x -= 0.5f;
                offset.y -= 0.1f;
                break;
        }
        return offset;
    }

    public static int GetObjectTypeByProbability(List<int> probabilities)
    {
        int type = 0;
        int cumulative = 0;
        int random = Random.Range(0, 100);
        for (int i = 0; i < probabilities.Count; i++)
        {
            cumulative += probabilities[i];
            if (random < cumulative)
            {
                type = i;
                break;
            }
        }

        cumulative = 0;
        return type;
    }

    //////////////////////////////////////////////////////////////////////
    // Object Getters

    public static Transform GetWeaponByName(string name)
    {
        Transform weapon = null;
        WeaponsSO weaponsConfig = ConfigManager.GetWeaponsConfig();
        if (name.Contains(Definitions.PISTOL)) weapon = weaponsConfig.Pistol;
        else if (name.Contains(Definitions.CANON)) weapon = weaponsConfig.Canon;
        else if (name.Contains(Definitions.LASER_RIFLE)) weapon = weaponsConfig.LaserRifle;
        else if (name.Contains(Definitions.AUTO_RIFLE)) weapon = weaponsConfig.AutoRifle;

        return weapon;
    }

    //////////////////////////////////////////////////////////////////////
    // Coroutines

    public static IEnumerator EnableObjectWithFade(Transform objectToFade, bool enableColliders)
    {
        objectToFade.gameObject.SetActive(true);
        if (enableColliders)
        {
            foreach (Collider2D collider in objectToFade.GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = true;
            }
        }

        WaitForSeconds fadeDelay = new WaitForSeconds(Definitions.SPRITE_FADE_DELAY);
        SpriteRenderer sprite = objectToFade.GetComponent<SpriteRenderer>();
        float alpha = sprite.color.a;
        Color tmpColor = sprite.color;

        while (sprite.color.a < 1)
        {
            alpha += Definitions.SPRITE_FADE_AMOUNT;
            tmpColor.a = alpha;
            sprite.color = tmpColor;
            yield return fadeDelay;
        }
    }

    public static IEnumerator DisableObjectWithFade(Transform objectToFade, bool disableColliders)
    {
        if (disableColliders)
        {
            foreach (Collider2D collider in objectToFade.GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
        }

        WaitForSeconds fadeDelay = new WaitForSeconds(Definitions.SPRITE_FADE_DELAY);
        SpriteRenderer sprite = objectToFade.GetComponent<SpriteRenderer>();
        float alpha = sprite.color.a;
        Color tmpColor = sprite.color;

        while (sprite.color.a > 0)
        {
            alpha -= Definitions.SPRITE_FADE_AMOUNT;
            tmpColor.a = alpha;
            sprite.color = tmpColor;
            yield return fadeDelay;
        }

        objectToFade.gameObject.SetActive(false);
    }
}
