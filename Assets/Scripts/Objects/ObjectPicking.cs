using UnityEngine;

public class ObjectPicking : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Players camera
    /// </summary>
    private Camera m_playerCamera = null;

    /// <summary>
    /// Range of the player
    /// </summary>
    private float m_range = 3f;
    #endregion

    #region Methods
    private void Start()
    {
        m_playerCamera = transform.GetComponentInChildren<Camera>();
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
                MonoBehaviour[] scripts = hitInfo.collider.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour s in scripts)
                {
                    if (s is IPickable)
                    {
                        IPickable p = s as IPickable;
                        p.PickUp();
                        break;
                    }
                }
            }
        }
    }
    #endregion
}