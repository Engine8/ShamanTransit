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
    //list of bought items, string - item name, int - count
    public Dictionary<string, int> PurchaseIndexes;

    public PlayerData(int money = 100000, int curStage = 0, int curLevel = 0, Dictionary<string, int> purchaseIndexes = null)
    {
        Money = money;
        CurrentStage = curStage;
        CurrentLevel = curLevel;
        if (purchaseIndexes == null)
            PurchaseIndexes = new Dictionary<string, int>();
        else
            PurchaseIndexes = purchaseIndexes;
    }

}

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
        Debug.Log(Application.persistentDataPath + "/data.dat");
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

    public void ItemPurchased(string itemIndex)
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

    public int HasItem(string itemIndex)
    {
        Dictionary<string, int> dict = Data.PurchaseIndexes;

        bool ok = Data.PurchaseIndexes.ContainsKey(itemIndex);
        if (ok && Data.PurchaseIndexes[itemIndex] > 0)
            return Data.PurchaseIndexes[itemIndex];
        return 0;
    }

    public void UseItem(string itemIndex)
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
            Debug.Log("Save was written");
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
                Debug.Log("Save was loaded");
            }
        }
        else
        {
            Data = new PlayerData();
            WriteData();
            Debug.Log("New save file was created");
        }
    }
}
