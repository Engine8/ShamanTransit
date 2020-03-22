using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    /*
     * -1 - endless
     * 0 - not available
     * >0 - restricted count
     */
    public int PurchaseCountAvailable = -1;

    public int PurchasedCount;
    private Animator _animator;
    private int _cost;

    public UnityEvent<ShopItem> OnBuyClick;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Button button = transform.Find("Buy").GetComponent<Button>();
        button.onClick.AddListener(OnClickResender);
        _cost = int.Parse(button.transform.GetChild(0).GetComponent<Text>().text);

        //check if player already have max count of item or max count not restricted
        int PurchasedCount = PlayerDataController.Instance.HasItem(GetName());
        if (PurchasedCount == PurchaseCountAvailable)
        {
            button.interactable = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void BuyItem()
    {
        ++PurchasedCount;
        if (PurchasedCount == PurchaseCountAvailable)
        {
            transform.Find("Buy").GetComponent<Button>().interactable = false;
        }
    }

    public void AnimateError()
    {
        _animator.Play("ShopItemBuyFailed");
    }

    private void OnClickResender()
    {
        Debug.Log($"Item is null: {this == null}");
        Debug.Log($"OnBUyClick is null: {OnBuyClick == null}");
        OnBuyClick.Invoke(this);
    }

    public string GetName()
    {
        return transform.Find("Name").GetComponent<Text>().text;
    }

    public string GetDescription()
    {
        return transform.Find("Description").GetComponent<Text>().text;
    }

    public int GetCost()
    {
        return _cost;
    }
}
