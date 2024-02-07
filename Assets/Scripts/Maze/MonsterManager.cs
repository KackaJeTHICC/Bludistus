using System.Collections;
using UnityEngine;
using Random = System.Random;

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

    public float baseSpawnRate = 1f; // Base spawn rate when no notes are collected
    public float currentSpawnRate; // Current spawn rate
    public float maxNotes; // Maximum number of notes
    public float baseDisappearanceTime = 10f; // Base disappearance time when no notes are collected
    public float currentDisappearanceTime; // Current disappearance time
    public float maxDisappearanceTime = 60f; // Maximum disappearance time
    #endregion

    #region Setters
    /// <summary>
    /// Sets the maximum amount of notes
    /// </summary>
    /// <param name="amount">Amount of notes in total</param>
    public void SetMaxNotes(byte amount)
    {
        maxNotes = amount;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Tries spawning the monster
    /// </summary>
    /// <param name="notesPickedUp">Amount of notes already picked up</param>
    public void TryMonsterSpawn(byte notesPickedUp)
    {
        if (m_isMonster)
        {
            return;
        }
        CalculateSpawnRateAndDisappearanceTime(notesPickedUp);
        if (currentSpawnRate * 100 >= m_rng.Next(1, 100))
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
        yield return new WaitForSeconds(currentDisappearanceTime);
        Destroy(monster);
        m_isMonster = false;
    }

    /// <summary>
    /// Calculates spawn rate and disappearance time
    /// </summary>
    private void CalculateSpawnRateAndDisappearanceTime(byte notesPickedUp)
    {
        // Calculate the spawn rate linearly based on the number of notes collected
        currentSpawnRate = Mathf.Clamp01(notesPickedUp / maxNotes);

        // Calculate the disappearance time linearly based on the number of notes collected
        currentDisappearanceTime = Mathf.Lerp(baseDisappearanceTime, maxDisappearanceTime, notesPickedUp / maxNotes);
    }
    #endregion
}