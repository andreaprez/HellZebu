using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameData : Data
{
    public PlayerData playerData = new PlayerData();
    public LevelData levelData = new LevelData();
    public ScoreData scoreData= new ScoreData();
    public GameData() { }

}
