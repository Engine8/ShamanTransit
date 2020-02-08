﻿using System.Collections;
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
    public List<int> PurchaseIndexes;

    public PlayerData(int money = 500, int curStage = 1, int curLevel = 1, List<int> purchaseIndexes = null)
    {
        Money = money;
        CurrentStage = curStage;
        CurrentLevel = curLevel;
        PurchaseIndexes = purchaseIndexes;
    }

}

public class PlayerDataController
{
    
    private static PlayerDataController _instance;
    public PlayerDataController Instance
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
        Data.PurchaseIndexes.Add(itemIndex);
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
        using (FileStream fs = new FileStream(_filePath, FileMode.Open))
        {
            Data = (PlayerData)formatter.Deserialize(fs);
            Debug.Log("Save was loaded");
        }
    }
}
