using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killfeed : MonoBehaviour
{
    [SerializeField] private GameObject m_killfeedItemPrefab;

    void Start()
    {
        GameManager.m_instance.m_onPlayerKilledCallback += OnKill;
    }

    public void OnKill (string player, string source)
    {
        GameObject go = Instantiate(m_killfeedItemPrefab, transform);
        go.GetComponent<KillfeedItem>().Setup(player, source);
        Destroy(go, 3f);
    }
}
