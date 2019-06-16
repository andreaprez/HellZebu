using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openDoor : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject door, heart;
    public GameObject [] platforms;
    public Animation myAnimation;
    public GameObject trigger;
    private bool played = false;
      
    void Start()
    {
        if (!myAnimation)
        {
            myAnimation = GetComponent<Animation>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tag == "Door")
        {
            if (door == null && heart == null)
            {
                if (!played)
                {
                    trigger.SetActive(true);
                 //   Debug.Log("opening");
                    myAnimation.Play();
                    played = true;               
                }

            }
        }
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
