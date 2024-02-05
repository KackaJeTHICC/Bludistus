using UnityEngine;

public class ObjectPicking : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Players camera
    /// </summary>
    [SerializeField]
    private Camera m_playerCamera = null;

    /// <summary>
    /// Range of the player
    /// </summary>   
    [SerializeField]
    private float m_range = 3f;
    #endregion

    #region Methods
    private void Start()
    {
        if (m_playerCamera == null)
        {
            m_playerCamera = transform.GetChild(0).GetComponent<Camera>();
        }
    }

    /// <summary>
    /// Checks if there is pickable object
    /// </summary>
    private void Update()
    {
        RaycastHit hitInfo;
        Vector3 rayOrigin = m_playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        if (Physics.Raycast(rayOrigin, m_playerCamera.transform.forward, out hitInfo, m_range))
        {
            if (hitInfo.collider.CompareTag("Pickable"))
            {
                if (!Input.GetKeyDown(KeyCode.E))    //TODO new input system
                {
                    return;
                }
                MonoBehaviour[] scripts = hitInfo.collider.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour s in scripts)
                {
                    if (s is Pickable)
                    {
                        Pickable p = s as Pickable;
                        p.PickUp();
                        break;
                    }
                }
            }
        }
    }
    #endregion
}