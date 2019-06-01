using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ScoreData : Data
{
    public List<Score> scores;
    public ScoreData() { }
}
[System.Serializable]
public class Score : Data
{
    public string playerName;
    public int playerScore;
    public Score() { }
}