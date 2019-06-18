using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreStatic : MonoBehaviour
{

    public static ScoreStatic Instance;
    public int playerScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else { Destroy(this.gameObject); }

        //   OpenPauseMenu(menuOpened);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
