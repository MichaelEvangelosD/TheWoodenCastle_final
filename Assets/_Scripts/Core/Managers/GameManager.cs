using UnityEngine;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 *
 * [Class Flow]
 * 1. The entry point of the class is inside the Start() method in which the CurrentGameScene property
 *      is set to the corresponding data field in the LevelData.cs script present in this scene.
 * 
 * [Must Know]
 * 1. EVERY time the CurrentGameState property is set, UpdateGameState(...) gets called.
 * 2. UpdateGameScene(...), after Switching on the _currentGameScene it calls the OnGameSceneChanged(newScene) to 
 *      notify the subsribers that the game scene changed.
 */

/// <summary>
/// All the Game Scenes the game can be on, InMainMenu (0) being the default.
/// <para>Each enum int corresponds to the scene build index.</para>
/// </summary>
public enum GameScenes
{
    InMainMenu = 0,
    InTutorialScene = 1,
    InLevel1 = 2,
    InLevel2 = 3,
    InBestEnding = 4,
    InMixedEnding = 5,
    InWorstEnding = 6,
    InWorstTutorial = 7,
    InSuicideScene = 8,
}

/// <summary>
/// All the possible endings the game may have, Best being the default.
/// <para>Enum ints are automatically set from the GameScenes enum by casting their values</para>
/// </summary>
public enum EligibleEndings
{
    Best = (int)GameScenes.InBestEnding,
    Mixed = (int)GameScenes.InMixedEnding,
    Worst = (int)GameScenes.InWorstEnding,
}

[DefaultExecutionOrder(500)]
public class GameManager : MonoBehaviour
{
    //PseudoSingleton
    public static GameManager S;

    [Header("Set in inspector")]
    [SerializeField] float bestEndingEnemyMax = 5; //Max enemies to kill to get the best ending
    [SerializeField] float mixedEndingEnemyMax = 11; //Max enemies to kill to get the mixed ending

    private static int _playerDeaths = 0; //How many times has the player died.

    //How many enemies has the player killed during THIS run.
    private static int _enemiesKilled = 0;
    public int EnemiesKilled
    {
        get { return _enemiesKilled; }
        private set { _enemiesKilled = value; }
    }

    private GameScenes _currentGameScene;
    /// <summary>
    /// Set when you want to update the games' current scene state
    /// </summary>
    public GameScenes CurrentGameScene
    {
        get { return _currentGameScene; }
        private set
        {
            _currentGameScene = value;

            UpdateGameScene(value);
        }
    }

    private EligibleEndings _eligibleEndings;
    public EligibleEndings EligibleEnding
    {
        get { return _eligibleEndings; }
        private set { _eligibleEndings = value; }
    }

    private bool _controllerConnected;
    public bool ControllerConnected
    {
        get { return _controllerConnected; }
        set { _controllerConnected = value; }
    }

    #region STARTUP_SETUP
    private void Awake()
    {
        S = this;

        //Sub the methods to the appropriate event triggers
        if (FindObjectOfType<GameEvents>() && GameEvents.S != null)
        {
            GameEvents.S.onGameSceneChanged += SetKilledEnemies;

            GameEvents.S.onEnemyDeath += IncrementEnemiesKilled;
            GameEvents.S.onPlayerDeath += IncrementPlayerDeaths;
        }
        else
        { Utils.PrintMissingComponentMsg("GameEvents script", this); }
    }

    /// <summary>
    /// Called every time the scene changes to set the correct killed enemy number from the previous scene.
    /// <para>When on the tutorial scene it resets the killed enemies to 0</para>
    /// <para>Validates the eligible ending at the end.</para>
    /// </summary>
    void SetKilledEnemies(GameScenes scene)
    {
        if (scene == GameScenes.InTutorialScene)
        {
            _enemiesKilled = 0;
            DetermineEnding();
        }
        else
        {
            EnemiesKilled = _enemiesKilled;
            DetermineEnding();
        }
    }

    /// <summary>
    /// Call to incease the current amount of enemies killed by 1
    /// <para>Validates the ending after incrementing</para>
    /// </summary>
    void IncrementEnemiesKilled()
    {
        _enemiesKilled++;

        DetermineEnding();
    }

    /// <summary>
    /// Call to set the EligibleEnding property on the appropriate EligibleEnding enum value
    /// based on the current number of killed enemies.
    /// </summary>
    void DetermineEnding()
    {
        if (EnemiesKilled <= bestEndingEnemyMax)
        {
            EligibleEnding = EligibleEndings.Best;
        }
        else if (EnemiesKilled > bestEndingEnemyMax && EnemiesKilled <= mixedEndingEnemyMax)
        {
            EligibleEnding = EligibleEndings.Mixed;
        }
        else
        {
            EligibleEnding = EligibleEndings.Worst;
        }
    }

    /// <summary>
    /// Call to add 1 to PlayerDeaths and write the deaths number in the PlayerPrefs file
    /// </summary>
    void IncrementPlayerDeaths()
    {
        _playerDeaths++;

        PlayerPrefs.SetInt("playerDeaths", _playerDeaths);
    }

    private void Start()
    {
        ControllerConnected = false;

        //Get the CurrentGameScene from the LevelData script 
        if (FindObjectOfType<LevelData>() && LevelData.S != null)
        { CurrentGameScene = LevelData.S.GetSceneGameStateInfo(); }
        else
        { Utils.PrintMissingComponentMsg("LevelData script", this); }

    }
    #endregion

    /// <summary>
    /// Gets called when the CurrentGameScene property is set from a script
    /// </summary>
    /// <param name="newScene">The CURRENT loaded scene</param>
    void UpdateGameScene(GameScenes newScene)
    {
        switch (newScene)
        {
            case GameScenes.InMainMenu:
                //Reset the player damage when he dies
                PlayerPrefs.SetFloat("playerDMG", 1);

                //Display the mouse
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                break;

            case GameScenes.InTutorialScene:
                //Lock and make the cursor invisible
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                //Reset back to normal colors every time the game restarts
                ColorGrader.S.SetToColored();

                //Deactivate the player behaviour
                PlayerBehaviour.S.PlayerActive = false;
                break;

            case GameScenes.InWorstTutorial:
                //Set the mood
                ColorGrader.S.SetToBlackWhite();
                AudioMaster.S.SetMasterVolume(1f);

                //Deactivate the player behaviour
                PlayerBehaviour.S.PlayerActive = false;
                break;
        }

        //Trigger the OnGameSceneChanged event
        if (GameEvents.S != null)
        { GameEvents.S.OnGameSceneChanged(newScene); }
        else
        { Utils.PrintMissingComponentMsg("GameEvents script", this); }
    }

    private void OnDestroy()
    {
        S = null;

        //Unsub the methods to not get NullRef errors
        GameEvents.S.onGameSceneChanged -= SetKilledEnemies;

        GameEvents.S.onEnemyDeath -= IncrementEnemiesKilled;
        GameEvents.S.onPlayerDeath -= IncrementPlayerDeaths;
    }

    private void OnApplicationQuit()
    {
        //Clear all player progression from PlayerPrefs
        PlayerPrefs.DeleteAll();
    }
}
