using UnityEngine;
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
        SaveManager.LoadData();

        InvokeRepeating("CollectMoney", 0f, 1f);
   }

    void CollectMoney()
    {
        
    }

    void OnApplicationQuit()
    {
        SaveManager.SaveData();
    }

   void Update()
   {
      // Sprawdzanie czy budynek został kliknięty? Jeżeli tak to wyświetlić menu ulepszeń
      if (Input.GetMouseButtonDown(0))//(Input.touchCount > 0)
      {
            //if (Input.GetTouch(0).phase == TouchPhase.Began && !Helper.IsPointerAboveGUI())
            //{
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//Input.GetTouch(0).position);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
               if (!IsEditMode() && hitInfo.collider.tag == "Building")
               {
                  Building buildingScript = hitInfo.collider.GetComponent<Building>();
                  if (buildingScript != null)
                  {
                     Helper.GetGUIManager().BuildingMode_SetBuildingInfo(buildingScript);

                     // Wyświetlanie markera
                     Helper.GetGUIManager().BuildingMode_CurrentBuildingMarkerShow(hitInfo.collider.gameObject);
                  }
               }
            }
         //}
      }
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

         Helper.GetGUIManager().GameStats_SetIncomeInfo(GameStats.Instance.GetCurrentIncome());
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
         if (GameStats.Instance.GetCurrentMoney() >= new BigInteger(upgrade.Cost) && selectedBuilding.BuildingLevel >= upgrade.RequiredLevel)
         {
            // Activate upgrade
            if (selectedBuilding.ActivateUpgrade(upgrade))
            {
                    GameStats.Instance.SpendMoney(new BigInteger(upgrade.Cost));

               Helper.GetGUIManager().BuildingMode_SetBuildingInfo(selectedBuilding);
               if (upgradeBy != 1)
               {
                  Helper.GetGUIManager().BuildingMode_UpdateBuildingLevelCostInfo(selectedBuilding, upgradeBy);
               }

               Helper.GetGUIManager().GameStats_SetIncomeInfo(GameStats.Instance.GetCurrentIncome());
            }
         }
      }
   }
}
