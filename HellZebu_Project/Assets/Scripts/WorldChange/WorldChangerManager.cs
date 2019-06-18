using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MaskNames { Default, HideFromCamera, Enemies, Weapon, NoCollisionWithPlayer }
public class WorldChangerManager : MonoBehaviour,DataInterface
{
    public static WorldChangerManager Instance;
    //Enums
    public enum Worlds { Fire, Ice };
    


    [Header("CollisionFilterAndCullingMask")]
    public Worlds currentWorld;
    public GameObject fireLevel;
    public GameObject iceLevel;

    public GameObject fireLevelHiddenObjects;
    public GameObject iceLevelHiddenObjects;

    [Header("General Values")]
    public float coolDown;
    private float coolDownTimer;

    [Header("Fog Settings")] 
    [SerializeField] private Color red;
    [SerializeField] private Color blue;
    [SerializeField] private Camera backgroundColorCamera;

    [Header("Lighting Settings")] 
    [SerializeField] private GameObject redLighting;
    [SerializeField] private GameObject blueLighting;

    [Header("UI")]
    [SerializeField] private Crossfire crossfireCanvas;

    [SerializeField] GameObject Particles;
    
    LevelData levelData;

    public delegate void WorldChangeDelegate();
    public static event WorldChangeDelegate worldChangeEvent;


    public void OnSave()
    {
       
    }

    public void OnLoad()
    {
       
    }
    void OnWorldChange() { }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        crossfireCanvas = GameObject.Find("Crossfire").GetComponent<Crossfire>();

        DataManager.savingEvent += OnSave;
        DataManager.loadingEvent += OnLoad;
        worldChangeEvent += OnWorldChange;
        currentWorld = Worlds.Ice;
        //crossfireCanvas.ChangeCrossfire(false);
        fireLevel.SetActive(false);
        ShowObjets(false, fireLevelHiddenObjects);
        iceLevel.SetActive(true);
        ShowObjets(true, iceLevelHiddenObjects);

        levelData = new LevelData();
        RenderSettings.fogColor = blue;
        backgroundColorCamera.backgroundColor = blue;
        if (blueLighting != null) {
            blueLighting.SetActive(true);
            redLighting.SetActive(false);
        }

    }
    public void PlayerWorldChange(GameObject target)
    {

        CollisionFilterAndCullingMaskWorldPlayerChange();
        worldChangeEvent();
        MainCanvas.Instance.SplashChangeWorld();
    }
    
    public void CollisionFilterAndCullingMaskWorldPlayerChange()
    {
        if (fireLevel.activeSelf)
        {
            fireLevel.SetActive(false);
            ShowObjets(false, fireLevelHiddenObjects);
            iceLevel.SetActive(true);
            ShowObjets(true, iceLevelHiddenObjects);
            currentWorld = Worlds.Ice;
            //crossfireCanvas.ChangeCrossfire(false);
            RenderSettings.fogColor = blue;
            backgroundColorCamera.backgroundColor = blue;
            if (blueLighting != null) {
                blueLighting.SetActive(true);
                redLighting.SetActive(false);
            }
            Particles.SetActive(false);

        }
        else
        {

            iceLevel.SetActive(false);
            ShowObjets(false, iceLevelHiddenObjects);
            fireLevel.SetActive(true);
            ShowObjets(true, fireLevelHiddenObjects);
            currentWorld = Worlds.Fire;
            //crossfireCanvas.ChangeCrossfire(true);
            RenderSettings.fogColor = red;
            backgroundColorCamera.backgroundColor = red;
            if (blueLighting != null) {
                blueLighting.SetActive(false);
                redLighting.SetActive(true);

            }
            Particles.SetActive(true);

        }
    }

    public void CollisionFilterAndCullingMaskWorldTargetChange(GameObject target)
    {
        if (fireLevel.activeSelf)
        {
            target.transform.parent = iceLevelHiddenObjects.transform;
            target.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
        }
        else
        {
            target.transform.parent = fireLevelHiddenObjects.transform;
            target.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
        }

    }

  

    private void ShowObjets(bool show, GameObject parentObject)
    {
        if (show)
        {
            foreach (Transform t in parentObject.GetComponentsInChildren<Transform>())
            {
                if (t.gameObject.CompareTag("Enemy"))
                    t.gameObject.layer = LayerMask.NameToLayer(MaskNames.Enemies.ToString());
                else
                    t.gameObject.layer = LayerMask.NameToLayer(MaskNames.Default.ToString());
            }

        }
        else
        {
            foreach (Transform t in parentObject.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
            }

        }

    }

}
