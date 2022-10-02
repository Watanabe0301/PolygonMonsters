using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CsvReader : MonoBehaviour
{
    Dictionary<string, Dictionary<string, string>> Data = new Dictionary<string, Dictionary<string, string>>();


   
    private void Awake()
    {
        int Value = 0;
        TextAsset csv = Resources.Load("CSV/NameData") as TextAsset;
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > 0)
        {
            string line = reader.ReadLine();
            string[] nameList = line.Split(',');
            SetNameDictionary(nameList[Value]);
            //sValue++;
        }
    }

    //各Enemyのステータスを読み込む
    void SetNameDictionary(string name)
    {
        int Value = 0;
        TextAsset csv = Resources.Load("CSV/" + name + "Data") as TextAsset;
        StringReader reader = new StringReader(csv.text);
        List<string[]> lvListString = new List<string[]>();
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            lvListString.Add(line.Split(','));
            SetLvDictionary(Value, lvListString , name);
            Value++;
        }
    }

    //ステータスをDictionaryに登録
    void SetLvDictionary(int csvNumber, List<string[]> lisString , string name)
    {
        Dictionary<string, string> statusData = new Dictionary<string, string>();
        string Lv = name + lisString[csvNumber][0];
        string HP = lisString[csvNumber][1];
        string ATK = lisString[csvNumber][2];
        string SPEED = lisString[csvNumber][3];
        statusData.Add(lisString[0][0], Lv);
        statusData.Add(lisString[0][1], HP);
        statusData.Add(lisString[0][2], ATK);
        statusData.Add(lisString[0][3], SPEED);
        Data.Add(Lv, statusData);
    }
    
    //ステータスを返す
    public Dictionary<string, string> GetMonsterStatusData(string monsterName , int Lv)
    {
        string monsterLv = Lv.ToString("0");
        var MonsterDic = Data[monsterName + monsterLv];
        return MonsterDic;
    }

    
}
