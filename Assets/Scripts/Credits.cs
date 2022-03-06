using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Credits : MonoBehaviour
{
    [SerializeField] private float m_time;
    [SerializeField] private string m_sceneToLoad;

    private NetworkManager m_networkManager;
    

    private NetworkManagerHUD m_networkManagerHUD;

    private void Awake()
    {
        m_networkManager = FindObjectOfType<NetworkManager>();
        m_networkManagerHUD = m_networkManager.GetComponent<NetworkManagerHUD>();
        m_networkManagerHUD.enabled = false;
    }

    void Update()
    {
        StartCoroutine(PlayCreditsOnce());
    }

    private IEnumerator PlayCreditsOnce()
    {
        yield return new WaitForSeconds(m_time);
        m_networkManagerHUD.enabled = true;
        SceneManager.LoadScene(m_sceneToLoad);
    }
}
