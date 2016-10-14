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

    public bool ShitCollected = true;
    /// <summary>
    /// Metoda zlicza przychody z budynków i dodaje je do aktualnej gotówki
    /// </summary>
    public void CollectShit()
    {
        // Zliczanie gotówki jeżeli gra nie jest w trybie edycji budowli
        if (!Helper.GetGameManager().IsEditMode())
        {
            GameObject buildingGroup = Helper.GetBuildingsGroup();
            if (buildingGroup != null)
            {
                Building[] buildings = buildingGroup.GetComponentsInChildren<Building>();

                float howManyCanTake = HowManyShitCanTake();

                foreach (Building building in buildings)
                {
                    if (building.BuildingType != BuildingType.Sludgeworks)
                    {
                        if (new BigInteger(howManyCanTake.ToString("0")) < building.CurrentShits)
                        {
                            // Jeżeli zabrać może mniej niż ma budynek
                            // to zabiera tyle co może, a budynkowi odbiera tylko tyle co zabralo
                            CurrentMoney += building.TakeShit(howManyCanTake);
                            break;
                        }
                        else
                        {
                            // Jeżeli może zabrać to zabiera 
                            CurrentMoney += building.TakeShit();
                        }
                    }
                }
            }
        }

        ShitCollected = true;
    }

    float HowManyShitCanTake()
    {
        float result = 0f;

        GameObject buildingGroup = Helper.GetBuildingsGroup();
        if (buildingGroup != null)
        {
            Building[] buildings = buildingGroup.GetComponentsInChildren<Building>();

            foreach (Building building in buildings)
            {
                if (building.BuildingType == BuildingType.Sludgeworks)
                    result += building.Sludgeworks_CurrentAvailableShits;
            }
        }

        return result;
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
