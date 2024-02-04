using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    /// Sound max distance
    /// </summary>
    public float maxDistance = 10f;

    /// <summary>
    /// Audio clip minimum speed
    /// </summary>
    public float minSpeed = 0.5f;

    /// <summary>     
    /// Audio clip maximum speed
    /// </summary>
    public float maxSpeed = 2.0f;

    /// <summary>
    /// Distance from player
    /// </summary>
    private float distance = 0;

    /// <summary>
    /// Audio clip speed
    /// </summary>
    private float speed = 0;

    /// <summary>    
    /// Normalized distance from player
    /// </summary>
    private float normalizedDistance = 0;
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
        //Audio stuff
        distance = Mathf.Clamp(Vector3.Distance(transform.position, m_player.position) - 10f, 0.1f, 50f);
        normalizedDistance = Mathf.Clamp01(distance / maxDistance);
        speed = Mathf.Lerp(maxSpeed, minSpeed, normalizedDistance);
        m_audioSource.pitch = speed;

        //AI stuff
        m_navMeshAgent.destination = m_player.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameOver();
        }
    }

    /// <summary>
    /// Ends the game
    /// </summary>
    private void GameOver()
    {
        print("game over:c");
    }
#endregion
}
