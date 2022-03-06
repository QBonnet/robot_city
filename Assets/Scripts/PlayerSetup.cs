using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private Behaviour[] m_componentsToDisable;
    [SerializeField] private string m_remoteLayerName = "RemotePlayer";
    [SerializeField] private string m_dontDrawLayerName = "DontDraw";
    [SerializeField] private GameObject m_playerGraphics;
    [SerializeField] private GameObject m_playerUINameAndHealth;
    [SerializeField] private GameObject m_playerUIPrefab;
    
    private GameObject m_playerUIInstance;
    private Camera m_sceneCamera;

    private void Start()
    {
        // Pour chaque autre joueur 
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            // Changement de la caméra global vers la caméra fps du joueur
            m_sceneCamera = Camera.main;
            if (m_sceneCamera != null)
            {
                m_sceneCamera.gameObject.SetActive(false);
            }
            // Désactiver les grahpismes du joueur dans sa caméra
            Utils.SetLayerRecursively(m_playerGraphics, LayerMask.NameToLayer(m_dontDrawLayerName));
            Utils.SetLayerRecursively(m_playerUINameAndHealth, LayerMask.NameToLayer(m_dontDrawLayerName));

            // Afficher l'UI pour le joueur
            m_playerUIInstance = Instantiate(m_playerUIPrefab);
            // Configuration du UI
            PlayerUI ui = m_playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
            {
                Debug.LogError("ui error PlayerUI");
            }
            else
            {
                ui.SetPlayer(GetComponent<Player>());
            }
            GetComponent<Player>().Setup();

            string username = UserAccountManager.m_loggedInUsername;
            CmdSetUsername(transform.name, username);
        }
    }

    [Command]
    private void CmdSetUsername(string playerID, string username)
    {
        Player player = GameManager.GetPlayer(playerID);
        if (player != null)
        {
            player.m_username = username;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();
        GameManager.RegisterPlayer(netID, player);
    }


    private void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(m_remoteLayerName);
    }

    private void DisableComponents()
    {
        // On désactive leurs composants
        for (int i = 0; i < m_componentsToDisable.Length; i++)
        {
            m_componentsToDisable[i].enabled = false;
        }
    }

    // Quand le joueur se déconnecte
    private void OnDisable()
    {
        Destroy(m_playerUIInstance);
        if (m_sceneCamera != null)
        {
            m_sceneCamera.gameObject.SetActive(true);
        }
        GameManager.UnregisterPlayer(transform.name);
    }
}
