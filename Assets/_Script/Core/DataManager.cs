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
<<<<<<< HEAD
    public static bool IsReady { get; private set; }
=======
>>>>>>> 86c9093f55c3b781a43b05554e36499b4dd50bb9

    void Start()
    {
        GetGoogleData().Forget();
    }

    async UniTaskVoid GetGoogleData()
    {
        try
        {
            var json = await LoadDataGoogleSheet(DataSheetURLHolder.DATA_SHEET_URL);

            var enemyTask = await UniTask.RunOnThreadPool(() =>
            {
                var enemySheet = JsonUtility.FromJson<EnemyDataSheet>(json);
                var enemyDatas = new Dictionary<string, IGameData>();
                for (int i = 0; i < enemySheet.enemyDataSheet.Length; i++)
                {
                    var item = enemySheet.enemyDataSheet[i];
                    enemyDatas.Add(item.enemyID, item);
                }
                return enemyDatas;
            });

            var memoryTask = await UniTask.RunOnThreadPool(() =>
            {
                var memoryDataSheet = JsonUtility.FromJson<MemoryUpgradeDataSheet>(json);
                var upgDatas = new Dictionary<string, IGameData>();
                for (int i = 0; i < memoryDataSheet.memoryUpgradeSheet.Length; i++)
                {
                    var item = memoryDataSheet.memoryUpgradeSheet[i];
                    upgDatas.Add(item.upgradeID, item);
                }

                return upgDatas;
            });


            await UniTask.SwitchToMainThread();

            datas.Add(typeof(EnemyData), enemyTask);
            datas.Add(typeof(MemoryUpgradeData), memoryTask);

        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
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
<<<<<<< HEAD
            Debug.LogError($"Àß¸øµÈ enemyID ÀÔ´Ï´Ù: {enemyID}");
=======
            Debug.LogError($"ìž˜ëª»ëœ enemyID ìž…ë‹ˆë‹¤: {enemyID}");
>>>>>>> 86c9093f55c3b781a43b05554e36499b4dd50bb9
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
<<<<<<< HEAD
            Debug.LogError($"Àß¸øµÈ upgradeID ÀÔ´Ï´Ù: {upgradeID}");
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
=======
            Debug.LogError($"ìž˜ëª»ëœ enemyID ìž…ë‹ˆë‹¤: {upgradeID}");
            return null;
        }
    }
}
>>>>>>> 86c9093f55c3b781a43b05554e36499b4dd50bb9
