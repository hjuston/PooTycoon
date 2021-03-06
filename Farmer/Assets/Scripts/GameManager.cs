﻿using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    private bool EditMode = false;

    private Building selectedBuilding;

    public void SetCurrentlySelectedBuilding(Building building)
    {
        selectedBuilding = building;
    }

    public Building GetCurrentlySelectedBuilding()
    {
        return selectedBuilding;
    }

    void Start()
    {
        // Wczytywanie bazy danych budynków
        BuildingsDatabase.LoadDatabase();

        TimeToCollectShit = 0f;

        if (SaveManager.LoadData())
        {
            Helper.GetGUIManager().GameStats_SetCurrentMoneyInfo(GameStats.Instance.CurrentMoney);
        }

        // Skrypt 'zbierający' shit z budynków co 30 sekund
        InvokeRepeating("CollectMoney", 0f, 30f);
    }

    void CollectMoney()
    {
        // Jeżeli jest zabrany to ustawia przełącznik na nie zebrany (żeby zablokować podwójny dostęp)
        if (GameStats.Instance.ShitCollected)
        {
            GameStats.Instance.ShitCollected = false;
            GameStats.Instance.CollectShit();
        }
    }

    void OnApplicationQuit()
    {
        SaveManager.SaveData();
    }

    public void DecreaseTimeToCollectShit()
    {
        TimeToCollectShit += 1f;
        Helper.GetGUIManager().BuildingMode_TimeToCollectShitUpdate(TimeToCollectShit);
    }

    float TimeToCollectShit;
    void Update()
    {
        // Sprawdzanie czy budynek został kliknięty? Jeżeli tak to wyświetlić menu ulepszeń
        if (InputManager.GetInput() && !Helper.IsPointerAboveGUI())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.GetInputPosition());
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                if (!IsEditMode() && hitInfo.collider.tag == "Building")
                {
                    Helper.GetGUIManager().BuildingMode_SelectBuilding(hitInfo.collider.gameObject);
                }
            }
        }


        // Zbieranie shitu
        if (TimeToCollectShit >= 30f)
        {
            // Jeżeli minął odpowiedni czas to zbiera
            TimeToCollectShit = 0f;
            CollectMoney();
        }
        else
        {
            TimeToCollectShit += Time.deltaTime;
        }
        Helper.GetGUIManager().BuildingMode_TimeToCollectShitUpdate(TimeToCollectShit);
    }

    /// <summary>
    /// Metoda zwraca informację, czy gra znajduje się w trybie edycji
    /// </summary>
    /// <returns></returns>
    public bool IsEditMode()
    {
        return EditMode;
    }

    /// <summary>
    /// Metoda włącza tryb edycji
    /// </summary>
    public void SetEditMode(bool val)
    {
        EditMode = val;
    }


    public void UpgradeBuilding()
    {
        BigInteger buildingCost = selectedBuilding.CalculateCostForNextXLevels(upgradeBy);

        if (GameStats.Instance.GetCurrentMoney() >= buildingCost)
        {
            GameStats.Instance.SpendMoney(buildingCost);
            selectedBuilding.Upgrade(upgradeBy);

            GameStats.Instance.AddExperience(selectedBuilding.BuildingUpgardeExperience * upgradeBy);

            Helper.GetGUIManager().BuildingMode_SetBuildingInfo(selectedBuilding);
            if (upgradeBy != 1)
            {
                Helper.GetGUIManager().BuildingMode_UpdateBuildingLevelCostInfo(selectedBuilding, upgradeBy);
            }

            Helper.GetGUIManager().GameStats_TotalPeopleCountUpdate();
        }
    }

    int upgradeBy = 1;
    public void ToggleUpgradeBy(int levels)
    {
        upgradeBy = levels;
        Helper.GetGUIManager().BuildingMode_UpdateBuildingLevelCostInfo(selectedBuilding, upgradeBy);
        Helper.GetGUIManager().BuildingMode_SetMultiplierButtonActive();
    }

    public int GetUpgradeByValue()
    {
        return upgradeBy;
    }

    public void BuyBuildingUpgrade(Upgrade upgrade)
    {
        if (selectedBuilding != null && selectedBuilding.Upgrades.Contains(upgrade))
        {
            if (GameStats.Instance.GetCurrentMoney() >= new BigInteger(upgrade.Cost) && 
                selectedBuilding.BuildingLevel >= upgrade.RequiredLevel &&
                !upgrade.HasBeenBought)
            {
                // Activate upgrade
                if (selectedBuilding.ActivateUpgrade(upgrade))
                {
                    upgrade.HasBeenBought = true;
                    GameStats.Instance.SpendMoney(new BigInteger(upgrade.Cost));

                    Helper.GetGUIManager().BuildingMode_SetBuildingInfo(selectedBuilding);
                    if (upgradeBy != 1)
                    {
                        Helper.GetGUIManager().BuildingMode_UpdateBuildingLevelCostInfo(selectedBuilding, upgradeBy);
                    }

                    Helper.GetGUIManager().GameStats_TotalPeopleCountUpdate();
                }
            }
        }
    }
}
