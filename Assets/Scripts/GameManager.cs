using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A class that allows us to manage the scene name and its scene type from the Inspector.
/// </summary>
[Serializable]
public class GameSceneInfo
{
    public string sceneName;
    public SceneType sceneType;
}

/// <summary>
/// Enum that defines the GameState.
/// </summary>
[Serializable]
public enum GameState
{
    Title = 0,
    GamePlay = 1,
    End = 2,
    Num
}

/// <summary>
/// Enum that defines the SceneType.
/// </summary>
public enum SceneType
{
    Title = 0,
    Game = 1,
    End = 2,
    Num
}

/// <summary>
/// This manager is responsible for game flow and player information, and it implements the Singleton pattern.
/// </summary>
public class GameManager : MonoBehaviour
{
    // static field
    public static GameManager instance; 
    
    // A state machine that manages the GameState.
    private StateMachine stateMachine; 

    // This property has only a getter, so it is read-only to external classes.
    public GameState CurState => (GameState)stateMachine.CurrentState; 

    // Unity allows the list to be modified from the Inspector (since List is a serializable data structure).
    public List<GameSceneInfo> gameSceneInfos = new List<GameSceneInfo>(); 

    // Unity can't serialize dictionaries, so we set up the dictionary through code.
    private Dictionary<SceneType, string> sceneNameLookup = new Dictionary<SceneType, string>(); 

    
    // Check if this object is the only instance in the scene. (Singleton)
    private void Awake()
    {
        // if the current instance hasn't been assigned, that means this object is the first instance of this class.
        if (instance == null)
        {
            // Set the instance to this object.
            instance = this; 

            // Make this object persistent until the game ends.
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // if the instance already exist and it's not this object
            if (instance != this)
            {
                // destroy this one
                Destroy(gameObject); 
            }
        }
    }

    // Grab information from the gameSceneInfos list and build the dictionary for consistent and quick lookup of scene types and their names.
    private void InitSceneLookup()
    {
        sceneNameLookup.Clear();
        foreach (var sceneInfo in gameSceneInfos)
        {
            sceneNameLookup[sceneInfo.sceneType] = sceneInfo.sceneName;
        }
    }

    // Set up functions for each state.
    private void InitStateMachine()
    {
        stateMachine = new StateMachine((int)GameState.Num);

        // one way to setup function for state
        stateMachine.SetEnterStateFunc((int)GameState.Title, EnterTitleState);
        stateMachine.SetUpdateStateFunc((int)GameState.Title, () => { return; });
        stateMachine.SetExitStateFunc((int)GameState.Title, () => { return; });

        // another way to do it.
        stateMachine.SetStateFunctions((int)GameState.GamePlay, EnterInGameState, () => { return; }, () => { return; });
        stateMachine.SetStateFunctions((int)GameState.End, EnterEndState, () => { return; }, () => { return; });
    }

    private void EnterTitleState()
    {
        // [C1_QUIZ]
        // If you want to change scene to Title scene, what should be put in here?
        SceneManager.LoadScene(sceneNameLookup[SceneType.Title]);
    }

    private void EnterInGameState()
    {
        // [C1_QUIZ]
        // If you want to change scene to Game scene, what should be put in here?
        SceneManager.LoadScene(sceneNameLookup[SceneType.Game]);
    }

    private void EnterEndState()
    {
        // [C1_QUIZ]
        // If you want to change scene to End scene, what should be put in here?
        SceneManager.LoadScene(sceneNameLookup[SceneType.End]);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitSceneLookup();
        InitStateMachine();
        SetState((int)GameState.Title);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.UpdateState();
    }

    // Set state for the internal state machine
    public void SetState(GameState state)
    {
        stateMachine.SetState((int)state);
    }

    // Change scene by scene type
    private void ChangeScene(SceneType sceneType)
    {
        if (SceneManager.GetActiveScene().name != sceneNameLookup[sceneType])
        {
            SceneManager.LoadScene(sceneNameLookup[sceneType]);
        }
    }
}
