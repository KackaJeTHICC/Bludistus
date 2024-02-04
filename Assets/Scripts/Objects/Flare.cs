using UnityEngine;

public class Flare : MonoBehaviour, IPickable
{
    #region Methods
    public void PickUp()
    {
        GameManager.instance.NotePickedUp();

        print("flare picked up");

        //TODO do some logic
        //then delete it
        gameObject.SetActive(false);
    }
    #endregion
}
