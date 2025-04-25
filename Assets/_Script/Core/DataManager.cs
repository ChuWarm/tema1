using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using UnityEngine;

public static class DataSheetURLHolder
{
    public static string DATA_SHEET_URL = "https://script.google.com/macros/s/AKfycbyUd5sR4KJbcy8iaSWgNcuPUJbMuK_OH9CYZA3Tli7Qf0BRNr-H8iiSCABHMcupnFnl/exec";

}

public class DataManager : MonoBehaviour
{
    public Dictionary<string, EnemyData> enemyDatas = new();

    public List<EnemyData> enemyDataList = new();

    public List<MemoryUpgradeData> memoryUpgradeDataList = new();

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
            enemyDataList.Add(item);
        }


        var memoryDataSheet = JsonUtility.FromJson<MemoryUpgradeDataSheet>(json);

        for (int i = 0; i < memoryDataSheet.memoryUpgradeSheet.Length; i++)
        {
            var item = memoryDataSheet.memoryUpgradeSheet[i];
            memoryUpgradeDataList.Add(item);
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

    public EnemyData GetEnemyData(string enemyID)
    {
        if(enemyDatas.ContainsKey(enemyID))
            return enemyDatas[enemyID];
        else return null;
    }
}
