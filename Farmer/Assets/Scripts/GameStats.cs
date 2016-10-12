using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class GameStats
{
    private BigInteger _currentMoney = new BigInteger("100000");

    private int _level = 1;
    private int _requiredExperience = 100;
    private int _currentExperience = 0;

    public int Level
    {
        get { return _level; }
        set { _level = value; Helper.GetGUIManager().GameStats_SetLevelText(value); }
    }
    public int RequiredExperience
    {
        get { return _requiredExperience; }
        set
        {
            _requiredExperience = value; Helper.GetGUIManager().GameStats_SetExperienceValue(_currentExperience, value);
            Helper.GetGUIManager().GameStats_SetExperienceBarValue((float)_currentExperience / (float)_requiredExperience);
        }
    }
    public int CurrentExperience
    {
        get { return _currentExperience; }
        set
        {
            _currentExperience = value; Helper.GetGUIManager().GameStats_SetExperienceValue(value, _requiredExperience);
            Helper.GetGUIManager().GameStats_SetExperienceBarValue((float)_currentExperience / (float)_requiredExperience);
        }
    }
    public BigInteger CurrentMoney
    {
        get { return _currentMoney; }
        set
        {
            _currentMoney = value;

            // Listenery
            Helper.GetGUIManager().GameStats_SetCurrentMoneyInfo(value);
            Helper.GetGUIManager().BuildingMode_UpdateBuildingLevelCostInfo(Helper.GetGameManager().GetCurrentlySelectedBuilding(), Helper.GetGameManager().GetUpgradeByValue());
        }
    }
    
    //public void LoadBuildingToWorld()
    //{
    //    if (Buildings != null)
    //    {
    //        foreach (SerializableBuilding serBuilding in Buildings)
    //        {
    //            GameObject[] buildings = BuildingsDatabase.GetBuildingsByType(serBuilding.BuildingType);
    //            foreach(GameObject building in buildings)
    //            {
    //                Building script = building.GetComponent<Building>();
    //                if(script != null)
    //                {
    //                    if (script.Name == serBuilding.BuildingName)
    //                    {
    //                        // Znaleziono budynek
    //                        GameObject objInst = GameObject.Instantiate(building);
    //                        objInst.transform.SetParent(Helper.GetBuildingsGroup().transform, true);

    //                        objInst.transform.position = new Vector3(serBuilding.PositionX, serBuilding.PositionY, serBuilding.PositionZ);
    //                        objInst.transform.localRotation = new Quaternion(serBuilding.RotationX, serBuilding.RotationY, serBuilding.RotationZ, serBuilding.RotationW);

    //                        Building objectsScript = objInst.GetComponent<Building>();
    //                        if (objectsScript != null)
    //                        {
    //                            objectsScript.BuildingLevel = serBuilding.BuildingLevel;
    //                            objectsScript.IsPlacedForReal = serBuilding.IsPlacedForReal;

    //                            if (objectsScript.Upgrades != null)
    //                            {
    //                                foreach (Upgrade up in objectsScript.Upgrades)
    //                                {
    //                                    SerializableUpgrade serializableUp = serBuilding.Upgrades.FirstOrDefault(x => x.Name == up.Name);
    //                                    if (serializableUp != null)
    //                                    {
    //                                        up.HasBeenBought = serializableUp.HasBeenBought;
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //        }
    //    }
    //}

    static GameStats _instance;
    public static GameStats Instance
    {
        get
        {
            if (_instance == null) _instance = new GameStats();
            return _instance;
        }
        set
        {
            _instance = value;
            Helper.GetGUIManager().GameStats_SetLevelText(value.Level);
            Helper.GetGUIManager().GameStats_SetCurrentMoneyInfo(value.CurrentMoney);
            Helper.GetGUIManager().GameStats_SetExperienceValue(value.CurrentExperience, value.RequiredExperience);
            Helper.GetGUIManager().GameStats_SetExperienceBarValue((float)value.CurrentExperience / (float)value.RequiredExperience);
            Helper.GetGUIManager().GameStats_SetIncomeInfo(value.GetCurrentIncome());
        }
    }

    #region ** Cash
    /// <summary>
    /// Metoda zwiększa ilość gotówki (np. w przypadku sprzedania budynku)
    /// </summary>
    /// <param name="money"></param>
    public void AddMoney(BigInteger money)
    {
        CurrentMoney += money;
    }

    /// <summary>
    /// Metoda zlicza przychody z budynków i dodaje je do aktualnej gotówki
    /// </summary>
    public void CollectShit()
    {
        // Zliczanie gotówki jeżeli gra nie jest w trybie edycji budowli
        if (!Helper.GetGameManager().IsEditMode())
        {
            CurrentMoney += GetCurrentIncome();
        }
    }

    public BigInteger GetCurrentIncome()
    {
        BigInteger income = new BigInteger("0");

        GameObject buildingGroup = Helper.GetBuildingsGroup();
        if (buildingGroup != null)
        {
            Building[] buildings = buildingGroup.GetComponentsInChildren<Building>();

            foreach (Building building in buildings)
            {
                income += building.TakeShit(); //building.GetIncome();
            }
        }

        return income;
    }

    /// <summary>
    /// Metoda zwraca aktualną ilość gotówki.
    /// </summary>
    /// <returns></returns>
    public BigInteger GetCurrentMoney()
    {
        return CurrentMoney;
    }

    /// <summary>
    /// Metoda zmniejsza ilosć gotówki
    /// </summary>
    /// <returns></returns>
    public void SpendMoney(BigInteger money)
    {
        CurrentMoney -= money;
    }
    #endregion

    #region ** Level
    public void AddExperience(int experience)
    {
        while (experience > 0)
        {
            if (CurrentExperience + experience >= RequiredExperience)
            {
                Level++;
                RequiredExperience *= 5;
                CurrentExperience = 0;

                experience -= RequiredExperience - CurrentExperience;
            }
            else
            {
                CurrentExperience += experience;
                experience = 0;
            }
        }
    }

    public void SetStats(GameStats stats)
    {
        _currentExperience = stats.CurrentExperience;
        _currentMoney = stats.CurrentMoney;
        _level = stats.Level;
        _requiredExperience = stats.RequiredExperience;
    }
    #endregion
}
