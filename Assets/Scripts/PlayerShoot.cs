using UnityEngine;
using Mirror;
using UnityEngine.UI;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private WeaponData m_currentWeapon;
    private WeaponManager m_weaponManager;
    
    [SerializeField] private Camera m_camera;
    [SerializeField] private LayerMask m_layerMask;


    void Start()
    {
        if (m_camera == null)
        {
            Debug.LogError("No camera in PlayerShoot");
            this.enabled = false;
        }
        m_weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        // Récupérer l'arme actuel
        m_currentWeapon = m_weaponManager.GetCurrentWeapon(); 

        if (PauseMenu.m_isOn)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && m_weaponManager.m_currentMagazineSize < m_currentWeapon.m_magazineSize)
        {
            StartCoroutine(m_weaponManager.Reload());
            return;
        }
        
        // Semi automatique
        if (m_currentWeapon.m_fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        // Arme automatique
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f/m_currentWeapon.m_fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }

        // Viser codée en dur
        if (Input.GetButton("Fire2"))
        {
            if (m_weaponManager.m_currentWeapon.m_name == "Sniper")
            {
                PlayerUI.m_instance.SetSniperScope(true);
                m_camera.fieldOfView = 20.0f;
            }
            else
            {
                m_camera.fieldOfView = 30.0f;
            }
            m_weaponManager.SniperScoop();
        }
        else if (!Input.GetButton("Fire2"))
        {
            m_camera.fieldOfView = 60.0f;
            PlayerUI.m_instance.SetSniperScope(false);

        }
    }

    [Command]
    private void CmdOnHit(Vector3 pos, Vector3 normal)
    {
        RpcDoHitEffects(pos, normal);
    }

    [ClientRpc]
    private void RpcDoHitEffects(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect = Instantiate(m_weaponManager.GetCurrentGraphics().m_hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    // Foncrtion appelé sur le serveur quand noter joueur tir (préviens le serveur)
    [Command]
    private void CmdOnShoot()
    {
        RpcDoShootsEffects();
    }

    // Fait apparatitre les effets de tir chez tous le monde
    [ClientRpc]
    private void RpcDoShootsEffects()
    {
        m_weaponManager.GetCurrentGraphics().m_muzzleFlash.Play();
        m_weaponManager.GetCurrentGraphics().PlayLaser();

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(m_currentWeapon.m_shootSound);
    }

    [Client]
    private void Shoot()
    {
        // Eviter que tous les joueurs Shoot si quelqu'un clique
        if (!isLocalPlayer || m_weaponManager.m_isReloading)
        {
            return;
        }

        // Si on a pas de balle return, sinon on tire et on perd une balle
        if (m_weaponManager.m_currentMagazineSize <= 0)
        {
            StartCoroutine(m_weaponManager.Reload());
            return;
        }
        m_weaponManager.m_currentMagazineSize--;

        CmdOnShoot();

        RaycastHit hit;

        if (Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out hit, m_currentWeapon.m_range, m_layerMask))
        {
            if (hit.collider.tag == "Player")
            {
                CmdPlayerShot(hit.collider.name, m_currentWeapon.m_damage, transform.name);
            }
            CmdOnHit(hit.point, hit.normal);
        }

        // Recahrger si plus de balles après le tir
        if (m_weaponManager.m_currentMagazineSize == 0)
        {
            StartCoroutine(m_weaponManager.Reload());
            return;
        }
    }


    [Command]
    private void CmdPlayerShot(string playerId, float damage, string sourceID)
    {
        Debug.Log(playerId + " a été tocuhé.");

        // Stocké le joueur qui a été touché & lui ingliger des dégats
        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage, sourceID);
    }
}
