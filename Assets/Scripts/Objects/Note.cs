public class Note : Pickable
{
    #region Methods
    public override void PickUp()
    {
       GameManager.instance.NotePickedUp(byte.Parse(gameObject.name.TrimStart('N', 'o', 't', 'e')));
        gameObject.SetActive(false);
    }
    #endregion
}
