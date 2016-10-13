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

    // Bazowy koszt budynku
    public int iBaseCost;
    public BigInteger BaseCost;

    public int BuildingUpgardeExperience;
    public int BuildingBuyExperience;

    // Mnożnik kosztów budynku (1.07 - 1.15)
    public float CostMultiplier;

    // Lista ulepszeń budynku (aktywne i nieaktywne). Na jej podstawie zliczane są bonusy
    public Upgrade[] UpgradePrefabs; // -- prefaby

    float UpgradesBaseShitValueMultiplier = 1;



    [HideInInspector]
    public Upgrade[] Upgrades; // -- właściwa zmienna z upgradeami

    // Właściwość określa, czy budynek został postawiony za pomocą edytora.
    // Jeżeli tak to przy burzeniu należy zwrócić część kwoty.
    public bool IsPlacedForReal = false;

    // Prefaby budynku
    public GameObject ButtonPrefab;




    #region Generowanie
    // BAD
    public float _shitValue;              // Wartość jednego G
    public int _maximumShits;               // Maksymalna ilosć G jaką może pomieścić budynek
    public int _humansInBuilding;            // Ilosć ludzi w budynku
    public int _currentShits;                // Aktualna ilość G
    [Range(1.07f, 1.15f)]
    public float _shitValueMultiplier;   // Mnożnik wartości G zakres 

    public float ShitValue { get { return _shitValue; } set { _shitValue = value; } }   
    public int MaximumShits { get { return _maximumShits; } set { _maximumShits = value; } }
    public int HumansInBuilding { get { return _humansInBuilding; } set { _humansInBuilding = value; } }
    public int CurrentShits { get { return _currentShits; } set { _currentShits = value; } }

    /// <summary>
    /// Metoda wywoływana co sekundę
    /// </summary>
    void GenerateShit()
    {
        if (BuildingType == BuildingType.Sludgeworks)
        {
            // Jeżeli jest to oczyszczalnia to nie generuje zysków
        }
        else
        {
            // Jeżeli wartość nie przekroczyła maksymalnej dopuszczalnej to zwiększa licznik
            if (CurrentShits < MaximumShits)
            {
                CurrentShits += HumansInBuilding;

                // Aktualizacja wskaźnika zapełnienia - jeżeli jest on wyświetlony dla tego budynku
                if (Helper.GetGUIManager().BuildingMode_BuildingProgressBar.activeInHierarchy)
                {
                    BuildingProgressBar progressBarScript = Helper.GetGUIManager().BuildingMode_BuildingProgressBar.GetComponent<BuildingProgressBar>();
                    if (progressBarScript != null)
                    {
                        if (progressBarScript.GetCurrentlyFollowingBuilding() == gameObject)
                        {
                            progressBarScript.SetValue(this);
                        }
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
        BigInteger toReturn = CurrentValue();
        _currentShits = 0;

        // Aktualizacja wskaźnika zapełnienia - jeżeli jest on wyświetlony dla tego budynku
        if (Helper.GetGUIManager().BuildingMode_BuildingProgressBar.activeInHierarchy)
        {
            BuildingProgressBar progressBarScript = Helper.GetGUIManager().BuildingMode_BuildingProgressBar.GetComponent<BuildingProgressBar>();
            if (progressBarScript != null)
            {
                if (progressBarScript.GetCurrentlyFollowingBuilding() == gameObject)
                {
                    progressBarScript.SetValue(this);
                }
            }
        }

        return toReturn;
    }

    /// <summary>
    /// Metoda zwraca aktualną wartość jaką przechowuje budynek na podstawie poziomu budynku
    /// i wartości pojedyńczego shitu
    /// </summary>
    /// <returns></returns>
    public BigInteger CurrentValue()
    {
        float value = _currentShits * GetShitValue();
        return new BigInteger(Math.Round(value, 0).ToString());
    }
    #endregion

    void Start()
    {
        // Uruchomienie mechanizmu generującego G
        InvokeRepeating("GenerateShit", 0f, 1.0f);
    }
    
    void Awake()
    {
        List<Upgrade> upgradesList = new List<Upgrade>();
        foreach (Upgrade prefab in UpgradePrefabs)
        {
            upgradesList.Add(prefab.GetCopy());
        }
        Upgrades = upgradesList.ToArray();
    }

    /// <summary>
    /// Metoda inicjalizująca bazowy koszt (potrzebna żeby przenieść wartości z inspektora do BigInteger)
    /// </summary>
    public void InitializeBase()
    {
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

    /// <summary>
    /// Metoda zwraca koszt budynku na danym poziomie
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private BigInteger SimulateCost(int level)
    {
        return BaseCost * ((float)Math.Pow(CostMultiplier, level));
    }

    /// <summary>
    /// Metoda zwraca wartość G
    /// </summary>
    /// <returns></returns>
    public float GetShitValue()
    {
        // wartość shitu = (base_value * multiplier) * (level ^ shitvaluemultiplier)
        return (float)(BuildingLevel * (Math.Pow((float)5, _shitValueMultiplier) * UpgradesBaseShitValueMultiplier));
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

    /// <summary>
    /// Metoda oblicza koszt ulepszenia budynku o levels poziomów
    /// </summary>
    /// <param name="levels"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Metoda uaktywnia ulepszenie
    /// </summary>
    /// <param name="upgrade"></param>
    /// <returns></returns>
    public bool ActivateUpgrade(Upgrade upgrade)
    {
        bool result = false;

        if (Upgrades.Contains(upgrade))
        {
            Upgrade buildingUpgrade = Upgrades.FirstOrDefault(x => x.Name == upgrade.Name);
            
            // TODO : sprawdzać typ ulepszenia
            UpgradesBaseShitValueMultiplier += buildingUpgrade.Value;
            result = true;
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
                    if (upgrade.HasBeenBought)
                        ActivateUpgrade(upgrade);
                }
            }
        }
    }
}
