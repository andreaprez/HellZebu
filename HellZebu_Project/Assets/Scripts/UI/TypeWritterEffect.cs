using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeWritterEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public float delay = 0.2f;
    public string fullText;
    private string currentText = "";
    public Text myText;
    void Start()
    {
        myText = GetComponent<Text>();
        StartCoroutine(ShowText());
    }

    // Update is called once per frame
    void Update()
    {
        
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
