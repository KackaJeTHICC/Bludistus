using UnityEngine;

public class Note : MonoBehaviour
{
    #region Variables

    #endregion


    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.NotePickedUp();       
            print("note no." + LevelStart.instance.NotesNeeded());
        }
    }
    #endregion
}
