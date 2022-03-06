using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerControl : NetworkBehaviour
{
    [SerializeField] private float m_speed = 7.5f;
    [SerializeField] private float m_mouseSensitivityX = 3.0f;
    [SerializeField] private float m_mouseSensitivityY = 3.0f;
    [SerializeField] private float m_thrusterForce = 1000f;
    [SerializeField] private float m_thrusterFloatBurnSpeed = 0.4f;
    [SerializeField] private float m_thrusterFloatRegenSpeed = 0.3f;
    private float m_thrusterFuelAmount = 1f;
    public float GetThrusterFuelAmount()
    {
        return m_thrusterFuelAmount;
    }
    [Header("Joint Options")]
    [SerializeField] private float m_configurableJointSpring = 20;
    [SerializeField] private float m_configurableJointMaxForce = 50;

    private PlayerMotor m_playerMotor;
    private ConfigurableJoint m_configurableJoint;
    private Animator m_animator;

    [SerializeField] private AudioClip m_pickUpSound;

    private void Start()
    {
        m_playerMotor = GetComponent<PlayerMotor>();
        m_configurableJoint = GetComponent<ConfigurableJoint>();
        m_animator = GetComponent<Animator>();
        SetJointSettings(m_configurableJointSpring);
    }

    private void Update()
    {

        if (PauseMenu.m_isOn)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            m_playerMotor.Move(Vector3.zero);
            m_playerMotor.Rotate(Vector3.zero);
            m_playerMotor.RotateCamera(0f);
            m_playerMotor.ApplyThruster(Vector3.zero);
            return;
        }

        if(Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Gérer position du joueur sur les objets (vole stationnaire)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f))
        {
            m_configurableJoint.targetPosition = new Vector3(0f, -hit.point.y, 0f);
        }
        else
        {
            m_configurableJoint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        // Calculer la vélocité (vitesse) du mouvement du joueur
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 velocity = (moveHorizontal + moveVertical) * m_speed;

        // Ajout des animations Thruster
        m_animator.SetFloat("ForwardVelocity", zMov);

        // Appliquer la vélocité
        m_playerMotor.Move(velocity);


        // Calculer la rotation du joueur en un vector3 (caméra)
        float yRot = Input.GetAxisRaw("Mouse X");
        Vector3 rotation = new Vector3(0, yRot, 0) * m_mouseSensitivityX;
        m_playerMotor.Rotate(rotation);

        // Calculer la rotation X de la caméra en un float (caméra)
        float xRot = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRot* m_mouseSensitivityY;
        m_playerMotor.RotateCamera(cameraRotationX);

        //Calculer la vélocité du jetpack
        Vector3 thrusterVelocity = Vector3.zero;
        // Gestion du JetPack (appliquer thrusterForce)
        if (Input.GetButton("Jump") && m_thrusterFuelAmount > 0)
        {
            m_thrusterFuelAmount -= m_thrusterFloatBurnSpeed * Time.deltaTime;
            if (m_thrusterFuelAmount >= 0.01f)
            {
                thrusterVelocity = Vector3.up * m_thrusterForce;
                // Désactiver la gravité en gros
                SetJointSettings(0.0f);
            }
        }
        else
        {
            m_thrusterFuelAmount += m_thrusterFloatRegenSpeed * Time.deltaTime;
            // Réactiver la gravité
            SetJointSettings(m_configurableJointSpring);
        }
        m_thrusterFuelAmount = Mathf.Clamp(m_thrusterFuelAmount, 0f, 1f);
        // Appliquer la force du jetpack
        m_playerMotor.ApplyThruster(thrusterVelocity);
    }

    private void SetJointSettings(float configurableJointSpring)
    {
        m_configurableJoint.yDrive = new JointDrive { positionSpring = configurableJointSpring, maximumForce = m_configurableJointMaxForce };
    }

    [ClientRpc]
    public void RpcRegenFullFuel()
    {
        m_thrusterFuelAmount = 1f;
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(m_pickUpSound);
    }
}
