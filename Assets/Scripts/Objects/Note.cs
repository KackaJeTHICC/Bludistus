/// <summary>
/// Note class which inherits from the Pickable class
/// </summary>
public class Note : Pickable
{
    public override void PickUp()
    {
        GameManager.instance.NotePickedUp(byte.Parse(gameObject.name.TrimStart('N', 'o', 't', 'e')));
        gameObject.SetActive(false);
    }
}