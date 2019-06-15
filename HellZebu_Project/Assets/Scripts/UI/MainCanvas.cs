using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance;

    public delegate void PauseOnDelegate();
    public static event PauseOnDelegate pauseOnEvent;

    public delegate void PauseOffDelegate();
    public static event PauseOffDelegate pauseOffEvent;

    [Header("Menus")]
    public GameObject pausePanel;
    public GameObject configPanel;
    public GameObject bindingPanel;
    public bool menuOpened;
    [Header("Hitmarker")]
    public Animation hitmarker;
    [Header("Spread")]
    public RectTransform spreadCrossFire;
    [Header("Player life")]
    public int playerLife;
    public GameObject[] UILifes;
    [Header("World change")]
    public Image wChangeImageCooldown;
    public Color wChangeBarColor;
    public Color wChangeHolderColor;
    public Image wChangeCooldownHolder;
    public Text wChangeTextCooldown;
    public Image fillImageWC;
    [HideInInspector]
    public float wChangeCD;
    [HideInInspector]
    public float maxWChangeCD;

    [Header("Splashes")]
    public Animation splashHeal;
    public ParticleSystem healParticle;
    public Animation splashDamage;
    public Animation splashChangeWorld;
    [Header("WChange Accesory")]
    public int maxBullets;
    public int currentBullets;
    public GameObject[] UIBullets;

    public float bulletRechargeTime;
    public float bulletRechargeTimer;

    public bool PARAR_EL_PUTO_PAUSE_DE_MIERDA_GRACIAS;

    [Header("Key Binding")]
    public List<Button> buttons;

    private Event keyEvent;
    KeyCode newKey;
    bool waitingForKey;
    string keyName;
    bool waitOut;

    [Header("Scenes")]
    public string sceneToLoad;
    [SerializeField] private Animation fadeAnimation;
    [SerializeField] private AnimationClip fadeOut;
    [SerializeField] private AnimationClip fadeIn;

    [FMODUnity.EventRef]
    public string confirmOption = "";

    [FMODUnity.EventRef]
    public string rechargeBullet = "";
    //Score

    public float killStreakTime;
    public float killStreakTimer;
    bool onKillStreak;
 
    public int pointMultiplier;
    public int playerScore;

    public Text killStreak;
    public Text score;
    public Text popUpText;
    public Animator popUp;
    public Animation killstreakAnim;
    public Text popUpComboText;
    public void ResetComboTime()
    {
        killStreakTimer = killStreakTime;
    }
    public void OnKillStreak(int points)
    {
        if (popUp != null)
        {
            popUp.SetBool("OnKill", true);
            popUpText.text = "+" + points * pointMultiplier;
            popUpComboText.text = "";
            if ((pointMultiplier + 1) % 5 == 0 || (pointMultiplier + 1) % 10 == 0)
            {
                //No he podido fixearlo, cuando points mutlilpier se queda en 5/10/15...t el play se hace correctamente, cuando acto seguido matas a un enemigo
                //la animacion sigue pasando pero el texto se borra.
                int multiplier = pointMultiplier + 1;
                string mstring = multiplier.ToString();
                popUpComboText.text = "X" + mstring;
                if (killstreakAnim.isPlaying == false)
                {
                    print("PLAY");
                    killstreakAnim.Play();
                }
            }
            playerScore += points * pointMultiplier;
            onKillStreak = true;
            pointMultiplier++;
            killStreakTimer = killStreakTime;
         
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else { Destroy(this.gameObject); }
       
     //   OpenPauseMenu(menuOpened);
    }
    private void Start()
    {
        pointMultiplier = 1;
        killStreakTimer = killStreakTime;
        onKillStreak = false;
        menuOpened = false;
        maxBullets = UIBullets.Length;
        currentBullets = maxBullets;
        UpdateKeyText();

        pausePanel.SetActive(false);
        configPanel.SetActive(false);
        bindingPanel.SetActive(false);
        Time.timeScale = 1;
        menuOpened = false;

    }
    private void Update()
    {

     
        killStreak.text = "x " + pointMultiplier;
        score.text = "Score: " + playerScore;
        if (onKillStreak)
        {
            popUp.SetBool("OnKill", false);
            killStreakTimer -= Time.deltaTime;
            if (killStreakTimer <= 0)
            {
                onKillStreak = false;
                pointMultiplier = 1;
               
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!PARAR_EL_PUTO_PAUSE_DE_MIERDA_GRACIAS)
            {
                menuOpened = !menuOpened;
                OpenPauseMenu(menuOpened);
            }
        }

        CheckWChangeCD();
        UIPlayerLife();
        ShowBullets();

        if (Input.GetKeyDown(KeyCode.F5))
        {
            DataManager.Instance.Save();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            DataManager.Instance.Load();
        }
    }
    public void OpenPauseMenu(bool open)
    {
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);

        if (open)
        {
            Cursor.visible = true;
            SetActivePausePanel(true);
            Time.timeScale = 0;
            pauseOnEvent();

        }
        else
        {
            Cursor.visible = false;
            pausePanel.SetActive(false);
            configPanel.SetActive(false);
            bindingPanel.SetActive(false);
            Time.timeScale = 1;
            menuOpened = false;
            pauseOffEvent();
        }

    }

    #region PanelSetActiveMethods
    public void SetActivePausePanel(bool active)
    {
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);

        if (active)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            pausePanel.SetActive(false);
        }
    }
    public void SetActiveConfigPanel(bool active)
    {
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);

        if (active)
        {
            configPanel.SetActive(true);
            
        }
        else
        {
            configPanel.SetActive(false);
            OptionsManager.Instance.SetOptionsValues();
        }
    }
    public void SetActiveBindingPanel(bool active)
    {
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);

        if (active)
        {
            bindingPanel.SetActive(true);

        }
        else
        {
            bindingPanel.SetActive(false);
        }
    }
    #endregion

    #region UIPlayer
    void UIPlayerLife()
    {
        for(int i = 0; i < UILifes.Length ; i++)
        {
            if (i <= playerLife-1)
            {
                UILifes[i].SetActive(true);
            }
            else { UILifes[i].SetActive(false); }
        }

    }
    public void CheckWChangeCD()
    {

        //wChangeTextCooldown.text = Mathf.Clamp(wChangeCD, 0, 5).ToString();
        wChangeImageCooldown.fillAmount = 1 - wChangeCD / maxWChangeCD; //hardocded cd
        if (wChangeImageCooldown.fillAmount == 1f) {
            wChangeImageCooldown.color = wChangeBarColor;
            wChangeCooldownHolder.color = Color.white;
        }
        else if (wChangeImageCooldown.color != Color.white){
            wChangeImageCooldown.color = Color.white;
            wChangeCooldownHolder.color = wChangeHolderColor;
        }

    }
    void ShowBullets()
    {
        for (int i = 0; i < UIBullets.Length; i++)
        {
            if (i <= currentBullets - 1)
            {
                UIBullets[i].SetActive(true);
               
            }
            else { UIBullets[i].SetActive(false); }
        }
        fillImageWC.fillAmount = 1 - bulletRechargeTimer / bulletRechargeTime;
        if (bulletRechargeTimer > 0)
        {
            bulletRechargeTimer -= Time.deltaTime;
        }
       

        if (bulletRechargeTimer <= 0)
        {
            if (currentBullets < maxBullets)
            {
                UIBullets[currentBullets].transform.parent.GetComponent<Animation>().Play();
                FMODUnity.RuntimeManager.PlayOneShot(rechargeBullet);
                currentBullets++;
                if (currentBullets == maxBullets) { }
                else { bulletRechargeTimer = bulletRechargeTime; }
               
            }
        }


    }
    #endregion

    public void ExitGame()
    {
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
    public void BackToMainMenu()
    {
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);

        SceneManager.LoadScene("MainMenu");
    }
    public void UpdateKeyText()
    {
        foreach(Button b in buttons)
        {
          //  print(b.name);
            if (b.name.Contains("Forward"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.moveForward.ToString();
            }
            if (b.name.Contains("Left"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.moveLeft.ToString();
            }
            if (b.name.Contains("Back"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.moveBackwards.ToString();
            }
            if (b.name.Contains("Right"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.moveRight.ToString();
            }
            if (b.name.Contains("Jump"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.jump.ToString();
            }
            if (b.name.Contains("Shoot"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.shoot.ToString();
            }
            if (b.name.Contains("Special"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.specialShoot.ToString();
            }
            if (b.name.Contains("Transfer"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.transferEnemy.ToString();
            }
            if (b.name.Contains("Weapon1"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.selectWeaponOne.ToString();
            }
            if (b.name.Contains("Weapon2"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.selectWeaponTwo.ToString();
            }
            if (b.name.Contains("WorldChange"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.changeWorld.ToString();
            }

        }
    }
    public void ResetInputs()
    {
        InputsManager.Instance.ResetToDefault();
        UpdateKeyText();
    }
    private void OnGUI()
    {
        
        keyEvent = Event.current;

        if (keyEvent.isKey && waitingForKey)
        {
            newKey = keyEvent.keyCode;
            waitingForKey = false;
            print("NEW KEY: " + newKey);
            waitOut = true;
        }
        if (keyEvent.type == EventType.MouseDown && waitingForKey)
        {
            newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Mouse"+keyEvent.button); 
            waitingForKey = false;
            print("NEW KEY: " + newKey);
            waitOut = true;
        }
   
      

    }
    public void StartEnterKey(string keyname)
    {
    
        if (waitingForKey == false)
        {
            
            StartCoroutine(CheckKey(keyname));
      
        }
    }
    public void EnterKey(string keyName)
    {
        //fixTimeWait = fixedTime;
        //fixWaitForKey = true;
        //this.keyName = keyName;
        if (waitingForKey == false)
        {

            StartCoroutine(CheckKey(keyName));

        }
    }
    private IEnumerator Wait()
    {
    
        while (waitOut == false)
        {
            yield return null;
        }
        
  
    }
    public IEnumerator CheckKey(string name)
    {
        waitingForKey = true;
        waitOut = false;
        yield return Wait();

      
        switch (name)
        {
            case "Forward":
                InputsManager.Instance.currentInputs.moveForward = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Left":
                InputsManager.Instance.currentInputs.moveLeft = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Back":
                InputsManager.Instance.currentInputs.moveBackwards = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Right":
                InputsManager.Instance.currentInputs.moveRight = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Jump":
                InputsManager.Instance.currentInputs.jump = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Shoot":
                InputsManager.Instance.currentInputs.shoot = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Transfer":
                InputsManager.Instance.currentInputs.transferEnemy = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Weapon1":
                InputsManager.Instance.currentInputs.selectWeaponOne = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Weapon2":
                InputsManager.Instance.currentInputs.selectWeaponTwo = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "WChange":
                InputsManager.Instance.currentInputs.changeWorld = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Special":
                InputsManager.Instance.currentInputs.specialShoot = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;

        }

    }

    public void ShowHitmarker()
    {
        hitmarker.Rewind();
        hitmarker.Play();
    }
    public void SplashHeal()
    {
        splashHeal.Rewind();
        splashHeal.Play();
    }
    public void SplashDamage()
    {
        splashDamage.Rewind();
        splashDamage.Play();
    }
    public void SplashChangeWorld()
    {
        splashChangeWorld.Rewind();
        splashChangeWorld.Play();
    }

    public void FadeOut()
    {
        fadeAnimation.clip = fadeOut;
        fadeAnimation.Play();
    }
    public void FadeIn() {
        fadeAnimation.clip = fadeIn;
        fadeAnimation.Play();
    }
    
    public void LoadScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }

}
