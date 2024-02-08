using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Monster audio source
    /// </summary> 
    [SerializeField]
    private AudioSource m_audioSource = null;

    /// <summary>
    /// Monster NavMeshAgent
    /// </summary> 
    [SerializeField]
    private NavMeshAgent m_navMeshAgent = null;

    /// <summary>
    /// Player transform
    /// </summary>
    private Transform m_player = null;

    /// <summary>
    /// Gameover screen
    /// </summary>
    private GameObject m_gameOverScreen;

    /// <summary>
    /// Is the player dead?
    /// </summary>
    private bool m_isDead = false;

    /// <summary>
    /// Sound max distance
    /// </summary>    
    [SerializeField]
    private float m_maxDistance = 0.4f;

    /// <summary>
    /// Audio clip minimum speed
    /// </summary>   
    [SerializeField]
    private float m_minSpeed = 0.1f;

    /// <summary>     
    /// Audio clip maximum speed
    /// </summary>   
    [SerializeField]
    private float m_maxSpeed = 1.2f;

    /// <summary>
    /// Distance from player
    /// </summary>
    private float m_distance = 0f;

    /// <summary>    
    /// Normalized distance from player
    /// </summary>
    private float m_normalizedDistance = 0f;

    /// <summary>
    /// Audio clip speed
    /// </summary>
    private float m_clipSpeed = 0f;
    #endregion

    #region Methods
    private void Start()
    {
        m_player = GameObject.Find("PlayerCapsule").transform;

        //these should never occur
        if (m_audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned!");
            m_audioSource = GetComponent<AudioSource>();
        }
        if (m_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is not assigned!");
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }
    }

    private void Update()
    {
        //audio stuff
        m_distance = Mathf.Clamp(Vector3.Distance(transform.position, m_player.position) - 10f, 0.1f, 50f);
        m_normalizedDistance = Mathf.Clamp01(m_distance / m_maxDistance);
        m_clipSpeed = Mathf.Lerp(m_maxSpeed, m_minSpeed, m_normalizedDistance);
        m_audioSource.pitch = m_clipSpeed;

        //ai stuff
        m_navMeshAgent.destination = m_player.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !m_isDead)    //triggers gameover, if player is caught
        {
            GameOver();
        }
    }

    /// <summary>
    /// Ends the game
    /// </summary>   
    /// <param name="other">Player gameobject</param>
    private void GameOver()
    {
        m_isDead = true;
        m_player.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //spawns gameover screen, finds respawn button and disables it if needed
        m_gameOverScreen = Instantiate(Resources.Load("Prefabs/GameOverScreen"), new Vector3(0f, 0, 0), Quaternion.Euler(0f, -0f, 0f)) as GameObject;
        Button b = m_gameOverScreen.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>();
        b.onClick.AddListener(RespawnPlayer);
        if (!GameManager.instance.Difficulty().HasFlag(DifficultySettigns.respawn))
        {
            b.interactable = false;
        }
    }

    /// <summary>
    /// Respawns player
    /// </summary>
    public void RespawnPlayer()
    {
        m_isDead = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_player.transform.position = LevelStart.instance.PlayerStartLocation();
        m_player.gameObject.SetActive(true);
        Destroy(m_gameOverScreen);
    }
    #endregion
}