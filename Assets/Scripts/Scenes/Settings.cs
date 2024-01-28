using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 
/// </summary>
public class Settings : MonoBehaviour
{
#region Variables

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private AudioMixer m_masterMixer = null;

    #endregion


    #region Methods
    private void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("MasterVolume"));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {
        if (m_masterMixer == null)
        {
            Debug.LogError("Audio Mixer is not assigned!");
        }
        else
        {
            m_masterMixer.SetFloat("MasterVolume", volume);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
    }

#endregion
}
