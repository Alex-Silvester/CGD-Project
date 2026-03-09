using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the current gameplay state: Prelude (Payday searching around), Playing (main gameplay), Victory (player completed enough quotas), Game Over (player failed too many quotas)
/// Uses the State Machine Pattern.
/// There is a GameState interface that specifies the three things a state can do: OnEnter (Start() for the state), UpdateState (Update() for the state, OnExit (OnDestroy() for the state)
/// There is a variable, currentState, that will be used to determine what the GameManager will do.
/// ChangeState() can be used to change to another state. The old state's OnExit() and new state's OnEnter() will automatically be called using this function.
/// </summary>
public class GameManager : MonoBehaviour
{
	// Reference to the current active game state
    public IGameState currentState { get; private set; }

	// Create instances of game state classes
	// They are containers for code
	// They can be passed in as a reference to ChangeState()
    // Source - https://gamedevbeginner.com/state-machines-in-unity-how-and-when-to-use-them/
    public PreludeState preludeState = new PreludeState(); // E.g. tutorial room timer not started
    public PlayingState playingState = new PlayingState();
    public VictoryState victoryState = new VictoryState();
    public GameOverState gameOverState = new GameOverState();

    // Singleton
    public static GameManager instance { get; private set; }

	[Header("Settings")]
    [SerializeField] AudioSource button_sound;
	
    private string next_scene = "";

    private void Awake()
    {
        // Singleton
        instance = this;
    }

    #region State Machine

    private void Start()
    {
        // Subscribe to events
        GameOverButtonInteractable.onPressed += SetGameOverState;
		TimeManager.instance.onTimerRanOut.AddListener(SetVictoryState);

        Time.timeScale = 1.0f; // Reset from any previous playthroughs
        ChangeState(playingState);
    }

    private void Update()
    {
        if (currentState != null)
            currentState.UpdateState(this);
    }

	/// <summary>
	/// Used to change to another state by passing in a reference to a game state object.
	/// The old state's OnExit() and new state's OnEnter() will automatically be called.
	/// Example code to cause game over when L key is pressed:
    ///    if (Input.GetKeyDown(KeyCode.L))
    ///        ChangeState(gameOverState);
	/// </summary>
    private void ChangeState(IGameState newState)
    {
        if (currentState != null)
            currentState.OnExit(this);

        currentState = newState;
        currentState.OnEnter(this);
    }

    private void SetGameOverState()
    {
        ChangeState(gameOverState);
    }
	
	private void SetVictoryState()
	{
		ChangeState(victoryState);
	}

    #endregion

    #region Level Utility

    public void LoadScene(string sceneToLoad)
    {
        next_scene = sceneToLoad;

        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        Debug.Log("Started");
        yield return new WaitForSecondsRealtime(button_sound.clip.length);
        if (next_scene != string.Empty)
        {
			// Make sure to unpause
			Time.timeScale = 1.0f;
			
            SceneManager.LoadScene(next_scene);
        }
        yield return null;
    }

    public void ReloadCurrentScene()
    {
        currentState.OnExit(this);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion Utility

	// Unsubscribe from static events.
	// If we don't the next time the game scene (such as play again) loads will try to call both the now destroyed game object and the new one (when only the new one is needed).
    private void OnDestroy()
    {
        // Unsubscribe from events
        GameOverButtonInteractable.onPressed -= SetGameOverState;
		TimeManager.instance.onTimerRanOut.RemoveListener(SetVictoryState);
    }
}

/// <summary>
/// Interface for a game state class to implement. Implementing classes will add actual functionality.
/// OnEnter is called once when the game state is first entered
/// UpdateState is called every frame
/// OnExit is called once when the game state is being changed to a new one
/// Source - https://gamedevbeginner.com/state-machines-in-unity-how-and-when-to-use-them/
/// </summary>
public interface IGameState
{
    public void OnEnter(GameManager manager);
    public void UpdateState(GameManager manager);
    public void OnExit(GameManager manager);
}

/// <summary>
/// Used for pre-gameplay when the quotas and timers aren't active
/// Could be used for lobby area (like REPO) or Payday caseing
/// </summary>
public class PreludeState : IGameState
{
	// Events can be subscribed to by outside classes.
	// This allows them to react to when the event happens, without clutering this class with lots of code.
    public delegate void OnEntered();
    public static event OnEntered onEntered;
    public delegate void OnExited();
    public static event OnExited onExited;

    public void OnEnter(GameManager manager)
    {
        if (onEntered != null)
            onEntered();
    }

    public void UpdateState(GameManager manager)
    {
		// Add prelude state Update() code here.
    }

    public void OnExit(GameManager manager)
    {
        if (onExited != null)
            onExited();
    }
}

public class PlayingState : IGameState
{
	// Events can be subscribed to by outside classes.
	// This allows them to react to when the event happens, without clutering this class with lots of code.
    public delegate void OnEntered();
    public static event OnEntered onEntered;
    public delegate void OnExited();
    public static event OnExited onExited;

    public void OnEnter(GameManager manager)
    {
        if (onEntered != null)
            onEntered();
    }

    public void UpdateState(GameManager manager)
    {
		// Add playing state Update() code here.
    }

    public void OnExit(GameManager manager)
    {
        if (onExited != null)
            onExited();
    }
}

public class VictoryState : IGameState
{
	// Events can be subscribed to by outside classes.
	// This allows them to react to when the event happens, without clutering this class with lots of code.
    public delegate void OnEntered();
    public static event OnEntered onEntered;
    public delegate void OnExited();
    public static event OnExited onExited;

    public void OnEnter(GameManager manager)
    {
        onEntered?.Invoke();
        Time.timeScale = 0;
    }

    public void UpdateState(GameManager manager)
    {
		// Add victory state Update() code here.
    }

    public void OnExit(GameManager manager)
    {
        onExited?.Invoke();
        Time.timeScale = 1;
    }
}

public class GameOverState : IGameState
{
	// Events can be subscribed to by outside classes.
	// This allows them to react to when the event happens, without clutering this class with lots of code.
    public delegate void OnEntered();
    public static event OnEntered onEntered;
    public delegate void OnExited();
    public static event OnExited onExited;

    public void OnEnter(GameManager manager)
    {
        // Stop player movement, physics, etc.
        Time.timeScale = 0.0f;

        if (onEntered != null)
            onEntered();
    }

    public void UpdateState(GameManager manager)
    {
		// Add game over state Update() code here.
    }

    public void OnExit(GameManager manager)
    {
        if (onExited != null)
            onExited();
    }
}