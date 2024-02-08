using System.Collections;
using UnityEngine;
using Random = System.Random;

/// <summary>
/// Manages monster
/// </summary>
public class MonsterManager : MonoBehaviour
{
    #region Instance
    public static MonsterManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    #region Variables
    /// <summary>
    /// Is the monster spawned
    /// </summary>
    private bool m_isMonster = false;

    /// <summary>
    /// Random number generator
    /// </summary>
    private Random m_rng = new Random();

    /// <summary>
    /// Current spawn rate
    /// </summary>
    private float m_currentSpawnRate = 0f;

    /// <summary>
    /// Maximum number of notes
    /// </summary>
    private float m_maxNotes = 0f;

    /// <summary>
    /// Base disappearance time when no notes are collected
    /// </summary>
    [SerializeField]
    private float m_baseDisappearanceTime = 1f;

    /// <summary>
    /// Current disappearance time
    /// </summary>
    private float m_currentDisappearanceTime;

    /// <summary>
    /// Maximum disappearance time
    /// </summary>     
    [SerializeField]
    private float m_maxDisappearanceTime = 60f;
    #endregion

    #region Setters
    /// <summary>
    /// Sets the maximum amount of notes
    /// </summary>
    /// <param name="amount">Amount of notes in total</param>
    public void SetMaxNotes(byte amount)
    {
        m_maxNotes = amount;
    }

    /// <summary>
    /// Sets m_isMonster
    /// </summary>
    /// <param name="isAlive">Is the monster alive?</param>
    public void IsMonster(bool isAlive)
    {
        m_isMonster = isAlive;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Tries spawning the monster
    /// </summary>
    /// <param name="notesPickedUp">Amount of notes already picked up</param>
    public void TryMonsterSpawn(byte notesPickedUp)
    {
        if (m_isMonster)    //we don't want to have more then 1 enemy at the time
        {
            return;
        }

        //randomly spawns monster based on spawn rate
        CalculateSpawnRateAndDisappearanceTime(notesPickedUp);
        if (m_currentSpawnRate * 100 >= m_rng.Next(1, 100))
        {
            m_isMonster = true;
            StartCoroutine(SpawnAndDestroyMonsterAfterSomeTimes());
        }
    }

    /// <summary>
    /// Spawns the monster and despawns it after set amount of time
    /// </summary>
    private IEnumerator SpawnAndDestroyMonsterAfterSomeTimes()
    {
        GameObject monster = Instantiate(Resources.Load("Prefabs/Monster"), LevelStart.instance.RandomSpot(), Quaternion.Euler(0f, 0f, 0f)) as GameObject;
        yield return new WaitForSeconds(m_currentDisappearanceTime);
        Destroy(monster);
        m_isMonster = false;
    }

    /// <summary>
    /// Calculates spawn rate and disappearance time
    /// </summary>
    private void CalculateSpawnRateAndDisappearanceTime(byte notesPickedUp)
    {
        m_currentSpawnRate = Mathf.Clamp01(notesPickedUp / m_maxNotes); //calculates the spawn rate based on the number of notes collected
        m_currentDisappearanceTime = Mathf.Lerp(m_baseDisappearanceTime, m_maxDisappearanceTime, notesPickedUp / m_maxNotes);   //calculates the disappearance time based on the number of notes collected
    }
    #endregion
}