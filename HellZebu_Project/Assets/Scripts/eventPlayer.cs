using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [FMODUnity.EventRef]
    public string myEvent= "";

    public float timer;
    private float counter;
    private bool played = false;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            counter += Time.deltaTime;
            if (counter >= timer && !played)
            {
                FMODUnity.RuntimeManager.PlayOneShot(myEvent);
                played = true;

            }
        }
    }
}
