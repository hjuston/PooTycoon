using UnityEngine;
using System.Collections;

public class GameStats : MonoBehaviour
{
   private BigInteger _currentMoney = new BigInteger("123123331");

   private int _level = 1;
   private int _requiredExperience = 100;
   private int _currentExperience = 0;

   private int Level
   {
      get { return _level; }
      set { _level = value; Helper.GetGUIManager().GameStats_SetLevelText(value); }
   }

   private int RequiredExperience
   {
      get { return _requiredExperience; }
      set { _requiredExperience = value; Helper.GetGUIManager().GameStats_SetExperienceValue(_currentExperience, value);
            Helper.GetGUIManager().GameStats_SetExperienceBarValue((float)_currentExperience / (float)_requiredExperience);
         }
   }

   private int CurrentExperience
   {
      get { return _currentExperience; }
      set { _currentExperience = value; Helper.GetGUIManager().GameStats_SetExperienceValue(value, _requiredExperience);
            Helper.GetGUIManager().GameStats_SetExperienceBarValue((float)_currentExperience / (float)_requiredExperience);
         }
   }

   private BigInteger CurrentMoney
   {
      get { return _currentMoney; }
      set {
            _currentMoney = value;

            // Listenery
            Helper.GetGUIManager().GameStats_SetCurrentMoneyInfo(value);
            Helper.GetGUIManager().BuildingMode_UpdateBuildingLevelCostInfo(Helper.GetGameManager().GetCurrentlySelectedBuilding(), Helper.GetGameManager().GetUpgradeByValue());
          }
   }

   void Start()
   {
      InvokeRepeating("CollectMoney", 0f, 1f);
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
   void CollectMoney()
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

      GameObject buildingGroup = GameObject.FindGameObjectWithTag(CONSTS.BuildingsGroupTag);
      if (buildingGroup != null)
      {
         Building[] buildings = buildingGroup.GetComponentsInChildren<Building>();

         foreach (Building building in buildings)
         {
            income += building.GetIncome();
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
   #endregion
}
