using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastText : MonoBehaviour
{
    // Start is called before the first frame update
    public Text HighScoreSentence, myScore, highScore;
    public float delay = 0.2f, timer1,timer2,timer3;
    public string fullText;
    private string currentText = "";
   
    private float counter;
    private bool shown1 = false, shown2 = false, shown3 = false;

    private void Awake()
    {
     
    }
    void Start()
    {
        if (MainCanvas.Instance.playerScore >= HighScore.Instance.getHighScore())
        {
            HighScoreSentence.text = "You made a new best score, but it aint an impressive one";

        }
        else
        {
            HighScoreSentence.text = "But your score is even worse than the last time... \n¿ what kind of meaningless fly doesn't improve?";
      
        }

        myScore.text = "FINAL SCORE: " + MainCanvas.Instance.playerScore;
        highScore.text = "HIGH SCORE: " + HighScore.Instance.getHighScore();
        HighScoreSentence.gameObject.SetActive(false);
        myScore.gameObject.SetActive(false);
        highScore.gameObject.SetActive(false);

    }

    void Update()
    {
        counter += Time.deltaTime;
        if (counter >= timer1 && !shown1)
        {
            shown1 = true;
            fullText = HighScoreSentence.text;
            HighScoreSentence.text = "";
            HighScoreSentence.gameObject.SetActive(true);

            StartCoroutine(ShowText(HighScoreSentence));

        }
        if (counter >= timer2 && !shown2)
        {
            shown2 = true;
            fullText = myScore.text;
            myScore.text = "";
            myScore.gameObject.SetActive(true);

            StartCoroutine(ShowText(myScore));

        }
        if (counter >= timer3 && !shown3)
        {
            shown3 = true;
            fullText = highScore.text;
      
            highScore.text = "";
            highScore.gameObject.SetActive(true);

            StartCoroutine(ShowText(highScore));

        }
    }
    IEnumerator ShowText(Text myText)
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            myText.text = currentText;
            yield return new WaitForSeconds(delay);
        }

    }
}
