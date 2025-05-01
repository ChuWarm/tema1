using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public static class DataSheetURLHolder
{
    public static string DATA_SHEET_URL = "https://script.google.com/macros/s/AKfycbyUd5sR4KJbcy8iaSWgNcuPUJbMuK_OH9CYZA3Tli7Qf0BRNr-H8iiSCABHMcupnFnl/exec";
}

public class DataManager : Singleton<DataManager>
{  

battle
    public Dictionary<string, EnemyData> enemyDatas = new();
    public Dictionary<string, MemoryUpgradeData> upgradeDatas = new();


    void Start()
    {
        GetGoogleData().Forget();
    }

    async UniTaskVoid GetGoogleData()
    {

        var json = await LoadDataGoogleSheet(DataSheetURLHolder.DATA_SHEET_URL);

        var enemySheet = JsonUtility.FromJson<EnemyDataSheet>(json);

        print(json);

        for (int i = 0; i < enemySheet.enemyDataSheet.Length; i++)
        {
            var item = enemySheet.enemyDataSheet[i];
 battle
            enemyDatas.Add(item.enemyID, item);
        }


        var memoryDataSheet = JsonUtility.FromJson<MemoryUpgradeDataSheet>(json);

        for (int i = 0; i < memoryDataSheet.memoryUpgradeSheet.Length; i++)
        {
            var item = memoryDataSheet.memoryUpgradeSheet[i];
 battle
            upgradeDatas.Add(item.upgradeID, item);

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
 battle

    public static EnemyData GetEnemyData(string enemyID)
    {
        if (Instance.enemyDatas.ContainsKey(enemyID))
        {
            return Instance.enemyDatas[enemyID];
        }
        else
        {
            Debug.LogError($"�߸��� enemyID �Դϴ�: {enemyID}");
            return null;
        }
    }


    public static MemoryUpgradeData GetUpgradeData(string upgradeID)
    {
        if (Instance.enemyDatas.ContainsKey(upgradeID))
        {
            return Instance.upgradeDatas[upgradeID];
        }
        else
        {
            Debug.LogError($"�߸��� upgradeID �Դϴ�: {upgradeID}");
            return null;
        }
    }
}
