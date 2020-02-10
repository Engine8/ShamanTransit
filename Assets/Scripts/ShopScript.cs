using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public Text MoneyText;
    public GameObject ItemsHolder;


    //-------------debug -----------
    private struct Item
    {
        public int index;
        public int money;
    }

    private Item[] Items;

    //------------------------------

    public Animator manyMoneyAnimation;

    private void Start()
    {
        //------------debug----------
        Items = new Item[5];
        for (int i = 0; i < 5; ++i)
        {
            Items[i].index = i;
            Items[i].money = i * 100 + 100;

            Transform itemUiTransform = ItemsHolder.transform.Find($"Item {i}").transform;
            //set name
            itemUiTransform.Find("Name").GetComponent<Text>().text = $"Item {i}";
            //set price
            Button buyButton = itemUiTransform.Find("Buy").GetComponent<Button>();
            buyButton.transform.Find("Text").GetComponent<Text>().text = Items[i].money.ToString();

            //check if player already ought this item, then disable button
            if (PlayerDataController.Instance.Data.PurchaseIndexes.IndexOf(i) != -1)
                buyButton.interactable = false;
        }
        //---------------------------

        MoneyText.text = PlayerDataController.Instance.Data.Money.ToString();
    }

    public void PurchaseItem(int index)
    {
        Debug.Log($"Purchase items. Input index: {index}");
        if (Items[index].money <= PlayerDataController.Instance.Data.Money)
        {
            Debug.Log($"Purchase item { Items[index].index}");
            //change and write data
            PlayerDataController.Instance.AddMoney(-Items[index].money);
            PlayerDataController.Instance.ItemPurchased(Items[index].index);
            PlayerDataController.Instance.WriteData();
            //disable button
            ItemsHolder.transform.Find($"Item {Items[index].index}").Find("Buy").GetComponent<Button>().interactable = false;
            //set new money text
            MoneyText.text = PlayerDataController.Instance.Data.Money.ToString();
        }
        else
        {
            Debug.Log($"Play animation: Item { Items[index].index}");
            manyMoneyAnimation.SetTrigger($"Item {Items[index].index}");
        }
    }

    public void Update()
    {
        //set new money text
        MoneyText.text = PlayerDataController.Instance.Data.Money.ToString();
        //also maybe need to update purchases
        
    }

}
