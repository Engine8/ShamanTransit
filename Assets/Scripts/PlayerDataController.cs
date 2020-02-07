using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class PlayerDataController
{
    [NonSerialized]
    public static PlayerDataController Instance;

    public int Money;
    public int CurrentStage;
    public int CurrentLevel;
    public List<int> PurchaseIndexes;

    public PlayerDataController(int money, int curStage, int curLevel, List<int> purchaseIndexes)
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Instance.Money = money;
        Instance.CurrentStage = curStage;
        Instance.CurrentLevel = curLevel;
        Instance.PurchaseIndexes = purchaseIndexes;
    }

    public void AddMoney(int value)
    {
        Money += value;
    }

    public void ItemPurchased(int itemIndex)
    {
        PurchaseIndexes.Add(itemIndex);
    }

    public void WriteData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        // получаем поток, куда будем записывать сериализованный объект
        using (FileStream fs = new FileStream(Application.persistentDataPath + "/data.dat", FileMode.OpenOrCreate))
        {
            formatter.Serialize(fs, this);
            Debug.Log("Save was written");
        }
    }

    public void LoadData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fs = new FileStream(Application.persistentDataPath + "/data.dat", FileMode.OpenOrCreate))
        {
            PlayerDataController newData = (PlayerDataController)formatter.Deserialize(fs);
            Debug.Log("Save was loaded");
        }
    }
}
