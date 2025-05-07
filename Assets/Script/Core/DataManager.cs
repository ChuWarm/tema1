using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public static class DataSheetURLHolder
{
    public static string DATA_SHEET_URL = "https://script.google.com/macros/s/AKfycbyUd5sR4KJbcy8iaSWgNcuPUJbMuK_OH9CYZA3Tli7Qf0BRNr-H8iiSCABHMcupnFnl/exec";
}

public class DataManager : Singleton<DataManager>
{
    public Dictionary<Type, Dictionary<string, IGameData>> datas = new();

    public static bool IsReady { get; private set; }

    void Start()
    {
        GetGoogleData().Forget();
    }

    async UniTaskVoid GetGoogleData()
    {
        try
        {
            var json = await LoadDataGoogleSheet(DataSheetURLHolder.DATA_SHEET_URL);

            print("Asd");
            var enemySheet = JsonUtility.FromJson<EnemyDataSheet>(json);
            var memoryDataSheet = JsonUtility.FromJson<MemoryUpgradeDataSheet>(json);
            var itemDataSheet = JsonUtility.FromJson<ItemDataSheet>(json);

            print("parse suc");

            var enemyTask = await UniTask.RunOnThreadPool(() =>
            {
                var enemyDatas = new Dictionary<string, IGameData>();
                for (int i = 0; i < enemySheet.enemyDataSheet.Length; i++)
                {
                    var item = enemySheet.enemyDataSheet[i];

                    // ID 유효성 검사 추가
                    if (string.IsNullOrEmpty(item.enemyID))
                    {
                        Debug.LogError($"잘못된 enemyID: 인덱스 {i}");
                        continue;
                    }

                    // 중복 키 체크
                    if (enemyDatas.ContainsKey(item.enemyID))
                    {
                        Debug.LogWarning($"중복 enemyID: {item.enemyID}");
                        continue; // 또는 기존 데이터 덮어쓰기
                    }

                    enemyDatas.Add(item.enemyID, (IGameData)item);
                }
                return enemyDatas;
            });

            var memoryTask = await UniTask.RunOnThreadPool(() =>
            {
                var upgDatas = new Dictionary<string, IGameData>();
                for (int i = 0; i < memoryDataSheet.memoryUpgradeSheet.Length; i++)
                {
                    var item = memoryDataSheet.memoryUpgradeSheet[i];

                    // ID 유효성 검사 추가
                    if (string.IsNullOrEmpty(item.upgradeID))
                    {
                        Debug.LogError($"잘못된 enemyID: 인덱스 {i}");
                        continue;
                    }

                    // 중복 키 체크
                    if (upgDatas.ContainsKey(item.upgradeID))
                    {
                        Debug.LogWarning($"중복 enemyID: {item.upgradeID}");
                        continue; // 또는 기존 데이터 덮어쓰기
                    }

                    upgDatas.Add(item.upgradeID, item);
                }

                return upgDatas;
            });


            var itemTask = await UniTask.RunOnThreadPool(() =>
            {
                var itemDatas = new Dictionary<string, IGameData>();
                for (int i = 0; i < itemDataSheet.itemDataSheet.Length; i++)
                {
                    var item = itemDataSheet.itemDataSheet[i];

                    if (string.IsNullOrEmpty(item.itemID))
                    {
                        Debug.LogError($"잘못된 enemyID: 인덱스 {i}");
                        continue;
                    }
                    if (itemDatas.ContainsKey(item.itemID))
                    {
                        Debug.LogWarning($"중복 enemyID: {item.itemID}");
                        continue; // 또는 기존 데이터 덮어쓰기
                    }

                    itemDatas.Add(item.itemID, item);
                }

                return itemDatas;
            });



            await UniTask.SwitchToMainThread();

            print($"added - enemyData count: {enemyTask.Count}");
            datas.Add(typeof(EnemyData), enemyTask);

            print($"added - memoryData count: {memoryTask.Count}");
            datas.Add(typeof(MemoryUpgradeData), memoryTask);

            print($"added - itemData count: {itemTask.Count}");
            datas.Add(typeof(ItemData), itemTask);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        print("DataManager - Ready    asd");
        IsReady = true;
    }

    async UniTask<string> LoadDataGoogleSheet(string url)
    {
        using (var request = UnityWebRequest.Get(url))
        {
            try
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"요청 오류: {request.error}");
                    return null;
                }

                return request.downloadHandler.text;
            }
            catch (Exception e)
            {
                Debug.LogError($"요청 오류: {e.Message}");
                return null;
            }
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
