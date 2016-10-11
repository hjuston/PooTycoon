using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Game
{
    // GameStats
    public int Level;
    public BigInteger CurrentMoney;
    public int CurrentExperience;
    public int RequiredExperience;

    // Buildings
    public SerializableBuilding[] Buildings;

    #region ** Save
    public Game()
    {
        SaveGameStats();
        SaveBuildings();
    }

    void SaveGameStats()
    {
        Level = GameStats.Instance.Level;
        CurrentMoney = GameStats.Instance.CurrentMoney;
        CurrentExperience = GameStats.Instance.CurrentExperience;
        RequiredExperience = GameStats.Instance.RequiredExperience;
    }

    void SaveBuildings()
    {
        List<SerializableBuilding> list = new List<SerializableBuilding>();

        // Pętla na każdym obiekcie w grupie Buildings
        foreach (Transform transform in Helper.GetBuildingsGroup().transform)
        {
            Building building = transform.gameObject.GetComponent<Building>();
            if (building != null)
            {
                // Zapamiętywanie pozycji, rotacji i właściwości budynku
                list.Add(new SerializableBuilding(building, transform.position, transform.localRotation));
            }
        }

        Buildings = list.ToArray();
    }
    #endregion

    #region ** Load
    public bool Load()
    {
        bool result = true;

        try
        {
            LoadGameStats();
            LoadBuildings();
        }
        catch
        {
            result = false;
        }

        return result;
    }

    void LoadGameStats()
    {
        GameStats.Instance.Level = Level;
        GameStats.Instance.RequiredExperience = RequiredExperience;
        GameStats.Instance.CurrentExperience = CurrentExperience;
        GameStats.Instance.CurrentMoney = CurrentMoney;
    }

    void LoadBuildings()
    {
        // Jeżeli jakiekolwiek budowle są zapisane we właściwości
        if (Buildings != null)
        {
            foreach (SerializableBuilding serializableBuilding in Buildings)
            {
                // Pobieranie budynków z bazy danych
                GameObject[] dbBuildings = BuildingsDatabase.GetBuildingsByType(serializableBuilding.BuildingType);
                foreach (GameObject dbBuilding in dbBuildings)
                {
                    // Wczytywanie skryptu danego budynku w celu znalezienia odpowiedniego budynku po nazwie
                    Building dbBuildingScript = dbBuilding.GetComponent<Building>();
                    if (dbBuildingScript != null)
                    {
                        if (dbBuildingScript.Name == serializableBuilding.BuildingName)
                        {
                            // Znaleziono budynek - tworzenie obiektu budynku i przypisanie go do grupy Buildings
                            GameObject objInst = GameObject.Instantiate(dbBuilding);
                            objInst.transform.SetParent(Helper.GetBuildingsGroup().transform, true);

                            Building instanceScript = objInst.GetComponent<Building>();
                            if (instanceScript != null)
                            {
                                instanceScript.LoadBuilding(serializableBuilding);
                            }
                        }
                    }
                }

            }
        }
    }
    #endregion

}

