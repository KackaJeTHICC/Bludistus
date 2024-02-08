using UnityEngine;

/// <summary>
/// Handles gameover
/// </summary>
public class GameOver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //enables cursor+gameover screen and disables player
        {
            transform.GetChild(0).gameObject.SetActive(true);
            other.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}