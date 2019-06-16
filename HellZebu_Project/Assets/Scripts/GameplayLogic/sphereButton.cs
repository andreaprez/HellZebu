using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphereButton : MonoBehaviour
{
    // Start is called before the first frame update
    public bool openDoor;
    public GameObject door, trigger;
    public Animation animationComponent;
    void Start()
    {
        if (door)
        door.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
      

    }
    public void Damage()
    {
        if (openDoor) { 
            animationComponent.Play();
        trigger.SetActive(true);
    }
        else
         door.SetActive(true);

        this.gameObject.SetActive(false);
    }
}
