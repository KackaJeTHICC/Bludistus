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
        if (m_masterMixer == null)
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
    #endregion
}
