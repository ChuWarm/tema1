using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameConstValues
{

}


public class GameManager : Singleton<GameManager>
{
    public void LoadScene(int targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
