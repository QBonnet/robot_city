using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    [SerializeField] private float m_cameraRotationLimit = 85f;

    private Vector3 m_velocity;
    private Vector3 m_rotation;
    private float m_cameraRotationX = 0f;
    private float m_currentCameraRotationX = 0f;
    private Vector3 m_thrusterVelocity;
    private Rigidbody m_rb;


    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 velocity)
    {
        // R�cup�r� la v�locit� de PlayerController
        m_velocity = velocity;
    }

    public void Rotate(Vector3 rotation)
    {
        // R�cup�r� la rotation du joueur de PlayerController
        m_rotation = rotation;
    }

    public void RotateCamera(float cameraRotationX)
    {
        // R�cup�r� la rotation de la cam�ra de PlayerController
        m_cameraRotationX = cameraRotationX;
    }

    public void ApplyThruster(Vector3 thrusterVelocity)
    {
        m_thrusterVelocity = thrusterVelocity;
    }

    // FixedUpdate == Update pour la physique
    private void FixedUpdate()
    {
        PerformMovement();
        //PerformRotation();
    }

    private void Update()
    {
        PerformRotation();
        //PerformMovement();
    }

    private void PerformMovement()
    {
        // D�placement uniquement si la v�locit� est non nul
        if (m_velocity != Vector3.zero)
        {
            // D�placement, fixedDeltaTime car FixedUpdate & Physique
            m_rb.MovePosition(m_rb.position + m_velocity * Time.fixedDeltaTime);
        }

        // D�placement du jetpack
        if (m_thrusterVelocity != Vector3.zero)
        {
            m_rb.AddForce(m_thrusterVelocity * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    private void PerformRotation()
    {
        // Calcul la rotation de la cam�ra
        m_rb.MoveRotation(m_rb.rotation * Quaternion.Euler(m_rotation));
        m_currentCameraRotationX -= m_cameraRotationX;
        m_currentCameraRotationX = Mathf.Clamp(m_currentCameraRotationX, -m_cameraRotationLimit, m_cameraRotationLimit);
        // Applique la rotation
        m_camera.transform.localEulerAngles = new Vector3(m_currentCameraRotationX, 0f, 0f);
    }
}
