using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int Money;
    public int CurrentStage;
    public int CurrentLevel;
    //list of bought items, int1 - item id, int2 - count
    public Dictionary<int, int> PurchaseIndexes;

    public PlayerData(int money = 0, int curStage = 0, int curLevel = 0, Dictionary<int, int> purchaseIndexes = null)
    {
        Money = money;
        CurrentStage = curStage;
        CurrentLevel = curLevel;
        if (purchaseIndexes == null)
            PurchaseIndexes = new Dictionary<int, int>();
        else
            PurchaseIndexes = purchaseIndexes;
    }

}

/*
 * 0 - soul magnet/ магнит душ
 * 1 - second life/вторая жизнь
 * 2 - beast skip/пропустить зверей
 * 3 - bears are strange/что-то странное
 */

public class PlayerDataController
{
    
    private static PlayerDataController _instance;
    public static PlayerDataController Instance
    {
        get
        {
            return _instance;
        }
    }

    public PlayerData Data;

    private string _filePath = Application.persistentDataPath + "/data.dat";

    public static void Initialize()
    {
        if (_instance == null)
            _instance = new PlayerDataController();
       //Debug.Log(Application.persistentDataPath + "/data.dat");
        _instance.LoadData();
    }

    private PlayerDataController()
    {
        if (!File.Exists(_filePath))
            Data = new PlayerData();
        else
            LoadData();
    }

    public void AddMoney(int value)
    {
        Data.Money += value;
    }

    public void ItemPurchased(int itemIndex)
    {
        if (Data.PurchaseIndexes.ContainsKey(itemIndex))
        {
            ++Data.PurchaseIndexes[itemIndex];
        }
        else
        {
            Data.PurchaseIndexes.Add(itemIndex, 1);
        }
    }

    public int HasItem(int itemIndex)
    {
        //Dictionary<int, int> dict = Data.PurchaseIndexes;

        bool ok = Data.PurchaseIndexes.ContainsKey(itemIndex);
        if (ok && Data.PurchaseIndexes[itemIndex] > 0)
            return Data.PurchaseIndexes[itemIndex];
        return 0;
    }

    public void UseItem(int itemIndex)
    {
        if (Data.PurchaseIndexes.ContainsKey(itemIndex) && Data.PurchaseIndexes[itemIndex] > 0)
            --Data.PurchaseIndexes[itemIndex];
    }

    public void WriteData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        // получаем поток, куда будем записывать сериализованный объект
        using (FileStream fs = new FileStream(_filePath, FileMode.OpenOrCreate))
        {
            formatter.Serialize(fs, Data);
            //Debug.Log("Save was written");
        }
    }

    public void LoadData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        if (File.Exists(_filePath))
        {
            using (FileStream fs = new FileStream(_filePath, FileMode.Open))
            {
                Data = (PlayerData)formatter.Deserialize(fs);
                //Debug.Log("Save was loaded");
            }
        }
        else
        {
            Data = new PlayerData();
            WriteData();
            //Debug.Log("New save file was created");
        }
    }
}
