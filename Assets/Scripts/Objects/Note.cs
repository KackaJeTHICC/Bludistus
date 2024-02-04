using UnityEngine;

public class Note : MonoBehaviour, IPickable
{
    #region Methods
    public void PickUp()
    {
        GameManager.instance.NotePickedUp();

        print("note no." + LevelStart.instance.NotesNeeded() + " picked up");
        
        //TODO show the note
        //then delete it
        gameObject.SetActive(false);
    }
    #endregion
}
