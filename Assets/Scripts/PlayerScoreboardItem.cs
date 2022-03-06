using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreboardItem : MonoBehaviour
{
    [SerializeField] private Text m_usernameText;
    [SerializeField] private Text m_killsText;
    [SerializeField] private Text m_deathsText;

    public void Setup(Player player)
    {
        m_usernameText.text = player.m_username;
        m_killsText.text = "Kills : " + player.m_kills;
        m_deathsText.text = "Deaths : " + player.m_deaths;
    }
}
