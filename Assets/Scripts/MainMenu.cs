using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    private SceneField sceneToLoad; // Reference to the scene to load

    public void StartGame() {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame() {
        Application.Quit();
    }

}
