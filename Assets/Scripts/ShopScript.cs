using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public Text MoneyText;
    public GameObject ItemsHolder;

    public List<ShopItem> ShopItems;

    public Animator manyMoneyAnimation;

    private void Start()
    {
        foreach (var item in ShopItems)
        {
            item.OnBuyClick.AddListener(PurchaseItem);
        }
        MoneyText.text = PlayerDataController.Instance.Data.Money.ToString();
    }

    public void PurchaseItem(ShopItem item)
    {
        string itemName = item.GetName();
        int cost = item.GetCost();

        if (cost <= PlayerDataController.Instance.Data.Money)
        {
            //change and write data
            PlayerDataController.Instance.AddMoney(-cost);
            PlayerDataController.Instance.ItemPurchased(itemName);
            PlayerDataController.Instance.WriteData();
            //set new money text
            MoneyText.text = PlayerDataController.Instance.Data.Money.ToString();
            //play animation and other do other processing
            item.BuyItem();
        }
        else
            item.AnimateError();
    }
}

