using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 *
 * [Variable Specifics]
 * Dynamically changed: All variables are dynamically cached and changed throughout the game
 * 
 * [Class Flow]
 * 1. Entry point of the script is the SetDefaultUIStates() method that executes every time
 *  a new scene loads.
 * 2. The SliderValue increases from a coin pickup OR an enemy kill (event triggers).
 * 
 * [Must Know]
 * 1. When the SliderValue property is set it automatically calls the OnSliderValueChanged() method
 *  to do the appropriate things (from UI to player damage changes).
 *  
 * 2. The PlayerDMG (in PlayerAttack script) gets zeroed and set back to playerDMG (from PlayerPrefs) 
 *  when we update its value from the SetDefaultUIStates() method.
 */

[DefaultExecutionOrder(445)]
public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager S;

    [Header("Set Dynamically")]
    [SerializeField] Slider experienceSlider; //The exp UI slider reference
    [SerializeField] TextMeshProUGUI currentDamageText; //The dmg UI text reference

    static int sliderValue; //Current slider value
    /// <summary>
    /// When SET OnSliderValueChanged() gets called to update the UI element value
    /// </summary>
    public int SliderValue
    {
        get { return sliderValue; }
        set
        {
            sliderValue = value;

            //Update the UI slider value
            OnSliderValueChanged();
        }
    }

    #region STARTUP_SETUP
    private void Awake()
    {
        S = this;

        //Subsribe the methods to their appropriate events
        GameEvents.S.onGameSceneChanged += SetDefaultUIStates;

        GameEvents.S.onEnemyDeath += EnemyKill;
        GameEvents.S.onCoinPickup += CoinPickUp;
    }

    /// <summary>
    /// Called everytime the scene changes.
    /// <para>If the current scene is the Tutorial then set SliderValue to 0</para>
    /// <para>Else update the player damage and slider values</para>
    /// </summary>
    void SetDefaultUIStates(GameScenes state)
    {
        //If we are in the tutorial scene
        if (state == GameScenes.InTutorialScene)
        {
            SliderValue = 0; //Reset the sldier value when starting the game
        }
        else
        {
            //Set the filling of the experience slider
            experienceSlider.value = SliderValue;

            //Set the players damage every time we change a scene
            PlayerBehaviour.S.PlayerAttack.IncreasePlayerDamage(PlayerPrefs.GetFloat("playerDMG"), true);

            //Update the GUI text to display the correct player damage
            UpdateDMGText(PlayerBehaviour.S.PlayerAttack.PlayerDMG);
        }
    }

    private void Start()
    {
        //Cache the needed component references
        experienceSlider = GameObject.Find("ExperienceSlider").GetComponent<Slider>();
        currentDamageText = GameObject.Find("Text_CurrentDamage").GetComponent<TextMeshProUGUI>();
    }
    #endregion

    /// <summary>
    /// Call to set the damage level of the player and set the experience slider value to the
    /// appropriate integer states.
    /// </summary>
    void OnSliderValueChanged()
    {
        //A value between 0f and 1f
        float wholeDivisionValue = SliderValue / experienceSlider.maxValue;

        //Keep only the right hand side of the float value
        double sliderIncrease = wholeDivisionValue - Math.Truncate(wholeDivisionValue);

        //If we reach/surpass the maximum value of the UI slider
        if (SliderValue >= experienceSlider.maxValue)
        {
            //Time to LEVEL UP
            RandomizeAndIncreaseDamage();

            //If we don't round the float to an int then we can't update the UI slider's value correctly
            SliderValue = Mathf.RoundToInt((float)((experienceSlider.minValue + 1) * sliderIncrease * 10) / 2);
        }
        else // else just update its filling value
        {
            experienceSlider.value = SliderValue;
        }

        //Lastly update the UI damage text, based on the Players' damage
        UpdateDMGText(PlayerBehaviour.S.PlayerAttack.PlayerDMG);
    }

    #region LEVELUP_HANDLING
    /// <summary>
    /// Increase player damage based on a random selection of damage increase
    /// </summary>
    void RandomizeAndIncreaseDamage()
    {
        //Throw a 'dice'
        System.Random randomizer = new System.Random((int)System.DateTime.Now.Ticks);

        if (randomizer.Next(0, 11) < 7)
        {
            PlayerBehaviour.S.PlayerAttack.IncreasePlayerDamage(0.5f);
        }
        else
        {
            PlayerBehaviour.S.PlayerAttack.IncreasePlayerDamage(1f);
        }
    }

    /// <summary>
    /// Updates the UI damageText to display the correct player damage
    /// </summary>
    /// <param name="playerDamage">The player damage to display</param>
    void UpdateDMGText(float playerDamage)
    {
        currentDamageText.text = $"DAMAGE:{playerDamage}";
    }
    #endregion

    #region EVENT_HANDLING
    /// <summary>
    /// <para>Called from GameEvents -> onEnemyDeath event</para>
    /// Increases SliderValue by 1 fifth
    /// </summary>
    void EnemyKill()
    {
        SliderValue++;
    }

    /// <summary>
    /// <para>Called from GameEvents -> onCoinPickup event</para>
    /// Increases SliderValue by 1 fifth
    /// </summary>
    void CoinPickUp()
    {
        SliderValue++;
    }
    #endregion

    private void OnDestroy()
    {
        S = null;

        //Unsub events to not get null reference errors
        GameEvents.S.onGameSceneChanged -= SetDefaultUIStates;
        GameEvents.S.onEnemyDeath -= EnemyKill;
        GameEvents.S.onCoinPickup -= CoinPickUp;
    }
}
