using Cysharp.Threading.Tasks;
using System;
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
    public Dictionary<Type, Dictionary<string, IGameData>> datas = new();

    public Dictionary<string, EnemyData> enemyDatas = new();
    public Dictionary<string, MemoryUpgradeData> upgradeDatas = new();
    public static bool IsReady { get; private set; }

    void Start()
    {
        GetGoogleData().Forget();
    }

    async UniTaskVoid GetGoogleData()
    {
        var json = await LoadDataGoogleSheet(DataSheetURLHolder.DATA_SHEET_URL);

        var enemyTask = UniTask.RunOnThreadPool(() =>
        {
            var enemySheet = JsonUtility.FromJson<EnemyDataSheet>(json);
            var enemyDatas = new Dictionary<string, IGameData>();   
            for (int i = 0; i < enemySheet.enemyDataSheet.Length; i++)
            {
                var item = enemySheet.enemyDataSheet[i];
                enemyDatas.Add(item.enemyID, item);
            }
            datas.Add(typeof(EnemyData), enemyDatas);
        });

        /*
        
        var memoryTask = UniTask.RunOnThreadPool(() =>
        {
            var memoryDataSheet = JsonUtility.FromJson<MemoryUpgradeDataSheet>(json);
            for (int i = 0; i < memoryDataSheet.memoryUpgradeSheet.Length; i++)
            {
                var item = memoryDataSheet.memoryUpgradeSheet[i];
                upgradeDatas.Add(item.upgradeID, item);
            }
        });
        */
        await UniTask.WhenAll(enemyTask); //, memoryTask);

        print("DataManager - Ready");
        IsReady = true;
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

    public static EnemyData GetEnemyData(string enemyID)
    {
        if (Instance.enemyDatas.TryGetValue(enemyID, out var data))
        {
            return data;
        }
        else
        {
            Debug.LogError($"잘못된 enemyID 입니다: {enemyID}");
            return null;
        }
    }

    public static MemoryUpgradeData GetUpgradeData(string upgradeID)
    {
        if (Instance.upgradeDatas.TryGetValue(upgradeID, out var data))
        {
            return data;
        }
        else
        {
            Debug.LogError($"잘못된 upgradeID 입니다: {upgradeID}");
            return null;
        }
    }

    public static T GetData<T>(string id) where T : IGameData
    {
        if (Instance.IsUnityNull()) 
            return default(T);
        if (Instance.datas.IsUnityNull()) 
            return default(T);
        if (!Instance.datas.TryGetValue(typeof(T), out var targetDic))
            return default(T);
        else
        {
            targetDic.TryGetValue(id, out var data);
            return (T)data;
        }

    }
}
