using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class HUDController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private HUDSO config;
    [SerializeField] private float levelTextTime;
    private List<Image> hearts = new List<Image>();
    private TextMeshProUGUI coinsText;
    
    private int index;

    private enum HeartTypes { Empty, Half, Full, }

    // const
    private const string HEALTH = "Health";
    private const string HEART = "Heart";
    private const string WEAPON_FRAME = "WeaponFrame";
    private const string LEVEL_NAME_TEXT = "LevelNameText";
    private const string COINS_TEXT = "Coins/CoinsText";

    ///////////////////////////////////////////
    // Methods

    private void Start()
    {
        InitializeLevelText();
        coinsText = transform.Find(COINS_TEXT).GetComponent<TextMeshProUGUI>();
        coinsText.text = GameState.Coins.ToString();
    }

    private void OnEnable()
    {
        Player.OnFullDamageTaken += OnFullDamageTaken;
        Player.OnHalfDamageTaken += OnHalfDamageTaken;
        Player.OnFullHeartPickedUp += OnFullHeartPickedUp;
        Player.OnHalfHeartPickedUp += OnHalfHeartPickedUp;
        Player.OnWeaponPickedUp += OnWeaponPickedUp;
        Player.OnHealthSet += PlaceHearts;

        HealthUpgradeController.OnHealthUpgraded += OnHealthUpgraded;
    }

    private void OnDestroy()
    {
        Player.OnFullDamageTaken -= OnFullDamageTaken;
        Player.OnHalfDamageTaken -= OnHalfDamageTaken;
        Player.OnFullHeartPickedUp -= OnFullHeartPickedUp;
        Player.OnHalfHeartPickedUp -= OnHalfHeartPickedUp;
        Player.OnWeaponPickedUp -= OnWeaponPickedUp;
        Player.OnHealthSet -= PlaceHearts;
        
        HealthUpgradeController.OnHealthUpgraded -= OnHealthUpgraded;
    }

    private void InitializeLevelText()
    {
        TextMeshProUGUI levelNameText = transform.Find(LEVEL_NAME_TEXT).GetComponent<TextMeshProUGUI>();
        string levelName = GameState.CurrentLevelName;
        if (!Utilities.IsFinalLevel())
        {
            int length = levelName.Length;
            levelNameText.text = levelName.Remove(length - 1).PadRight(length + 1) + levelName.Remove(0, length - 1);
        }
        else levelNameText.text = "The " + Definitions.FINAL + " " + Definitions.LEVEL;

        StartCoroutine(RemoveLevelText());
    }

    private void PlaceHearts()
    {
        Transform firstHeart = transform.Find(HEALTH + "/" + HEART);
        int currentHealth = Player.Health;
        int numberOfFullHearts = currentHealth / Definitions.ONE_HEALTH_POINT;
        hearts.Add(firstHeart.GetComponent<Image>());

        if (numberOfFullHearts > 0)
        {
            for (int i = 1; i < numberOfFullHearts; i++)
            {
                PlaceSpecificHeart(HeartTypes.Full, firstHeart);
            }

            if (currentHealth != Player.MaxHealth)
            {
                if (currentHealth % Definitions.ONE_HEALTH_POINT != 0)
                {
                    PlaceSpecificHeart(HeartTypes.Half, firstHeart);
                }
            }
            index = hearts.Count - 1;
        }
        else
        {
            firstHeart.GetComponent<Image>().sprite = config.HalfHeart;
            index = 0;
        }

        int numberOfEmptyHearts = (Player.MaxHealth - currentHealth) / Definitions.ONE_HEALTH_POINT;
        for (int i = 0; i < numberOfEmptyHearts; i++)
        {
            PlaceSpecificHeart(HeartTypes.Empty, firstHeart);
        }
    }

    private void PlaceSpecificHeart(HeartTypes heartType, Transform heartOrigin)
    {
        Image newHeart = Instantiate(heartOrigin.GetComponent<Image>(), transform.Find(HEALTH));
        switch (heartType)
        {
            case HeartTypes.Empty: newHeart.sprite = config.EmptyHeart;
                break;
            case HeartTypes.Half: newHeart.sprite = config.HalfHeart;
                break;
            case HeartTypes.Full: newHeart.sprite = config.FullHeart;
                break;
        }

        newHeart.name = HEART;
        newHeart.transform.localScale = heartOrigin.localScale;
        hearts.Add(newHeart);
    }

    private void OnFullDamageTaken()
    {
        if (Player.Health <= 0)
        {
            hearts[index].sprite = config.EmptyHeart;
        }
        else
        {
            bool isFullHeart = (Player.Health % Definitions.ONE_HEALTH_POINT == 0);
            if (isFullHeart)
            {
                hearts[index].sprite = config.EmptyHeart;
                index--;
            }
            else
            {
                hearts[index].sprite = config.EmptyHeart;
                index--;
                hearts[index].sprite = config.HalfHeart;
            }
        }
    }

    private void OnHalfDamageTaken()
    {
        if (Player.Health <= 0)
        {
            hearts[index].sprite = config.EmptyHeart;
        }
        else
        {
            bool isFullHeart = (Player.Health % Definitions.ONE_HEALTH_POINT == 0);
            if (isFullHeart)
            {
                hearts[index].sprite = config.EmptyHeart;
                index--;
            }
            else
            {
                hearts[index].sprite = config.HalfHeart;
            }
        }
    }

    private void OnFullHeartPickedUp()
    {
        bool isFullHeart = (Player.Health % Definitions.ONE_HEALTH_POINT == 0);
        if (isFullHeart)
        {
            if (index < hearts.Count - 1) index++;
            hearts[index].sprite = config.FullHeart;
        }
        else
        {
            hearts[index].sprite = config.FullHeart;
            index++;
            hearts[index].sprite = config.HalfHeart;
        }
    }

    private void OnHalfHeartPickedUp()
    {
        bool isFullHeart = (Player.Health % Definitions.ONE_HEALTH_POINT == 0);
        if (isFullHeart)
        {
            hearts[index].sprite = config.FullHeart;
        }
        else
        {
            if (index < hearts.Count - 1) index++;
            hearts[index].sprite = config.HalfHeart;
        }
    }

    private void OnWeaponPickedUp()
    {
        string weaponName = GameObject.FindObjectOfType<Player>().GetWeaponName();
        Image newWeapon = GetWeaponImage(weaponName);

        if (newWeapon != null)
        {
            Transform weaponFrame = transform.Find(WEAPON_FRAME);
            if (weaponFrame.childCount > 0) Destroy(weaponFrame.GetChild(0).gameObject);
            Image weapon = Instantiate(newWeapon, weaponFrame);
            weapon.name = newWeapon.name;
            weapon.transform.localScale = newWeapon.transform.localScale;
        }
    }

    private Image GetWeaponImage(string weaponName)
    {
        Image newImage = null;
        foreach (Image image in config.weaponImages)
        {
            if (image.name.Contains(weaponName))
            {
                newImage = image;
                break;
            }
        }

        return newImage;
    }

    private IEnumerator RemoveLevelText()
    {
        yield return new WaitForSeconds(levelTextTime);
        Destroy(transform.Find(LEVEL_NAME_TEXT).gameObject);
    }

    private void OnHealthUpgraded(int upgradeAmount)
    {
        int numberOfNewHearts = upgradeAmount / Definitions.ONE_HEALTH_POINT;
        Transform firstHeart = transform.Find(HEALTH + "/" + HEART);
        for (int i = 0; i < numberOfNewHearts; i++)
        {
            PlaceSpecificHeart(HeartTypes.Empty, firstHeart);
        }
    }

    public void UpdateCoinsText(int coins) => coinsText.text = coins.ToString();
}
