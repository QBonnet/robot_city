using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private GameObject m_playerScoreboardItem;
    [SerializeField] private Transform m_playerScoreboardList;

    private void OnEnable()
    {
        // récupérer tousles joueurs dans une array et créer un affichage pour chaque
        Player[] players = GameManager.GetAllPlayers();

        foreach (Player player in players)
        {
            GameObject item = Instantiate(m_playerScoreboardItem, m_playerScoreboardList);
            PlayerScoreboardItem psItem = item.GetComponent<PlayerScoreboardItem>();
            if (psItem != null)
            {
                psItem.Setup(player);
            }
        }
    }

    private void OnDisable()
    {
        // vider la lister
        foreach (Transform child in m_playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }
}
