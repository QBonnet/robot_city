using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserAccountManager : MonoBehaviour
{
    public static string m_loggedInUsername;
    public static UserAccountManager m_instance;
    public string m_lobbySceneName = "Lobby";
    
    [SerializeField] private Toggle m_submachineGunToggle;
    [SerializeField] private Toggle m_heavyToggle;
    [SerializeField] private Toggle m_sniperToggle;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_clickSound;
    [SerializeField] private AudioClip m_fightMusic;
    [SerializeField] private AudioClip m_menuMusic;

    [SerializeField] private string m_fightingSceneName = "Scene1";
    [SerializeField] private string m_loginSceneName = "Login";

    private bool m_isMenuing = true;

    public string m_weaponName;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == m_fightingSceneName && m_isMenuing == true)
        {
            m_audioSource.Stop();
            m_isMenuing = false;
            m_audioSource.PlayOneShot(m_fightMusic);
        }
        if (!m_audioSource.isPlaying)
        {
            m_audioSource.PlayOneShot(m_fightMusic);
        }

        if (SceneManager.GetActiveScene().name != m_fightingSceneName && m_isMenuing == false)
        {
            m_audioSource.Stop();
            m_isMenuing = true;
            m_audioSource.PlayOneShot(m_menuMusic);
        }
    }

    public void PlayClickSound()
    {
        m_audioSource.PlayOneShot(m_clickSound);
    }

    public void LogIn(Text username)
    {
        m_loggedInUsername = username.text;

        if (m_submachineGunToggle.isOn)
        {
            m_weaponName = "Submachine Gun";
        }
        else if (m_heavyToggle.isOn)
        {
            m_weaponName = "Heavy";
        }
        else if (m_sniperToggle.isOn)
        {
            m_weaponName = "Sniper";
        }

        SceneManager.LoadScene(m_lobbySceneName);
    }
}
