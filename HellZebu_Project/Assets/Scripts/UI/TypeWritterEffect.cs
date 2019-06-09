using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeWritterEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public float delay = 0.2f, timer;
    public string fullText;
    private string currentText = "";
    public Text myText;
    private float counter;
    private bool shown = false;
    void Start()
    {
        myText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter>= timer && !shown)
        {
            shown = true;

            StartCoroutine(ShowText());

        }
    }
    IEnumerator ShowText()
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            myText.text = currentText;
            yield return new WaitForSeconds(delay);
        }

    }
}
