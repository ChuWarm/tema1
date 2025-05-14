using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

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
    
    private async void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 순서 보장
        await MapGenerator.Instance.GenerateMap();
        GamePlayManager.Instance.InstantiatePlayer();
        CameraController.Instance.CameraInit();
    }
}
