using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponGraphics : MonoBehaviour
{
    public ParticleSystem m_muzzleFlash;
    public GameObject m_hitEffectPrefab;
    public LineRenderer m_laserEffect;
    public float m_laserTime = 0.05f;

    public void PlayLaser()
    {
        StartCoroutine(LaserSpawn());
    }

    private IEnumerator LaserSpawn()
    {
        m_laserEffect.enabled = true;
        yield return new WaitForSeconds(m_laserTime);
        m_laserEffect.enabled = false;
    }
}
