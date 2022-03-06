using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] m_meshRenderers;
    [SerializeField] private Collider m_collider;
    [SerializeField] private float m_respawnTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (MeshRenderer meshR in m_meshRenderers)
            {
                meshR.enabled = false;
            }
            m_collider.enabled = false;
            StartCoroutine(RespawnPickUp());

            if (gameObject.CompareTag("Heart"))
            {
                Player player = GameManager.GetPlayer(other.name);
                player.RpcHeal();
            }
            else if (gameObject.CompareTag("FuelPack"))
            {
                PlayerControl playerControl = other.GetComponent<PlayerControl>();
                playerControl.RpcRegenFullFuel();
            }
        }
    }

    public IEnumerator RespawnPickUp()
    {
        yield return new WaitForSeconds(m_respawnTime);
        foreach (MeshRenderer meshR in m_meshRenderers)
        {
            meshR.enabled = true;
        }
        m_collider.enabled = true;
    }
}
