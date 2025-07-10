using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance;
    public Player player;

    public enum SortingLayers { BaseGrass, BaseGround, path, VegetationBeforePlayer, StructuresBeforePlayer, VegetationOrstructures, Player, VegetationAfterPlayer, StructuresAfterPlayer, VegetationOrstructuresAfterPlayer, 
                                Layer2ground,Layer2Structures, Layer2PropsBeforePlayer, playerLayer2, Layer2PropsAfterPlayer}

    public enum Layers { Layer1  , Layer2 };

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

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
