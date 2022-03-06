using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PauseMenu : NetworkBehaviour
{
    public static bool m_isOn = false;

    private NetworkManager m_networkManager;

    private void Start()
    {
        m_networkManager = NetworkManager.singleton;
    }

    public void QuitButton()
    {
        if (isClientOnly)
        {
            m_networkManager.StopClient();
        }
        else
        {
            m_networkManager.StopHost();
        }
    }
}
