using UnityEngine;

[CreateAssetMenu(fileName = "weaponData", menuName = "SO/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string m_name = "Basic Gun";
    public float m_damage = 10f;
    public float m_range = 100f;
    public int m_magazineSize = 5;
    public float m_fireRate = 0f;
    public float m_reloadTime = 1f;
    public GameObject m_graphics;
    public AudioClip m_shootSound;
    public AudioClip m_reloadSound;
}
