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
            ReleaseSoul(soulToDelete);
            soulToDelete.transform.SetParent(null, true);
            //Destroy(soulToDelete.gameObject);
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

    public void ReleaseSoul(Soul soul)
    {
        //define direction
        float angle = Random.Range(60f, 120f);
        float x = Mathf.Cos(angle * Mathf.PI / 180);
        float y = Mathf.Sin(angle * Mathf.PI / 180);
        //Vector3 pointB = 10 * (new Vector3(x, y, 0));
        //Vector3 pointA = soul.transform.position;
        Vector3 dir = new Vector3(x, y, 0).normalized;
        soul.FlyAway(dir);
    }


    public void ReleaseSouls()
    {
        foreach(var soul in SoulList)
        {
            ReleaseSoul(soul);
        }
        SoulList.Clear();
    }
}
