using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class InitManager : Singleton<InitManager>
{
    private void OnEnable()
    {
        HandleSceneLoaded().Forget();
    }
    
    private async UniTaskVoid HandleSceneLoaded()
    {
        // 순서 보장
        await MapGenerator.Instance.GenerateMap();
        GameEventBus.Publish<NewGameStart>(new NewGameStart());
        // GamePlayManager.Instance.InstantiatePlayer();
        CameraController.Instance.CameraInit();
        BGMManager.Instance.PlayBGM(BGMType.InGame);
    }
}
