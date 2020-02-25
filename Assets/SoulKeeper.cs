using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulKeeper : MonoBehaviour
{
    public List<Soul> SoulList;

    private CircleCollider2D _keeperCollider;

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
}
