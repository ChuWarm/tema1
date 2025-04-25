using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DataManager : MonoBehaviour
{
    public const string DATA_URL = "https://script.google.com/macros/s/AKfycbyUd5sR4KJbcy8iaSWgNcuPUJbMuK_OH9CYZA3Tli7Qf0BRNr-H8iiSCABHMcupnFnl/exec";

    public static DataManager Instance;

    public Dictionary<string, EnemyData> enemyDataDic = new();
    public Dictionary<string, UprageData> upgradeDataDic = new();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    void Start()
    {
        GetGoogleData().Forget();
    }

    async UniTaskVoid GetGoogleData()
    {
        var json = await LoadDataGoogleSheet(DATA_URL);

        var enemySheet = JsonUtility.FromJson<EnemyDataSheet>(json);

        print(json);

        for (int i = 0; i < enemySheet.enemyDataSheet.Length; i++)
        {
            var item = enemySheet.enemyDataSheet[i];
            enemyDataDic.Add(item.enemyID, item);
        }


        var memoryDataSheet = JsonUtility.FromJson<MemoryDataSheet>(json);

        for (int i = 0; i < memoryDataSheet.memoryUpgradeSheet.Length; i++)
        {
            var item = memoryDataSheet.memoryUpgradeSheet[i];
            upgradeDataDic.Add(item.upgradeID, item);
        }
    }

    async UniTask<string> LoadDataGoogleSheet(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] dataBytes = await client.GetByteArrayAsync(url);
                return Encoding.UTF8.GetString(dataBytes);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
                return null;
            }
        }
    }
}
