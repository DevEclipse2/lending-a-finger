using UnityEngine;

using System.IO;
using UnityEditor;

public class SaveTransform : MonoBehaviour
{
    private string saveFilePath;
    [SerializeField]
    bool mainMenu;


    Vector3 position;
    Quaternion rotation;
    void Awake()
    {
        // Application.persistentDataPath is the standard, safe folder for save files on any platform
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    void Start()
    {
        // Automatically load the transform as soon as the object is initialized in the scene
        LoadPlayerTransform();
    }

    void OnDisable()
    {
        // Automatically save when the object is destroyed, the scene changes, or the game quits
        SavePlayerTransform();
    }

    public void SavePlayerTransform()
    {
        // Package the current transform into our serializable class
        TransformData data = new TransformData();
        if(!mainMenu)
        {
            data.position = transform.position;
            data.rotation = transform.rotation;
        }
        else
        {
            data.position = position;
            data.rotation = rotation;
        }
        data.difficulty = TitleScreen.difficulty;


        // Convert the class to a JSON string and write it to a file
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadPlayerTransform()
    {
        // Only attempt to load if the save file actually exists
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);

            // Convert the JSON string back into our TransformData class
            TransformData data = JsonUtility.FromJson<TransformData>(json);

            // Apply the loaded data to the player
            if(!mainMenu)
            {
                transform.position = new Vector3(data.position.x,data.position.y,0);
                transform.rotation = data.rotation;
            }
            else
            {
                position = new Vector3(data.position.x, data.position.y, 0);
                rotation = data.rotation;
                TitleScreen.difficulty = (byte)data.difficulty;
                GetComponent<TitleScreen>().canMod = false;
                GetComponent<TitleScreen>().hasSave.text = "using savedata";

            }
        }
    }
}

// We need a simple, serializable data container because Unity's Transform 
// component itself cannot be serialized directly to JSON.
[System.Serializable]
public class TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public int difficulty;
    //bool use

}