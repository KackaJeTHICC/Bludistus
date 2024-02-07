using System.Collections;
using System.Collections.Generic;
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
    private float maxDistance = 0.4f;

    /// <summary>
    /// Audio clip minimum speed
    /// </summary>   
    [SerializeField]
    private float minSpeed = 0.1f;

    /// <summary>     
    /// Audio clip maximum speed
    /// </summary>   
    [SerializeField]
    private float maxSpeed = 1.2f;

    /// <summary>
    /// Distance from player
    /// </summary>
    private float distance = 0f;

    /// <summary>
    /// Audio clip speed
    /// </summary>
    private float speed = 0f;

    /// <summary>    
    /// Normalized distance from player
    /// </summary>
    private float normalizedDistance = 0f;
    #endregion

    #region Methods
    private void Start()
    {
        m_player = GameObject.Find("PlayerCapsule").transform;

        if (m_audioSource == null)    //this should never occur
        {
            Debug.LogError("AudioSource is not assigned!");
            m_audioSource = GetComponent<AudioSource>();
        }
        if (m_navMeshAgent == null)   //this should never occur
        {
            Debug.LogError("NavMeshAgent is not assigned!");
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }
    }

    private void Update()
    {
        distance = Mathf.Clamp(Vector3.Distance(transform.position, m_player.position) - 10f, 0.1f, 50f);
        normalizedDistance = Mathf.Clamp01(distance / maxDistance);
        speed = Mathf.Lerp(maxSpeed, minSpeed, normalizedDistance);
        m_audioSource.pitch = speed;

        m_navMeshAgent.destination = m_player.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !m_isDead)
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
        m_gameOverScreen = Instantiate(Resources.Load("Prefabs/GameOverScreen"), new Vector3(0f, 0, 0), Quaternion.Euler(0f, -0f, 0f)) as GameObject;
        Button b = m_gameOverScreen.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>();
        b.onClick.AddListener(RespawnPlayer);
        if (!GameManager.instance.Difficulty().HasFlag(DifficultySettigns.respawn))
        {
            b.interactable = false;
        }
    }

    /// <summary>
    /// Respawns play
    /// </summary>
    public void RespawnPlayer()
    {
        m_isDead = false;
        Destroy(m_gameOverScreen);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_player.transform.position = LevelStart.instance.PlayerStartLocation();
        m_player.gameObject.SetActive(true);
    }
    #endregion
}