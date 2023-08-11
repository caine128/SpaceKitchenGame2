using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviourPersistent<GameManager>
{
    private Camera _cameraMain;
    public Camera CameraMain { get => _cameraMain; }
    private void OnEnable()
    {
        SceneController.Instance.OnSceneLoaded += SetCameraMain;
    }
 
    private void SetCameraMain(object sender, SceneController.OnSceneLoadedEventArgs e)
    {
        _cameraMain = Camera.main;
        SceneController.Instance.OnSceneLoaded -= SetCameraMain;
    }
    private void OnDisable()
    {
        SceneController.Instance.OnSceneLoaded -= SetCameraMain;

    }
    /*#region Singleton Syntax

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }
    }*/

    //private void OnEnable()
    //{
    //    GetComponent<SceneController>().OnSceneLoaded += GameSceneConfig;
    //}

    //private void OnDisable()
    //{
    //    GetComponent<SceneController>().OnSceneLoaded -= GameSceneConfig;
    //}


    //private void GameSceneConfig(object sender, SceneController.OnSceneLoadedEventArgs e)
    //{
    //    foreach (IngredientGenerator ingredientGenerator in FindObjectsOfType<IngredientGenerator>())
    //    {
    //        ingredientGenerator.Config(isActive_IN: true);
    //    }
    //}
}
