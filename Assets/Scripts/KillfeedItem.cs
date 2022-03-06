using UnityEngine;
using UnityEngine.UI;

public class KillfeedItem : MonoBehaviour
{
    [SerializeField] private Text m_text;

    public void Setup(string player, string source)
    {
        m_text.text = "[" + source + "]" + " killed " + "[" + player + "]";
    }
}
