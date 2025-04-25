using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitManager : Singleton<InitManager>
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }
    
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 순서 보장
        MapGenerator.Instance.GenerateMap();
        GameManager.Instance.InstantiatePlayer();
        CameraController.Instance.CameraInit();
    }
}
