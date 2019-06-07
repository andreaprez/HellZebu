using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemonDialogueManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> dialogues;
    [SerializeField] private float dialoguesDuration;
    [SerializeField] private AnimationClip textFadeIn;
    [SerializeField] private AnimationClip textFadeOut;

    private Controller playerController;
    private int currentScene;
    private float timer;
    private Animation textFadeAnimation;
    private bool finished = false;
    void Awake()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<Controller>();
        playerController.movementLocked = true;
        currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene > 0)
        {
            textFadeAnimation = dialogues[currentScene - 1].GetComponent<Animation>();
            dialogues[currentScene - 1].SetActive(true);
        }
    }

    private void Update()
    {
        if (!finished)
        {
            timer += Time.deltaTime;
            if (timer >= dialoguesDuration)
            {
                textFadeAnimation.clip = textFadeOut;
                textFadeAnimation.Play();
                MainCanvas.Instance.FadeIn();
                playerController.movementLocked = false;
                finished = true;
            }
        }
    }
}
