/// <summary>
/// Flare class which inherits from the Pickable class
/// </summary>
public class Flare : Pickable
{
    public override void PickUp()
    {
        GameManager.instance.FlarePickedUp();
        gameObject.SetActive(false);
    }
}