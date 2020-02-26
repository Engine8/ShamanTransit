using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulKeeper : MonoBehaviour
{
    public List<Soul> SoulList;

    private CircleCollider2D _keeperCollider;
    private string _sortingLayerName = "Line2";
    public GameObject SoulPrefab;
    // Start is called before the first frame update
    void Start()
    {
        SoulList = new List<Soul>();
        _keeperCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddSoul()
    {
        GameObject newSoul = Instantiate(SoulPrefab, transform.position, transform.rotation, transform);
        SoulList.Add(newSoul.GetComponent<Soul>());
        SoulList[SoulList.Count - 1].spriteRenderer.sortingLayerName = _sortingLayerName;
    }

    public void DeleteSoul()
    {
        if (SoulList.Count > 0)
        {
            Soul soulToDelete = SoulList[SoulList.Count - 1];
            SoulList.RemoveAt(SoulList.Count - 1);
            Destroy(soulToDelete.gameObject);
        }
    }

    public int GetSoulCount()
    {
        return SoulList.Count;
    }

    public void SetSoulsSortingLayer(string layerName)
    {
        _sortingLayerName = layerName;
        foreach (var soul in SoulList)
        {
            soul.spriteRenderer.sortingLayerName = _sortingLayerName;
        }
    }
}
