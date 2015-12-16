using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is intended to store some simple gamestate functionality and allow for a few specific objects to be accessed elsewhere.
/// </summary>
public class Core : MonoBehaviour
{
    private static Core m_Instance;
    private Vector3 m_PlayerStart = Vector3.zero;

    [SerializeField]
    private GameObject m_MainMenuRoot;              //the root of our main menu canvas. We store this so we can easily enable/disable the menu

    [SerializeField]
    private GameObject m_GameOverMenuRoot;          //the root of our game-over menu canvas. We store this so we can easily enable/disable the menu

    [SerializeField]
    private ObstacleManager m_ObstacleManager;      //the obstacle manager itself... we need to interact with it quite a bit

    [SerializeField]
    private FlappyController m_Player;              //keeps track of the player, as various things may need to reference the player (likely the player's position)

    [SerializeField]
    private RectTransform m_CanvasRoot;             //The root of our canvas, it'll be used in place of traditional Screen.width and Screen.height

    public RectTransform CanvasRoot
    {
        get { return m_CanvasRoot; }
        private set { m_CanvasRoot = value; }
    }

    public static Core Instance
    {
        get { return m_Instance; }
        private set { m_Instance = value; }
    }

    public FlappyController Player
    {
        get { return m_Player; }
    }

    public UnityEvent ResetEvent = null;        //called when the game first loads and when the player goes from the GameOver menu to the Main menu


    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        //record the player's starting position, we'll be resetting to this point after gameOver.
        m_PlayerStart = Player.cachedTransform.position;
        Player.OnDeath += OnGameOver;

        OnMainMenu();
    }

    public void OnMainMenu()
    {
        //we need to disable the game over screen and enable the main menu
        //we'll keep the obstacle manager from generating pillars while we're in the menu
        //like Flappy Bird, we want the scenic elements to scroll

        m_GameOverMenuRoot.SetActive(false);
        m_MainMenuRoot.SetActive(true);
        m_ObstacleManager.HidePillars();
        Player.cachedTransform.position = m_PlayerStart;
        Player.rigidBody2D.gravityScale = 0f;
        Player.StartMoving();

        ResetEvent.Invoke();
    }

    public void OnPlay()
    {
        //enable player input for flappy
        //set flappy to float while the user can't do anything (don't need it crashing into the ground)
        //tell the obstacle manager that it can start making pillars again
        //disable the main menu
        Player.EnableInput();
        Player.rigidBody2D.gravityScale = 1f;
        m_ObstacleManager.BeginDelayed(2f);
        m_MainMenuRoot.SetActive(false);
    }

    public void OnGameOver()
    {
        //enable the game over menu
        //disable player input for flappy
        //tell the obstacle manager to stop worrying about the pillars
        m_GameOverMenuRoot.SetActive(true);
        m_Player.DisableInput();
        m_ObstacleManager.DisablePillars();
    }
}
