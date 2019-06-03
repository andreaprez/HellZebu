using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openDoor : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject door, heart;
    public GameObject [] platforms;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DynamicWorldObject")
        {
            
            door.SetActive(false);
        }
        if (tag == "button")
        {
            foreach (GameObject plat in platforms)
            {
                plat.SetActive(true);
            }
            heart.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "DynamicWorldObject")
        {
            door.SetActive(true);
        }
    }
}
