using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    public bool playerAbove = false;
    public float counter = 0, counterRespawn =0;
    public float cooldown=5, cooldownRespawn=20;
    public MeshRenderer myMesh;
    public Collider myCollider;
    Material myRender;
    void Start()
    {
        if (!myMesh)
        {
            myMesh = gameObject.GetComponent<MeshRenderer>();
        }
        if (!myCollider)
        {
            myCollider = gameObject.GetComponent<Collider>();
        }
        if (!myRender)
        {
            myRender = myMesh.material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAbove)
        {
            counter += Time.deltaTime;
            StartCoroutine(FadeImage(true));

        }
        if (counter >= cooldown)
        {
          //  myMesh.enabled = false;
            myCollider.enabled = false;


            counter = 0;

            playerAbove = false;
        }
        if (!playerAbove&& !myCollider.enabled)
        {
            counterRespawn += Time.deltaTime;
            StartCoroutine(FadeImage(false));

            if (counterRespawn>= cooldownRespawn)
            {
               

                    counterRespawn = 0;

                    //myMesh.enabled = true;
                    myCollider.enabled = true;

                
            }
        }
    }
    public IEnumerator FadeImage(bool fadeAway)
    {
        Color old;

        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = cooldown; i >= 0; i -= Time.deltaTime)
            {
                float alpha = i / cooldown;
                old = myMesh.material.color;
                myMesh.material.color = new Color(old.r, old.g, old.b, alpha);

                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= cooldownRespawn; i += Time.deltaTime)
            {
                float alpha = i / cooldownRespawn;

                old = myMesh.material.color;
                myMesh.material.color = new Color(old.r, old.g, old.b, alpha);
                yield return null;
            }
        }
  

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            counter = 0;
            playerAbove = true;
            
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //counter = 0;
            //counterRespawn = 0;
          //  playerAbove = false;
        }
    }
}
