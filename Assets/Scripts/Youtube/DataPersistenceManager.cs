using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObject;

    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        }
        instance = this;
    }

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObject = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Load any saved data fromafile using the data handler
        this.gameData = dataHandler.Load();

        // if no data can be loaded,initialize toanew game
        if (this.gameData == null)
        {
            Debug.Log("No data was found.Initializing data to defaults.");
            NewGame();
        }

        // push the Loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObject)
        {
            dataPersistenceObj.LoadData(gameData);
        }
        Debug.Log("Loaded death count " +gameData.deathCount);
    }

    public void SaveGame()
    {
        // TODO-pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObject)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log("Save death count " + gameData.deathCount);

        // save that data toafile using the data handler
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}

