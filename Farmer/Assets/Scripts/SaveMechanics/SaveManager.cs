using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour {

    public static string SaveName = "/SavedGame.save";

	public static void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + SaveName);

        GameStats.Instance.LoadBuildingsToProperty();

        bf.Serialize(file, GameStats.Instance);
        file.Close();
    }

    public static void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + SaveName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + SaveName, FileMode.Open);
            GameStats.Instance = (GameStats)bf.Deserialize(file);

            GameStats.Instance.LoadBuildingToWorld();

            file.Close();
        }
    }
}
