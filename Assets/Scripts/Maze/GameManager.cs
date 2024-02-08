using System.Collections;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// Manages everything after player spawn
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Instance
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    #region Variables 
    [Header("Game")]
    /// <summary>
    /// Game difficulty settings
    /// </summary>
    private DifficultySettigns m_difficulty;

    [Header("Notes")]
    /// <summary>
    /// Amount of notes that were already picked up
    /// </summary>
    private byte m_notesPickedUp = 0;

    /// <summary>
    /// Note gameobject
    /// </summary>    
    [SerializeField]
    private GameObject m_note = null;

    [Header("Flare")]
    /// <summary>
    /// Flare camera
    /// </summary>
    [SerializeField]
    private Camera m_flareCamera = null;

    /// <summary>
    /// Flare game object
    /// </summary>
    [SerializeField]
    private GameObject m_flare = null;

    /// <summary>
    /// Flare projectile game object
    /// </summary>
    [SerializeField]
    private GameObject m_flareProjectile = null;

    /// <summary>
    /// Does player have flare?
    /// </summary>
    private bool m_hasFlare = false;

    /// <summary>
    /// Speed of flare shaking in hand
    /// </summary>  
    [SerializeField]
    private float m_shakeSpeed = 30f;

    /// <summary>
    /// Angle at which the flare will be shot at the sky instead of the enemy
    /// </summary>  
    [SerializeField]
    private float m_skyAngle = -0.2748f;

    /// <summary>
    /// Amplitude of flare shaking in hand
    /// </summary>
    [SerializeField]
    private float m_shakeAmplitude = 0.03f;

    /// <summary>
    /// Initial local position of flare in hand
    /// </summary>
    private Vector3 m_initiaLocalPosition;

    [Header("Pause")]
    /// <summary>
    /// Pause menu game object
    /// </summary>
    [SerializeField]
    private GameObject m_pauseMenu = null;

    /// <summary>
    /// Is the user input paused?
    /// </summary>
    private bool m_isInputLocked = false;

    /// <summary>
    /// Is the game paused?
    /// </summary>
    private bool m_isPause = false;

    [Header("Player")]
    /// <summary>
    /// Player game object
    /// </summary>  
    [SerializeField]
    private GameObject m_player = null;

    /// <summary>
    /// Player camera
    /// </summary>
    [SerializeField]
    private Camera m_playerCamera = null;

    /// <summary>
    /// Finish line object
    /// </summary>  
    private GameObject m_finishLine = null;
    #endregion

    #region Getters/Setters
    /// <summary>
    /// DifficultySettigns setter                                                
    /// </summary>
    /// <param name="difficulty">Game difficulty enum</param>
    public void Difficulty(DifficultySettigns difficulty)
    {
        m_difficulty = difficulty;
    }
    /// <summary>
    /// DifficultySettigns getter                                                
    /// </summary>
    /// <param name="difficulty">Game difficulty enum</param>
    public DifficultySettigns Difficulty()
    {
        return m_difficulty;
    }

    /// <summary>
    /// Is the user input paused?
    /// </summary>
    /// <param name="isLocked">true if we want to lock users input</param>
    public void isInputLocked(bool isLocked)
    {
        m_isInputLocked = isLocked;
    }
    #endregion

    #region Methods
    private void Start()    //this should never occur and the Find()/GetChild() will most likely fail anyway
    {
        if (m_player == null)
        {
            Debug.LogError("Player is not assigned!");
            m_player = GameObject.Find("PlayerCapsule");
        }
        if (m_pauseMenu == null)
        {
            Debug.LogError("Pause menu is not assigned!");
            m_pauseMenu = GameObject.Find("Pause menu").transform.GetChild(0).gameObject;
        }
        if (m_playerCamera == null)
        {
            Debug.LogError("Player camera is not assigned!");
            m_playerCamera = m_player.transform.GetChild(0).GetComponent<Camera>();
        }
        if (m_flareCamera == null)
        {
            Debug.LogError("Flare camera is not assigned!");
            m_flareCamera = m_player.transform.GetChild(1).GetComponent<Camera>();
        }
        if (m_flare == null)
        {
            Debug.LogError("Flare is not assigned!");
            m_flare = m_player.transform.GetChild(2).gameObject;
        }
        if (m_flareProjectile == null)
        {
            Debug.LogError("Flare Projectile is not assigned!");
            m_flareProjectile = m_player.transform.GetChild(0).gameObject;
        }
        if (m_note == null)
        {
            Debug.LogError("Note is not assigned!");
            m_note = m_player.transform.GetChild(3).gameObject;
        }
    }

    private void Update()
    {
        if (m_isInputLocked)    //disables player input when it is locked
        {
            return;
        }
        if (CrossPlatformInputManager.GetButtonDown("Cancel"))  //Pauses the game if it's unpaused and vice versa
        {
            PauseGame();
        }
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && m_hasFlare)  //Shoots the flare
        {
            StartCoroutine(FlareUse());
        }
    }

    private void LateUpdate()
    {
        if (m_hasFlare) //shakes the flare gun, if player has one
        {
            ShakeAnimation();
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;    //fixes a glitch where game wouldn't start properly on 2nd try
    }

    #region Flare
    /// <summary>
    /// Plays the zooming out animation
    /// </summary>
    private IEnumerator FlareUse()
    {
        if (m_isPause)  //doesn't shoot the flare if player paused the game
        {
            yield break;
        }
        m_hasFlare = false;
        m_flare.SetActive(false);
        LevelStart.instance.FlareSpawn();

        Vector3 rayOrigin = m_playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Vector3 rayDirection = m_playerCamera.transform.forward;
        RaycastHit hitInfo;
        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo) &&
            m_playerCamera.transform.localRotation.x > m_skyAngle)   //shooting at the enemy
        {
            m_flareProjectile.SetActive(true);
            m_flareProjectile.transform.LookAt(hitInfo.point);
        }
        else    //shooting at the sky
        {
            float width = MazeRenderer.instance.GetMazeSize().x;
            float height = MazeRenderer.instance.GetMazeSize().y;
            float aspectRatio = Screen.width / (float)Screen.height;
            float targetSize;

            if (width > height) //landscape orientation
            {
                targetSize = width / 2f / aspectRatio;
            }
            else    //portrait orientation
            {
                targetSize = height / 2f;
            }
            targetSize += 10f;  //adds padding

            float timeToLerp = 15f;
            float passedTime = 0f;

            m_playerCamera.gameObject.SetActive(false);
            m_flareCamera.gameObject.SetActive(true);
            RenderSettings.fog = false;
            while (passedTime < timeToLerp + 0.5f)  //zooms out the camera for given amount of time
            {
                m_flareCamera.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                m_flareCamera.transform.position = Vector3.Lerp(m_playerCamera.transform.position,
                    new Vector3(m_playerCamera.transform.position.x, targetSize, m_playerCamera.transform.position.z),
                    passedTime / timeToLerp);
                passedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            RenderSettings.fog = true;
            m_playerCamera.gameObject.SetActive(true);
            m_flareCamera.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Logic after picking up the flare
    /// </summary>
    public void FlarePickedUp()
    {
        m_flare.SetActive(true);
        m_initiaLocalPosition = m_flare.transform.localPosition;
        m_hasFlare = true;
    }

    /// <summary>
    /// Animation of flare shaking in hand
    /// </summary>
    private void ShakeAnimation()
    {
        float perlinX = Mathf.PerlinNoise(Time.time * m_shakeSpeed, 0);
        float perlinY = Mathf.PerlinNoise(0, Time.time * m_shakeSpeed);
        float perlinZ = Mathf.PerlinNoise(Time.time * m_shakeSpeed, Time.time * m_shakeSpeed);

        float shakeOffsetX = (perlinX * 2 - 1) * m_shakeAmplitude;
        float shakeOffsetY = (perlinY * 2 - 1) * m_shakeAmplitude;
        float shakeOffsetZ = (perlinZ * 2 - 1) * m_shakeAmplitude;

        m_flare.transform.localPosition = m_initiaLocalPosition + new Vector3(shakeOffsetX, shakeOffsetY, shakeOffsetZ);
    }
    #endregion

    #region Note
    /// <summary>
    /// Logic after picking up a note
    /// </summary>
    /// <param name="materialNumber">Material number of a note</param>
    public void NotePickedUp(byte materialNumber)
    {
        byte notesNeeded = LevelStart.instance.NotesNeeded();

        m_notesPickedUp++;

        if (m_difficulty.HasFlag(DifficultySettigns.spawnMonster))  //tries to spawn monster if needed    
        {
            MonsterManager.instance.TryMonsterSpawn(m_notesPickedUp);
        }

        if (m_notesPickedUp >= notesNeeded)   //opens the gate if all notes are picked up
        {
            m_finishLine = GameObject.Find("Finish Line");
            m_finishLine.GetComponentInChildren<Animator>().SetBool("All notes collected", true);
            m_finishLine.GetComponent<AudioSource>().Play();
        }

        //displays the note for set amount of time
        m_note.SetActive(true);
        m_note.GetComponent<MeshRenderer>().material = LevelStart.instance.m_noteMaterials.ElementAt(materialNumber);
        StartCoroutine(DisableNoteInSeconds(3f));
    }

    /// <summary>
    /// Disables the note t seconds
    /// </summary>
    /// <param name="t">When the note should be disabled</param>
    private IEnumerator DisableNoteInSeconds(float t)
    {   //this has to be in separate method, otherwise the WaitForSeconds() wont work for some reason
        yield return new WaitForSeconds(t);
        m_note.SetActive(false);
    }
    #endregion

    /// <summary>
    /// Pauses/Unpauses the game
    /// </summary> 
    public void PauseGame()
    {
        if (m_isPause)  //unpauses the game
        {
            Time.timeScale = 1f;
            m_isPause = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            m_player.SetActive(true);
            m_pauseMenu.SetActive(false);
        }
        else    //pauses the game
        {
            Time.timeScale = 0f;
            m_isPause = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            m_pauseMenu.SetActive(true);
            m_player.SetActive(false);
        }
    }
    #endregion
}