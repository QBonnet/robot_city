using UnityEngine;
using Mirror;
using System.Collections;

public class Player : NetworkBehaviour
{
    [SerializeField] private float m_maxHealth = 100f;
    [SyncVar] private float m_currentHealth;
    [SyncVar] public string m_username = "Unknow";

    public int m_kills;
    public int m_deaths;

    // Stocker les composants à désactiver quand mort
    [SerializeField] private Behaviour[] m_disableOnDeath;
    private bool[] m_wasEnabledOnStart;

    [SerializeField] private GameObject m_deathEffect;
    [SerializeField] private GameObject[] m_disableGameObjectOnDeath;

    [SerializeField] private AudioClip m_hitSound;
    [SerializeField] private AudioClip m_destorySound;
    [SerializeField] private AudioClip m_pickUpSound;

    [SyncVar] private bool m_isDead = false;
    public bool isDead
    {
        get { return m_isDead;  }
        protected set { m_isDead = value;  }
    }

    private bool m_firstSetup = true;

    public void Setup()
    {
        CmdBroadcastNewPlayerSetup();
    }

    [Command(requiresAuthority = false)]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (m_firstSetup)
        {
            m_wasEnabledOnStart = new bool[m_disableOnDeath.Length];
            for (int i = 0; i < m_disableOnDeath.Length; i++)
            {
                m_wasEnabledOnStart[i] = m_disableOnDeath[i].enabled;
            }
            m_firstSetup = false;
        }
        

        SetDefaults();
    }

    public void SetDefaults()
    {
        isDead = false;
        m_currentHealth = m_maxHealth;

        for (int i = 0; i < m_disableOnDeath.Length; i++)
        {
            m_disableOnDeath[i].enabled = m_wasEnabledOnStart[i];
        }
        for (int i = 0; i < m_disableGameObjectOnDeath.Length; i++)
        {
            m_disableGameObjectOnDeath[i].SetActive(true);
        }

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }

    [ClientRpc] public void RpcHeal()
    {
        m_currentHealth = m_maxHealth;
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(m_pickUpSound);
    }

    [ClientRpc]
    public void RpcTakeDamage(float damage, string sourceID)
    {
        if (isDead)
        {
            return;
        }

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(m_hitSound);

        m_currentHealth -= damage;

        if (m_currentHealth <= 0)
        {
            audioSource.PlayOneShot(m_destorySound);
            Die(sourceID);
        }
    }

    private void Die(string sourceID)
    {
        isDead = true;

        // récupérer le joueur qui a tiré et tué pour TabMenu
        Player sourcePlayer = GameManager.GetPlayer(sourceID);
        if (sourcePlayer != null)
        {
            sourcePlayer.m_kills++;
            GameManager.m_instance.m_onPlayerKilledCallback.Invoke(m_username, sourcePlayer.m_username);
        }
        m_deaths++;

        for (int i = 0; i < m_disableOnDeath.Length; i++)
        {
            m_disableOnDeath[i].enabled = false;
        }
        for (int i = 0; i < m_disableGameObjectOnDeath.Length; i++)
        {
            m_disableGameObjectOnDeath[i].SetActive(false);
        }
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        //Jouer l'effet de l'explosion
        GameObject gfxIns = Instantiate(m_deathEffect, transform.position, Quaternion.identity);
        Destroy(gfxIns, 3f);

        StartCoroutine(Respawn());
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.m_instance.m_matchSettings.m_respawnTimer);
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        Setup();
    }

    public float GetHealthPct()
    {
        return (float)m_currentHealth / m_maxHealth;
    }
}
