using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
    [SerializeField] Button[] mainButtons;


    IEnumerator Start()
    {

        foreach (var item in mainButtons)
        {
            item.gameObject.SetActive(false);
        }

        mainButtons[0].onClick.AddListener(() => GameManager.Instance.LoadScene(1));

        yield return new WaitUntil(() => !DataManager.IsReady);

        foreach (var item in mainButtons)
        {
            item.gameObject.SetActive(true);
        }
    }

}
