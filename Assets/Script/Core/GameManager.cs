using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameConstValues
{
<<<<<<< HEAD

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
=======
>>>>>>> 86c9093f55c3b781a43b05554e36499b4dd50bb9
}
