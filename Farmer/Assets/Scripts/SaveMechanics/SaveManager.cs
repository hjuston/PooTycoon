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

        bf.Serialize(file, new Game());

        file.Close();
    }

    public static bool LoadData()
    {
        bool result = false;

        if (File.Exists(Application.persistentDataPath + SaveName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + SaveName, FileMode.Open);

            Game game = (Game)bf.Deserialize(file);
            game.Load();

            file.Close();

            result = true;
        }

        return result;
    }
}
