using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareProjectile : MonoBehaviour
{
    #region Instance
    public static FlareProjectile instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    #region Variables
    /// <summary>
    /// Speed of the projectile
    /// </summary>
    [SerializeField]
    private float m_speed = 1f;

    /// <summary>
    /// Original parent
    /// </summary>
    [SerializeField]
    private Transform m_originalParent = null;

    /// <summary>
    /// Runtime parent
    /// </summary>
    [SerializeField]
    private Transform m_runtimeParent = null;
    #endregion

    #region Methods
    private void Start()
    {
        if (m_runtimeParent == null)
        {
            m_runtimeParent = GameObject.Find("--------Runtime--------").transform;
        }
    }

    private void OnEnable()
    {
        gameObject.transform.SetParent(m_runtimeParent);
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * m_speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameObject.transform.SetParent(m_originalParent);
            Destroy(collision.gameObject);  //TODO udelat aby o tom hra vedela ze chybi enemy
        }
        gameObject.SetActive(false);
    }
    #endregion
}