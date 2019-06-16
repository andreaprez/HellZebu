using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Rendering.PostProcessing;
public class OptionsManager : MonoBehaviour
{
    public OptionsConfiguration defaultOptions;
    public OptionsConfiguration customOptions;
    private string customOptionsFile;

   
    string customPath;
    public static OptionsManager Instance;

    public Slider bright;
    public Toggle fullScreen;
    public Slider FOV;
    public Toggle invertXAxis;
    public Slider mouseSensitivity;
    PostProcessVolume postProcessVolume;
    ColorGrading colorGrading;
    private void Awake()
    {
      
        customPath = Application.persistentDataPath + "/customOptionsConfig.json";
        Instance = this;
        

        if (File.Exists(customPath))
        {
            customOptionsFile = File.ReadAllText(customPath);
           
            customOptions= JsonUtility.FromJson<OptionsConfiguration>(customOptionsFile);

        }
        else
        {
            print("CREATE");
            //Usar Dispose() despues de crear un archivo si vamos a leer o escribir en el.
            File.CreateText(customPath).Dispose();
            
            customOptions = defaultOptions;
            customOptionsFile = JsonUtility.ToJson(customOptions);
            File.WriteAllText(customPath, customOptionsFile);



        }
       

        SetOptionsValues();


    }
    private void Start()
    {
        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out colorGrading);
    }
    private void Update()
    {

        colorGrading.postExposure.value = Mathf.Lerp(0.6f, 1.9f, bright.value);
       
      
    }
    public void SetOptionsValues()
    {

        bright.value = customOptions.bright;
   
        fullScreen.isOn = customOptions.fullScreen;
        FOV.value = customOptions.FOV;
        invertXAxis.isOn = customOptions.invertXAxis;
   
        mouseSensitivity.value = customOptions.mouseSensitivity;

    }
    public void SaveOptionsValues()
    {
        customOptions.bright = bright.value;
     
        customOptions.fullScreen = fullScreen.isOn;
        customOptions.FOV = FOV.value;
        customOptions.invertXAxis = invertXAxis.isOn;
       
        customOptions.mouseSensitivity = mouseSensitivity.value;
        customOptionsFile = JsonUtility.ToJson(customOptions);
        File.WriteAllText(customPath, customOptionsFile);

    }
    public void SetToDefault()
    {

        customOptions.bright = defaultOptions.bright;
    
        customOptions.fullScreen = defaultOptions.fullScreen;
        customOptions.FOV = defaultOptions.FOV;
        customOptions.invertXAxis = defaultOptions.invertXAxis;

        customOptions.mouseSensitivity = defaultOptions.mouseSensitivity;

     
        SetOptionsValues();
    }
   

}
