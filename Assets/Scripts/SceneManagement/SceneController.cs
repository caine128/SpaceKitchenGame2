using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController _instance;
    public static SceneController Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    private bool cr_running = false;
    private string ui_AdditiveSceneName;

    public event EventHandler<OnSceneLoadedEventArgs> OnSceneLoaded;
    public class OnSceneLoadedEventArgs
    {
        public SceneName.Scene targetScene;
        public int initializeOrder;
    }

    private void Awake()
    {
        if(_instance !=null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            lock (_lock)
            {
                if(_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }

        ui_AdditiveSceneName = SceneName.Scene.UI_Additive.ToString();

    }

    private void Update()  ///THIS IS FOR TEST PURPOSES
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (cr_running) return;
            StartCoroutine(LoadScene(SceneName.Scene.Game_Scene));
        }
    }

    IEnumerator LoadScene(SceneName.Scene targetScene) // Need To Prevent  It From Pressing Load Again CR running implemented!!!!
    {
        cr_running = true;

        string targetSceneName = targetScene.ToString();
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetSceneName);

        while (!asyncOperation.isDone)
        {

            yield return null;
        }

        asyncOperation = SceneManager.LoadSceneAsync(ui_AdditiveSceneName, LoadSceneMode.Additive);

        while (!asyncOperation.isDone)
        {

            yield return null;
        }

        OnSceneLoaded?.Invoke(this, new OnSceneLoadedEventArgs { targetScene = targetScene, initializeOrder=0 }); // latertoworkwith initializeorder !!!! 

        cr_running = false;
    }
}
