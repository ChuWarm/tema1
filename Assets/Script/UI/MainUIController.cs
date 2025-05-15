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
        mainButtons[1].onClick.AddListener(() =>
        {
            print("Show Config panel");
        });
        mainButtons[2].onClick.AddListener(() => Application.Quit());

        yield return new WaitUntil(() => !DataManager.IsReady);

        foreach (var item in mainButtons)
        {
            item.gameObject.SetActive(true);
            yield return new WaitForSeconds(.1f);
        }
    }
}
