using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public string sceneToLoad = "MainScene";

    void Update()
    {
        // qualquer tecla
        if (Input.anyKeyDown)
        {
            LoadGame();
        }

        // qualquer clique do mouse/touch
        if (Input.GetMouseButtonDown(0))
        {
            LoadGame();
        }
    }

    void LoadGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
