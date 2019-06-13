using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System;
public class HighScore : MonoBehaviour
{
    HighScoreData highScoreData;
    int highScore;
    public Text highScoreText;
    string highScoreFile;


    string customPath;
    public static HighScore Instance;


    private void Update()
    {
        if (MainCanvas.Instance.playerScore > highScoreData.highScore)
        {
            saveHighScore();
        }


        highScoreText.text = "high score:" + highScoreData.highScore;
    }

    private void Awake()
    {
        print("AWAKE");
        customPath = Application.persistentDataPath + "/highScore.json";
        Instance = this;
        highScoreData = new HighScoreData();

        if (File.Exists(customPath))
        {
            highScoreFile = File.ReadAllText(customPath);

            highScoreData = JsonUtility.FromJson<HighScoreData>(highScoreFile);

        }
        else
        {
            print("CREATE");
            //Usar Dispose() despues de crear un archivo si vamos a leer o escribir en el.
            File.CreateText(customPath).Dispose();


            highScoreFile = JsonUtility.ToJson(highScoreData);
            File.WriteAllText(customPath, highScoreFile);



        }

        print(highScoreData.highScore);
        


    }
    public void saveHighScore()
    {
        highScoreData.highScore = MainCanvas.Instance.playerScore;
        highScoreFile = JsonUtility.ToJson(highScoreData);
        File.WriteAllText(customPath, highScoreFile);

    }







}
[System.Serializable]
public class HighScoreData
{
    public int highScore;
    public HighScoreData() { }
}
