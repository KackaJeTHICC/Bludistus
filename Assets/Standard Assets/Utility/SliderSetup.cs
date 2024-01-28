using UnityEngine;
using UnityEngine.UI;

public class SliderSetup : MonoBehaviour
{
    [SerializeField]
    [Header("Value that will get loaded from PlayerPrefs")]
    private string m_value = "";

    private void OnEnable()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat(m_value, -42f);
    }
}
