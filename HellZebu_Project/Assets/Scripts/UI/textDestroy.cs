using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    public float timer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            Destroy(this.gameObject, timer);
        }
    }
}
