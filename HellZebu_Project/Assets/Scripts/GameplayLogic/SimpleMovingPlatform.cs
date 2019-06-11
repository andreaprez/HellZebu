using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    public float time;
    private float timer;
    public float speed;
    public int direction;

    void Start()
    {
        direction = 1;
        timer = time;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            direction = -direction;
            timer = time;
        }
        transform.position += transform.forward*direction * speed * Time.deltaTime;
       
    }
}
