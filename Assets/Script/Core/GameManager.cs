using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameConstValues
{
    public const int REWARED_SCENE_BUILDINDEX = 2;
}


public class GameManager : Singleton<GameManager>
{
    bool isAddtionalSceneLoaded = false;

    public void LoadScene(int targetScene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if(isAddtionalSceneLoaded)
            return;

        SceneManager.LoadScene(targetScene, mode);
        if (mode.Equals(LoadSceneMode.Additive))
            isAddtionalSceneLoaded = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void UnloadScene(int targetScene) 
    {
        if (!isAddtionalSceneLoaded)
            return;

        SceneManager.UnloadSceneAsync(targetScene);
        isAddtionalSceneLoaded = false;
    }
}
