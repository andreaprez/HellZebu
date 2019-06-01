using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EnumNamespace;

#if UNITY_EDITOR
public class SpawnManagerEditorTool : EditorWindow
{
    [MenuItem("Window/Spawn Manager Tool")]
    static void InitiateWindow()
    {
        SpawnManagerEditorTool window = (SpawnManagerEditorTool)GetWindow(typeof(SpawnManagerEditorTool));
        window.minSize = new Vector3(550, 650);
       
        window.Show();

    }
    public static SpawnManagerData spawnConfig;
    string spawnConfigFileName;

    Texture2D headerTexture, spawnValuesTexture;
    Rect headerRect, spawnValuesRect;
    GUILayoutOption[] options = new GUILayoutOption[1];
    GUILayoutOption[] options2 = new GUILayoutOption[1];
    GUIContent[] content = new GUIContent[1];

    int waveNumber;
    int showNumber;

    Transform[] randomPoints;


    Transform[] fixedPoints;
    int[] fixedInts;
    string[] fixedStrings;

    int randomPointsAmount;
    int fixedPointsAmount;
    SpawnType[] currentSpawnTypes;
    Vector2 scrollPos;
    private void OnEnable()
    {
        spawnConfig = (SpawnManagerData)CreateInstance(typeof(SpawnManagerData));
        options[0] = GUILayout.Width(150);
        options2[0]= GUILayout.Width(50);
        InitiateTextures();
        showNumber = -1;
    





    }



    private void OnGUI()
    {
        

        Sections();
        Header();
        SpawnValues();

    }
    void Header()
    {

        GUILayout.BeginArea(headerRect);
        GUILayout.Label("Spawn Manager Tool");
        GUILayout.EndArea();

    }
    void SpawnValues()
    {
        GUILayout.BeginArea(spawnValuesRect);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height-150));
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Spawn configuration file name:");
        spawnConfigFileName = EditorGUILayout.TextField(spawnConfigFileName, options);
        GUILayout.EndHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Random spawn points amount:");
        randomPointsAmount= EditorGUILayout.IntField(randomPointsAmount,options);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create random spawn points array"))
        {
            
            randomPoints = new Transform[randomPointsAmount];
          
           
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (randomPoints != null&&randomPoints.Length==randomPointsAmount)
        {
            for (int ii = 0; ii < randomPointsAmount; ii++)
            {
                if (ii % 3 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                GUILayout.Label("Nº: " + ii);
                randomPoints[ii] = (Transform)EditorGUILayout.ObjectField(randomPoints[ii], typeof(Transform), true);

            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Fixed spawn points amount:");
        fixedPointsAmount = EditorGUILayout.IntField(fixedPointsAmount, options);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create fixed spawn points array"))
        {

            fixedPoints = new Transform[fixedPointsAmount];
            fixedInts = new int[fixedPointsAmount];
            fixedStrings = new string[fixedPointsAmount];
           
            for (int a = 0; a < fixedPointsAmount; a++)
            {
                fixedInts[a] = a;
                Debug.Log("A: " + a);
                Debug.Log("FixedInt: " + fixedInts[a]);
            }

        }
        if (fixedPoints != null&&fixedPoints.Length==fixedPointsAmount)
        {
            for (int r = 0; r < fixedPointsAmount; r++)
            {
                if (fixedPoints[r] != null)
                {
                    fixedStrings[r] = fixedPoints[r].name;
                }
            }
           
        }




        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (fixedPoints != null&&fixedPoints.Length==fixedPointsAmount)
        {
            for (int ii = 0; ii < fixedPointsAmount; ii++)
            {
                if (ii % 3 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                GUILayout.Label("Nº: " + ii);
                fixedPoints[ii] = (Transform)EditorGUILayout.ObjectField(fixedPoints[ii], typeof(Transform), true);

            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Waves amount:");
        spawnConfig.wavesAmount = EditorGUILayout.IntField(spawnConfig.wavesAmount, options);
        GUILayout.EndHorizontal();



        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create waves list"))
        {
            spawnConfig.waves = new Wave[spawnConfig.wavesAmount];
            for (int e = 0; e < spawnConfig.waves.Length; e++)
            {

                spawnConfig.waves[e] = new Wave();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(25);

        GUILayout.Label("Select wave number:");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Nº:");
        int[] ints = new int[spawnConfig.wavesAmount];
        string[] strings = new string[spawnConfig.wavesAmount];
        for (int i = 0; i < spawnConfig.wavesAmount; i++)
        {
            ints[i] = i;
            strings[i] = i.ToString();
        }
        waveNumber = EditorGUILayout.IntPopup(waveNumber, strings, ints, options);
        GUILayout.EndHorizontal();

    
        if (spawnConfig.waves != null&&spawnConfig.waves.Length==spawnConfig.wavesAmount)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Time between waves:");
            spawnConfig.waves[waveNumber].timeBetweenWaves = EditorGUILayout.FloatField(spawnConfig.waves[waveNumber].timeBetweenWaves, options);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enemies amount:");
            spawnConfig.waves[waveNumber].enemiesAmount = EditorGUILayout.IntField(spawnConfig.waves[waveNumber].enemiesAmount, options);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Spawn time between enemies:");
            spawnConfig.waves[waveNumber].spawnTimeBetweenEnemies = EditorGUILayout.FloatField(spawnConfig.waves[waveNumber].spawnTimeBetweenEnemies, options);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create enemies list"))
            {
                spawnConfig.waves[waveNumber].enemies = new EnemeiesWaveClass[spawnConfig.waves[waveNumber].enemiesAmount];
                for (int ee = 0; ee < spawnConfig.waves[waveNumber].enemies.Length; ee++)
                {

                    spawnConfig.waves[waveNumber].enemies[ee] = new EnemeiesWaveClass();
                   

                }
                currentSpawnTypes = new SpawnType[spawnConfig.waves[waveNumber].enemies.Length];

            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            
       
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (spawnConfig.waves[waveNumber].enemies != null)
            {

                for (int u = 0; u < spawnConfig.waves[waveNumber].enemies.Length; u++)
                {
                    if (u % 1 == 0)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                    GUILayout.Label("Nº: " + u);
                    spawnConfig.waves[waveNumber].enemies[u].type = (EnemiesTypes)EditorGUILayout.EnumPopup(spawnConfig.waves[waveNumber].enemies[u].type, options);
                    currentSpawnTypes[u] = (SpawnType)EditorGUILayout.EnumPopup(currentSpawnTypes[u], options);
                    if (currentSpawnTypes[u] == SpawnType.FIXED)
                    {
                       
                        spawnConfig.waves[waveNumber].enemies[u].spawnPointID = EditorGUILayout.IntPopup(spawnConfig.waves[waveNumber].enemies[u].spawnPointID, fixedStrings, fixedInts, options);
                        spawnConfig.waves[waveNumber].enemies[u].spawnPosition = fixedPoints[spawnConfig.waves[waveNumber].enemies[u].spawnPointID].position;
                    
                        Debug.Log("SpawnID: "+spawnConfig.waves[waveNumber].enemies[u].spawnPointID);
                    }
                    else
                    {
                        spawnConfig.waves[waveNumber].enemies[u].spawnPosition = randomPoints[Random.Range(0, randomPoints.Length)].position;
                    }

                }


            }
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("You need to create a wave list and create every enemies list on each wave, before creating the spawner configuration data asset.", MessageType.Info);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create spawner configuration data asset"))
        {
            string path = "Assets/Resources/"+spawnConfigFileName+".asset";
            AssetDatabase.CreateAsset(spawnConfig, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();



     


        GUILayout.EndArea();
        EditorGUILayout.EndScrollView();
    }

    #region DrawSectionAndTextures
    void Sections()
    {
        //Header
        headerRect.x = headerRect.y = 0;
        headerRect.width = Screen.width;
        headerRect.height = 40;
        GUI.DrawTexture(headerRect, headerTexture);

        //SpawnValues
        spawnValuesRect.x = 0;
        spawnValuesRect.y = 41;
        spawnValuesRect.width = Screen.width;
        spawnValuesRect.height = Screen.height;
        GUI.DrawTexture(spawnValuesRect, spawnValuesTexture);



    }
    void InitiateTextures()
    {


        headerTexture = new Texture2D(1, 1);
        headerTexture.SetPixel(0, 0, Color.grey);
        headerTexture.Apply();

        spawnValuesTexture = new Texture2D(1, 1);
        spawnValuesTexture.SetPixel(0, 0, new Color(0.7f, 0.7f, 0.7f));
        spawnValuesTexture.Apply();


    }
    #endregion

}
#endif