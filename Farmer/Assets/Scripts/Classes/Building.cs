using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Building : MonoBehaviour
{

    // Nazwa i typ
    public string Name;
    public BuildingType BuildingType;
    public string Description;
    public Sprite BuildingImage;

    // Poziom ulepszenia budynku
    public int BuildingLevel = 1;

    // Bazowy przychód i koszt budynku
    public int iBaseIncome;
    public BigInteger BaseIncome;

    public int iBaseCost;
    public BigInteger BaseCost;

    public int BuildingUpgardeExperience;
    public int BuildingBuyExperience;

    // Mnożnik kosztów budynku (1.07 - 1.15)
    public float CostMultiplier;

    #region Generowanie
    private float _shitValue = 1.0f;        // Wartość jednego G
    private int _maximumShits = 10;         // Maksymalna ilosć G jaką może pomieścić budynek
    private int _humansInBuilding = 1;      // Ilosć ludzi w budynku
    private int _currentShits = 0;          // Aktualna ilość G

    public float ShitValue { get { return _shitValue; } set { _shitValue = value; } }   
    public int MaximumShits { get { return _maximumShits; } set { _maximumShits = value; } }
    public int HumansInBuilding { get { return _humansInBuilding; } set { _humansInBuilding = value; } }
    public int CurrentShits { get { return _currentShits; } set { _currentShits = value; } }

    /// <summary>
    /// Metoda wywoływana co sekundę
    /// </summary>
    void GenerateShit()
    {
        // Jeżeli wartość nie przekroczyła maksymalnej dopuszczalnej to zwiększa licznik
        if(CurrentShits < MaximumShits)
        {
            CurrentShits += HumansInBuilding;

            // Aktualizacja wskaźnika zapełnienia - jeżeli jest on wyświetlony dla tego budynku
             if(Helper.GetGUIManager().BuildingMode_BuildingProgressBar.activeInHierarchy)
             {
                BuildingProgressBar progressBarScript = Helper.GetGUIManager().BuildingMode_BuildingProgressBar.GetComponent<BuildingProgressBar>();
                if (progressBarScript != null)
                {
                    if(progressBarScript.GetCurrentlyFollowingBuilding() == gameObject)
                    {
                        float percentage = (float)_currentShits / (float)_maximumShits;
                        progressBarScript.SetValue(percentage);
                    }
                }
             }
        }
    }

    /// <summary>
    /// Metoda powoduje 'zabranie' przez skrypt oczyszczalni G z danego budynku
    /// </summary>
    /// <returns></returns>
    public BigInteger TakeShit()
    {
        BigInteger toReturn = new BigInteger(_currentShits);
        _currentShits = 0;
        return toReturn;
    }
    #endregion

    void Start()
    {
        // Uruchomienie mechanizmu generującego G
        InvokeRepeating("GenerateShit", 0f, 1.0f);
    }

    // Lista ulepszeń budynku (aktywne i nieaktywne). Na jej podstawie zliczane są bonusy
    public Upgrade[] UpgradePrefabs; // -- prefaby
    float UpgradesBaseIncomeMultiplier = 1;

    [HideInInspector]
    public Upgrade[] Upgrades; // -- właściwa zmienna z upgradeami

    // Właściwość określa, czy budynek został postawiony za pomocą edytora.
    // Jeżeli tak to przy burzeniu należy zwrócić część kwoty.
    public bool IsPlacedForReal = false;

    // Prefaby budynku
    public GameObject ButtonPrefab;

    void Awake()
    {
        List<Upgrade> upgradesList = new List<Upgrade>();
        foreach (Upgrade prefab in UpgradePrefabs)
        {
            upgradesList.Add(prefab.GetCopy());
        }
        Upgrades = upgradesList.ToArray();
    }

    public void InitializeBase()
    {
        BaseIncome = new BigInteger(iBaseIncome);
        BaseCost = new BigInteger(iBaseCost);
    }

    /// <summary>
    /// Metoda oblicza koszt budowli na podstawie poziomu ulepszenia
    /// </summary>
    /// <returns></returns>
    public BigInteger GetCost()
    {
        // Wzór BaseCost * CostMultiplier ^ (BuildingLevel)
        if (BaseCost == null || BaseCost == new BigInteger("0")) InitializeBase();

        return SimulateCost(BuildingLevel);
    }

    private BigInteger SimulateCost(int level)
    {
        return BaseCost * ((float)Math.Pow(CostMultiplier, level));
    }

    /// <summary>
    /// Metoda oblicza generowany przychód na podstawie poziomu ulepszenia
    /// </summary>
    /// <returns></returns>
    public BigInteger GetIncome()
    {
        if (BaseIncome == null || BaseIncome == new BigInteger("0")) InitializeBase();

        // Wzór BaseIncome  * BuildingLevel
        return (BaseIncome * UpgradesBaseIncomeMultiplier) * new BigInteger(BuildingLevel);
    }

    /// <summary>
    /// Metoda oblicza kwotę, za którą można sprzedać budynek
    /// </summary>
    /// <returns></returns>
    public BigInteger GetSellPrice()
    {
        return GetCost() / 3;
    }

    /// <summary>
    /// Metoda ulepsza budynek
    /// </summary>
    public void Upgrade(int levels)
    {
        BuildingLevel += levels;
    }

    /// <summary>
    /// Pobiera kopię prefabu (unlinked) - nie wiadomo czy będzie potrzebne?
    /// </summary>
    /// <returns></returns>
    public Building GetCopy()
    {
        return this.MemberwiseClone() as Building;
    }

    public BigInteger CalculateCostForNextXLevels(int levels)
    {
        BigInteger result = new BigInteger("0");

        for (int level = BuildingLevel; level < BuildingLevel + levels; level++)
        {
            BigInteger simulatedCost = SimulateCost(level);
            result += simulatedCost;
        }

        return result;
    }

    public bool ActivateUpgrade(Upgrade upgrade)
    {
        bool result = false;

        if (Upgrades.Contains(upgrade))
        {
            Upgrade buildingUpgrade = Upgrades.FirstOrDefault(x => x.Name == upgrade.Name);
            if (!buildingUpgrade.HasBeenBought)
            {
                buildingUpgrade.HasBeenBought = true;

                // TODO : sprawdzać typ ulepszenia
                UpgradesBaseIncomeMultiplier += buildingUpgrade.Value;

                result = true;
            }
        }

        return result;
    }

    // System save-load
    public void LoadBuilding(SerializableBuilding serializableBuilding)
    {
        // Ustawianie wartości pozycji i rotacji
        transform.position = new Vector3(serializableBuilding.PositionX, serializableBuilding.PositionY, serializableBuilding.PositionZ);
        transform.localRotation = new Quaternion(serializableBuilding.RotationX, serializableBuilding.RotationY, serializableBuilding.RotationZ, serializableBuilding.RotationW);

        // Ustawianie wartości budynku
        BuildingLevel = serializableBuilding.BuildingLevel;
        IsPlacedForReal = serializableBuilding.IsPlacedForReal;

        if (Upgrades != null)
        {
            foreach (Upgrade upgrade in Upgrades)
            {
                SerializableUpgrade serializableUp = serializableBuilding.Upgrades.FirstOrDefault(x => x.Name == upgrade.Name);
                if (serializableUp != null)
                {
                    upgrade.HasBeenBought = serializableUp.HasBeenBought;
                    ActivateUpgrade(upgrade);
                }
            }
        }
    }
}
