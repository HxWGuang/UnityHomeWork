using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;      

    
    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    protected float m_OriginalPitch;
    private TankHealth _health;

    #region sys callback
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        _health = GetComponent<TankHealth>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
        
        CollisionListener.onCollisionEnter.AddListener(StartCollision);
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
        
        CollisionListener.onCollisionEnter.RemoveListener(StartCollision);
    }

    
    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }
    

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue     = Input.GetAxis(m_TurnAxisName);

    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();

        EngineAudio();
    }
    #endregion

    #region engine audio ctrl
    protected virtual void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.volume = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.volume = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }
    #endregion

    #region tank move & turn ctrl
    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        // var movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        // m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        m_Rigidbody.velocity = transform.forward * m_MovementInputValue * m_Speed;
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        var turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRot = Quaternion.Euler(0, turn, 0);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRot);
    }
    #endregion

    #region collision check
    
    public float m_MaxCollideForce = 600f;
    public float m_MaxDamage = 10;
    public float CalculateColldieDamage(float force)
    {
        force = Mathf.Clamp(force, 0, m_MaxCollideForce);
        var rel = force / m_MaxCollideForce;
        var damage = m_MaxDamage * rel;
    
        return damage;
    }

    public void StartCollision(GameObject obj, Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground")) return;
        
        var force = (col.impulse / Time.fixedDeltaTime).magnitude;

        var damage = CalculateColldieDamage(force);

        _health.TakeDamage(damage);
        var otherHealth = col.gameObject.GetComponent<TankHealth>();
        if (null != otherHealth)
        {
            // Debug.Log($"othername = {otherHealth.gameObject.name}");
            otherHealth.TakeDamage(m_MaxDamage * 1.2f);
        }
    }
    
    #endregion
}