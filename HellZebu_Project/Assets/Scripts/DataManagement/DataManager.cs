using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{

    private string savePath;
    private string optionsPath;
    private string options;
    private string saveGame;

    public static GameData gameData;
    public static PlayerOptions playerOptions;

    public delegate void SavingDelegate();
    public static event SavingDelegate savingEvent;

    public delegate void LoadingDelegate();
    public static event LoadingDelegate loadingEvent;

    public static DataManager Instance;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/savegame.json";
        gameData = new GameData();
        if (File.Exists(savePath))
        {

        }
        else
        {
            CreateSaveFile();
        }
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else { Destroy(this.gameObject); }
    }
    public void CreateSaveFile()
    {
        savePath = Application.persistentDataPath + "/savegame.json";
        File.CreateText(savePath);
       
    }
    public void Save()
    {
        print("SAVING GAME");
        savingEvent();
        saveGame = JsonUtility.ToJson(gameData);
        File.WriteAllText(savePath, saveGame);
    }
    public void Load()
    {
        print("LOADING GAME");
        
        saveGame = File.ReadAllText(savePath);
        gameData = JsonUtility.FromJson<GameData>(saveGame);
        loadingEvent();
    }
    public void LoadPlayerOptions()
    {

    }
    public void SavePlayerOptions()
    {

    }
    
    private void OnLevelWasLoaded(int level)
    {
      
    }

}
