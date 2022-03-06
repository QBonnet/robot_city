using UnityEngine;
using UnityEngine.UI;

public class PlayerNameplate : MonoBehaviour
{
    [SerializeField] private Text m_usernameText;
    [SerializeField] private Player m_player;
    [SerializeField] private RectTransform m_healthBarFill;

    void Update()
    {
        m_usernameText.text = m_player.m_username;
        m_healthBarFill.localScale = new Vector3(m_player.GetHealthPct(), 1f, 1f);
    }
}
