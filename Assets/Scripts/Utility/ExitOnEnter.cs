using UnityEngine;

public class ExitOnEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
#if UNITY_EDITOR
            Debug.Log("Exiting game...");
#else
        Application.Quit();
#endif
        }
    }
}
