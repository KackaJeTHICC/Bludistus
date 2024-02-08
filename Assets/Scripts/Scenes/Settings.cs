using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Handless all game related settings
/// </summary>
public class Settings : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Master audio mixer
    /// </summary>
    [SerializeField]
    private AudioMixer m_masterMixer = null;
    #endregion

    #region Methods
    private void Start()
    {
        if (m_masterMixer == null)  //this should never occur
        {
            Debug.LogError("Audio Mixer is not assigned!");
        }
        SetVolume(PlayerPrefs.GetFloat("MasterVolume"));
    }

    /// <summary>
    /// Sets game volume
    /// </summary>
    /// <param name="volume">Volume in db</param>
    public void SetVolume(float volume)
    {
        m_masterMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    /// <summary>
    /// Sets the brightness
    /// </summary>
    /// <param name="brightness">Game brightness</param>
    public void SetBrightness(float brightness)
    {
        RenderSettings.ambientLight = new Color(0.212f, 0.227f, 0.259f, Mathf.Clamp(brightness, 0.7f, 1.3f));
        PlayerPrefs.SetFloat("Brightness", brightness);
    }
    #endregion
}