using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private RectTransform m_thrusterFuelFill;
    [SerializeField] private RectTransform m_healthBarFill;
    [SerializeField] private Text m_ammoText;
    [SerializeField] private GameObject m_pauseMenu;
    [SerializeField] private GameObject m_scoreBoard;
    [SerializeField] private GameObject m_sniperScopeUI;
    [SerializeField] private GameObject m_crosshair;

    private PlayerControl m_playerControl;
    private Player m_player;
    private WeaponManager m_weaponManager;

    public static PlayerUI m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            return;
        }
        Debug.LogError("UI > 1");
    }

    private void Update()
    {
        SetFuelAmount(m_playerControl.GetThrusterFuelAmount());
        SetHealthAmount(m_player.GetHealthPct());
        SetAmmoAmount(m_weaponManager.m_currentMagazineSize);

        // Ouvrir menu pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        // Menu des scores
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_scoreBoard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            m_scoreBoard.SetActive(false);
        }
    }

    private void Start()
    {
        PauseMenu.m_isOn = false;
    }

    public void TogglePauseMenu()
    {
        m_pauseMenu.SetActive(!m_pauseMenu.activeSelf);
        PauseMenu.m_isOn = m_pauseMenu.activeSelf;
    }

    public void SetPlayer(Player player)
    {
        m_player = player;
        m_playerControl = player.GetComponent<PlayerControl>();
        m_weaponManager = player.GetComponent<WeaponManager>();
    }

    private void SetFuelAmount(float amount)
    {
        m_thrusterFuelFill.localScale = new Vector3(1f, amount, 1f);
    }

    private void SetHealthAmount(float amount)
    {
        m_healthBarFill.localScale = new Vector3(1f, amount, 1f);
    }

    private void SetAmmoAmount(int amount)
    {
        m_ammoText.text = amount.ToString();
    }

    public void SetSniperScope(bool scopeBool)
    {
        m_sniperScopeUI.SetActive(scopeBool);
        m_crosshair.SetActive(!scopeBool);
    }
}
