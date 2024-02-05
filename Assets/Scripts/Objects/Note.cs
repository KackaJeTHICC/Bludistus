public class Note : Pickable
{
    #region Methods
    public override void PickUp()
    {
        GameManager.instance.NotePickedUp();
        
        //TODO show the note

        gameObject.SetActive(false);
    }
    #endregion
}
