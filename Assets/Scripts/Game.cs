using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public static Game Instance;
    public Player player;

    public Texture2D DefaultCrossHair;
    public Texture2D AimStateCrossHair;

    public Vector2 CursorOffset;

    public EffectManagerScript EffectManager;

    public InventoryManager inventoryManager;

    public GameObject MainmenuPanel;

    public Vector4 AttackedColor;

    public enum SortingLayers { BaseGrass, BaseGround, path, VegetationBeforePlayer, StructuresBeforePlayer, VegetationOrstructures, Player, VegetationAfterPlayer, StructuresAfterPlayer, VegetationOrstructuresAfterPlayer, 
                                Layer2ground,Layer2Structures, Layer2PropsBeforePlayer, playerLayer2, Layer2PropsAfterPlayer}

    public enum Layers { Layer1  , Layer2 };

    GameObject NoticePanel;
    public TMP_Text NoticePanelText;

    public LevelManager levelManager;

    public LevelManager LevelManager;

    public PathFinder pathFinder;

    public Item[] items;

    public Coroutine stopCoroutine;

    public CameraController mainCamera;

    public CharcterLayerManager characterLayerManager;

    public TimeManager timeManager;

    
    

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

        if(inventoryManager == null)
        {
            inventoryManager = GameObject.Find("Inventory").GetComponent<InventoryManager>();
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        CursorOffset = new Vector2(AimStateCrossHair.width / 2f, AimStateCrossHair.height / 2f);

        NoticePanel = NoticePanelText.transform.parent.gameObject;

        levelManager = GetComponent<LevelManager>();


        

}

//MainMenu Methods
#region
public void QuitGame()
    {
        Application.Quit();
    }

    public void SetMainMenuActiveOrInactive()
    {
        if (MainmenuPanel.activeInHierarchy)
        {
            MainmenuPanel.SetActive(false);
            return;
        }
        MainmenuPanel.SetActive(true);
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
        if (!player.CanAtk())
        {
            return;
        }
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

    public bool IsAllCoordinatesLessThan(Vector3 vector, float threshold)
    {
        return Mathf.Abs(vector.x) <= threshold
            && Mathf.Abs(vector.y) <= threshold
            && Mathf.Abs(vector.z) <= threshold;
    }
    public bool IsAllCoordinatesLessThan(Vector2 vector, float threshold)
    {
        return Mathf.Abs(vector.x) <= threshold
            && Mathf.Abs(vector.y) <= threshold;
           
    }

    public void EnableNoticePanel(string Message)
    {
        NoticePanel.SetActive(true);
        NoticePanelText.text = Message;
        Time.timeScale = 0f;
    }

    public void DisableNoticePanel()
    {
        NoticePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    #endregion

    public void SpawnItem(Vector3 pos)
    {
        int rand = UnityEngine.Random.Range(0, items.Length);
        Instantiate(items[rand], pos, Quaternion.identity);
    }

    IEnumerator StopGameCoroutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
        stopCoroutine = null;
    }
    
    public void StopGame(float duration)
    {
        if(stopCoroutine== null)
        {
            stopCoroutine = StartCoroutine(StopGameCoroutine(duration));
        }
    } 

    public IEnumerator ChangePlayerColorToAttacked()
    {

        foreach(var rend in player.RendrList)
        {
            rend.color = AttackedColor;
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var rend in player.RendrList)
        {
            rend.color = new Vector4(1f, 1f, 1f, 1f);
        }
        yield break;
    }

    
}
