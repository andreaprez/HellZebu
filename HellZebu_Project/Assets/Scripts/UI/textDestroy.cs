using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    public float timer;
    public GameObject otherTxt;
    public bool rifleAcc;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* if (this.gameObject.activeInHierarchy)
         {
             Destroy(this.gameObject, timer);
         }*/
        if (!rifleAcc)
        {


            if (otherTxt.activeInHierarchy)
            {
                Destroy(this.gameObject);
            }
        }
       /* if (rifleAcc && this.gameObject.activeInHierarchy)
        {
            Destroy(this.gameObject, timer);

        }*/
    }
}
