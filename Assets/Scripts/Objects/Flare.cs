public class Flare : Pickable
{
    #region Methods
    public override void PickUp()
    {
        GameManager.instance.FlarePickedUp();
        gameObject.SetActive(false);
    }
    #endregion
}