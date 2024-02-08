using UnityEngine;

/// <summary>
/// Manages the flare projectile
/// </summary>
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
        if (m_runtimeParent == null)    //this shouldn't occur
        {
            m_runtimeParent = GameObject.Find("--------Runtime--------").transform;
        }
    }

    private void OnEnable()
    {
        gameObject.transform.SetParent(m_runtimeParent);    //deattaches from player gameobject so the projectile can move freely
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * m_speed); //moves the projectile forwards
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))   //"kills" the monster
        {
            gameObject.transform.SetParent(m_originalParent);   //attaches back to player
            MonsterManager.instance.IsMonster(false);
            Destroy(collision.gameObject);
        }
        gameObject.SetActive(false);
    }
    #endregion
}