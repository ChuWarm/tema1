<<<<<<< Updated upstream
<<<<<<< Updated upstream
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


=======
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

    void Start()
    {
        GetGoogleData().Forget();
    }

    async UniTaskVoid GetGoogleData()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
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
=======
=======
>>>>>>> Stashed changes
        var json = await LoadDataGoogleSheet(DataSheetURLHolder.DATA_SHEET_URL);

        print(json);
        EnemyDataSheet enemyDataSheet = JsonUtility.FromJson<EnemyDataSheet>(json);

        for (int i = 0; i < enemyDataSheet.enemyDataSheet.Length; i++)
        {
            var enemyData = enemyDataSheet.enemyDataSheet[i];

            enemyDatas.Add(enemyData.enemyID, enemyData);
            enemyDataList.Add(enemyData);
        }

        var memoryDataSheet = JsonUtility.FromJson<MemoryUpgradeDataSheet>(json);

        for (int i = 0; i < memoryDataSheet.memoryUpgradeSheet.Length; i++)
        {
            var memoryUpgradeData = memoryDataSheet.memoryUpgradeSheet[i];

            memoryUpgradeDataList.Add(memoryUpgradeData);
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes


    public EnemyData GetEnemyData(string enemyID)
    {
        if(enemyDatas.ContainsKey(enemyID))
            return enemyDatas[enemyID];
        else return null;
    }
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
}
