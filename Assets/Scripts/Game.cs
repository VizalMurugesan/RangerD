using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance;
    public Player player;

    public void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }
    }

//MainMenu Methods
    public void QuitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        SceneManager.LoadSceneAsync("Scene1");
    }
}
