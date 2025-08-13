using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance;
    public Player player;

    public Texture2D DefaultCrossHair;
    public Texture2D AimStateCrossHair;

    public Vector2 CursorOffset;

    public EffectManagerScript EffectManager;

    public enum SortingLayers { BaseGrass, BaseGround, path, VegetationBeforePlayer, StructuresBeforePlayer, VegetationOrstructures, Player, VegetationAfterPlayer, StructuresAfterPlayer, VegetationOrstructuresAfterPlayer, 
                                Layer2ground,Layer2Structures, Layer2PropsBeforePlayer, playerLayer2, Layer2PropsAfterPlayer}

    public enum Layers { Layer1  , Layer2 };

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if(Instance!= this)
        {
            Destroy(gameObject);
            return;
            
        }

        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        CursorOffset = new Vector2(AimStateCrossHair.width / 2f, AimStateCrossHair.height / 2f);
    }

    //MainMenu Methods
    #region
    public void QuitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        SceneManager.LoadSceneAsync("Scene1");
    }
    #endregion

    //CrossHairMethods
    #region
    public void ChangeCursorToCrossHair()
    {
        Cursor.SetCursor(AimStateCrossHair, CursorOffset, CursorMode.Auto);
        
    }

    public void ChangeCursorToDefault()
    {
        Cursor.SetCursor(DefaultCrossHair, Vector2.zero, CursorMode.Auto);
    }

    public Vector3 GetCursorPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
    }
    #endregion

    //helper methods
    #region
    public Vector2 UpScaleNormalize(Vector2 vector)
    {
        if (vector == Vector2.zero)
            return Vector2.zero;

        while (vector.magnitude <= 0.5f)
        {
            vector *= 1.1f; // multiply by a factor > 1 each time
        }

        return vector;
    }
    #endregion
}
