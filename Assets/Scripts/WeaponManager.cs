using System.Collections;
using UnityEngine;
using Mirror;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private WeaponData m_primaryWeapon;
    [SerializeField] private WeaponData m_secondaryWeapon;

    [SerializeField] private WeaponData m_pistol;
    [SerializeField] private WeaponData m_submachineGun;
    [SerializeField] private WeaponData m_heavy;
    [SerializeField] private WeaponData m_sniper;

    [SerializeField] private string m_weaponLayerName = "Weapon";
    [SerializeField] private Transform m_weaponHolder;

    public WeaponData m_currentWeapon;
    private WeaponGraphics m_currentGraphics;
    private GameObject m_weaponInstance;
    private bool m_primaryWeaponActive = true;
    private bool m_canChangeWeapon = true;

    [HideInInspector] public int m_currentMagazineSize;

    public bool m_isReloading = false;

    private void Start()
    {
        if (UserAccountManager.m_instance.m_weaponName == "Submachine Gun")
        {
            EquipWeapon(m_submachineGun);
            m_primaryWeapon = m_submachineGun;
            m_secondaryWeapon = m_pistol;
        }
        else if (UserAccountManager.m_instance.m_weaponName == "Heavy")
        {
            EquipWeapon(m_heavy);
            m_primaryWeapon = m_heavy;
            m_secondaryWeapon = m_pistol;
        }
        else if (UserAccountManager.m_instance.m_weaponName == "Sniper")
        {
            EquipWeapon(m_sniper);
            m_primaryWeapon = m_sniper;
            m_secondaryWeapon = m_pistol;
        }
        //EquipWeapon(m_primaryWeapon);
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && m_canChangeWeapon)
        {
            StartCoroutine(WaitBetweenWeaponsChange());
            Destroy(m_weaponInstance);

            if (m_primaryWeaponActive)
            {
                EquipWeapon(m_secondaryWeapon);
                m_primaryWeaponActive = false;
            }
            else
            {
                EquipWeapon(m_primaryWeapon);
                m_primaryWeaponActive = true;
            }
            
        }
    }

    private IEnumerator WaitBetweenWeaponsChange()
    {
        m_canChangeWeapon = false;
        yield return new WaitForSeconds(1f);
        m_canChangeWeapon = true;
    }

    public WeaponData GetCurrentWeapon()
    {
        return m_currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return m_currentGraphics;
    }

    private void EquipWeapon(WeaponData weapon)
    {
        m_currentWeapon = weapon;
        m_currentMagazineSize = weapon.m_magazineSize;
        m_weaponInstance = Instantiate(weapon.m_graphics, m_weaponHolder.position, m_weaponHolder.rotation);
        m_weaponInstance.transform.SetParent(m_weaponHolder);

        m_currentGraphics = m_weaponInstance.GetComponent<WeaponGraphics>();
        if (m_currentGraphics == null)
        {
            Debug.LogError("No weapon graphics");
        }

        if (isLocalPlayer)
        {
            Utils.SetLayerRecursively(m_weaponInstance, LayerMask.NameToLayer(m_weaponLayerName));
        }
    }

    public IEnumerator Reload()
    {
        if (m_isReloading)
        {
            yield break;
        }
        m_isReloading = true;
        CmdOnReload();
        yield return new WaitForSeconds(m_currentWeapon.m_reloadTime);
        m_currentMagazineSize = m_currentWeapon.m_magazineSize;

        m_isReloading = false;
    }

    public void SniperScoop()
    {
        CmdOnSniperScope();
    }

    [Command] private void CmdOnSniperScope()
    {
        RpcOnSniperScope();
    }

    [ClientRpc] private void RpcOnSniperScope()
    {
        Animator animator = m_currentGraphics.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Scope");
        }
    }

    [Command]
    private void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    private void RpcOnReload()
    {
        Animator animator = m_currentGraphics.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Reload");
        }
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(m_currentWeapon.m_reloadSound);
    }
}
