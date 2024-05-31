using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private UIDocument uiDocument;
    public bool isNewGame = true;

    public SceneField gameScene;
    public SceneField demoScene;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    void Start()
    {
        // Get the buttons
        var startButton = uiDocument.rootVisualElement.Q<Button>("start_button");
        var optionButton = uiDocument.rootVisualElement.Q<Button>("option_button");
        var quitButton = uiDocument.rootVisualElement.Q<Button>("quit_button");

        // Register click event handlers
        if (startButton != null)
        {
            if (isNewGame) startButton.text = "New Game";
            else startButton.text = "Continue";
            startButton.RegisterCallback<ClickEvent>(evt => StartGame());
        }
        if (optionButton != null)
        {
            optionButton.RegisterCallback<ClickEvent>(evt => OpenOptions());
        }
        if (quitButton != null)
        {
            quitButton.RegisterCallback<ClickEvent>(evt => QuitGame());
        }
    }

    void StartGame()
    {
        if (isNewGame)
            SceneManager.LoadScene(demoScene);
        else
            SceneManager.LoadScene(gameScene);
    }

    void OpenOptions()
    {
        Debug.Log("Option button clicked");
    }

    void QuitGame()
    {
        Debug.Log("Quit button clicked");
    }
}