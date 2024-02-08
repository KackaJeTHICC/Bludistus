using UnityEngine;

/// <summary>
/// Manages objects pick ups
/// </summary>
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
        if (m_playerCamera == null) //this should never occur and the GetChild() will most likely fail anyway
        {
            m_playerCamera = transform.GetChild(0).GetComponent<Camera>();
        }
    }

    /// <summary>
    /// Checks if there is pickable object
    /// </summary>
    private void Update()
    {
        Vector3 rayOrigin = m_playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(rayOrigin, m_playerCamera.transform.forward, out RaycastHit hitInfo, m_range))
        {
            if (hitInfo.collider.CompareTag("Pickable"))
            {
                if (!Input.GetKeyDown(KeyCode.E))    //New input system should be implemented
                {
                    return;
                }
                MonoBehaviour[] scripts = hitInfo.collider.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour s in scripts)    //we look through all the scripts to find Pickable.cs
                {                                       //this is a workaround, since we can't search for Pickable.cs directly
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