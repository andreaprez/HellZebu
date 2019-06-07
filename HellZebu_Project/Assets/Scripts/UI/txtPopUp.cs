using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class txtPopUp : MonoBehaviour
{
    public float timer;
    public GameObject myText;
    private float counter;
    private bool inside;
    // Start is called before the first frame update
    void Start()
    {
        if (myText == null)
        {
            myText = transform.GetChild(0).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inside)
        {
            counter += Time.deltaTime;
        }
        if (counter>= timer)
        {
            myText.SetActive(true);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inside = true;
        }
    }
}
