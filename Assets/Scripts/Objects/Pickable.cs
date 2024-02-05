using UnityEngine;

public abstract class Pickable : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Spinning speed
    /// </summary>
    private float m_spinSpeed = 10f;

    /// <summary>
    /// Hovering amplitude
    /// </summary>
    private float m_hoverAmplitude = 0.05f;

    /// <summary>
    /// Hovering speed
    /// </summary>
    private float m_hoverSpeed = 0.5f;

    /// <summary>
    /// Initial position of the object
    /// </summary>
    private Vector3 initialPosition;
    #endregion

    #region Methods
    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        Levitate();
    }

    /// <summary>
    /// Logic after picking up an object
    /// </summary>
    public abstract void PickUp();

    /// <summary>
    /// Levitating animation
    /// </summary>
    private protected void Levitate()
    {
        transform.Rotate(Vector3.up, m_spinSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right, m_spinSpeed * Time.deltaTime);
        float hoverOffset = Mathf.Sin(Time.time * m_hoverSpeed) * m_hoverAmplitude;
        transform.position = initialPosition + new Vector3(0f, hoverOffset, 0f);
    }
    #endregion
}