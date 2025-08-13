using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] float MovementSpeed;
    [SerializeField] float MovementSpeedInstance;
    [SerializeField] float MovementSpeedMultiplier;
    [SerializeField] float intervalBetweenAttack = 0.5f;
    [SerializeField] float intervalBeforeAttack = 0.15f;

    Vector2 movementInput = Vector2.zero;
    Rigidbody2D rb;

    Animator anim;

    public List<SpriteRenderer> RendrList;
    public SpriteRenderer rendr;

    public GameObject MainHand;
    public GameObject mainhandpivot;

    public Game.Layers PlayerLayer = Game.Layers.Layer1;

    public enum PlayerState { Idle, Attacking, Moving }
    public enum PlayerAttackType { Normal, Ability1, Ability2 }

    public Action Ability1;
    
    public Coroutine StateCoroutine;


    public PlayerState state = PlayerState.Idle;
    PlayerAttackType attackType = PlayerAttackType.Normal;

    public enum Direction {  Left, Right, Up , Down};
    public Direction PlayerDirection = Direction.Right;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        if (GetComponent<SpriteRenderer>() != null)
            rendr = GetComponent<SpriteRenderer>();

        int childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                RendrList.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
        }
    }

    private void Update()
    {
        //keys
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (attackType.Equals(PlayerAttackType.Ability2))
            {
                attackType = PlayerAttackType.Normal;
                Debug.Log("changed to" + attackType);
            }
            else
            {
                attackType = PlayerAttackType.Ability2;
                Debug.Log("changed to" + attackType);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (attackType.Equals(PlayerAttackType.Ability1))
            {
                attackType = PlayerAttackType.Normal;
            }
            else
            {
                attackType = PlayerAttackType.Ability1;
            }
        }

    }
    private void FixedUpdate()
    {
        float VelocityX = Input.GetAxis("Horizontal");
        float VelocityY = Input.GetAxis("Vertical");

        movementInput = new Vector2(VelocityX, VelocityY).normalized;




        if (movementInput != Vector2.zero )
        {
            anim.SetBool("IsRunning", true);
            state = PlayerState.Moving;
            anim.SetFloat("movementX", movementInput.x);
            anim.SetFloat("movementY", movementInput.y);
            Game.Instance.ChangeCursorToDefault();
            
            //Direction setting while moving
            if(!PlayerDirection.Equals(Direction.Right) && movementInput.x > 0) { FaceRight(); }
     
            else if(!PlayerDirection.Equals(Direction.Left) && movementInput.x < 0) { FaceLeft(); }

            

        }

        else
        {
            anim.SetBool("IsRunning", false);
            if (!state.Equals(PlayerState.Idle) && CanMove())
            {
                state = PlayerState.Idle;
                ChangeStateCoroutine(EnterIdleState());
                
            }

        }
        if (Input.GetKey(KeyCode.LeftShift) && MovementSpeedInstance==MovementSpeed)
        {
            MovementSpeedInstance *= MovementSpeedMultiplier;

        }
        else
        {
            MovementSpeedInstance = MovementSpeed;
        }

        rb.linearVelocity = movementInput * MovementSpeedInstance;


    }

    //Layer Methods
    #region

    public static string LayerToLayerName(Game.Layers layer)
    {
        return layer switch
        {
            Game.Layers.Layer1 => "Player",
            Game.Layers.Layer2 => "Layer2Player",
            _ => "Unknown"

        };
    }

    public void ChangePlayerLayer(Game.Layers layer)
    {
        string LayerName = LayerToLayerName(layer);

        if (rendr != null)
        {
            rendr.sortingLayerName = LayerName;
            MainHand.GetComponent<SpriteRenderer>().sortingLayerName = LayerName;
        }
        else
        {
            foreach (var rendrer in RendrList)
            {
                rendrer.sortingLayerName = LayerName;
                MainHand.GetComponent<SpriteRenderer>().sortingLayerName = LayerName;
            }
        }
    }
    #endregion


    //MainHand methods
    #region
    public void EnableMainHand()
    {
        MainHand.SetActive(true);
    }

    public void DisableMainHand()
    {
        MainHand.SetActive(false);
    }

    public void BringMainHandToFront()
    {
        MainHand.GetComponent<SpriteRenderer>().sortingOrder = 6;
    }

    public void SendMainHandToBack()
    {
        MainHand.GetComponent<SpriteRenderer>().sortingOrder = 4;
    }

    public void TurnMainHandandBody()
    {
        Vector3 MousePos = Game.Instance.GetCursorPosition();
        Vector3 MousePosPlayerPosDiff = MousePos - mainhandpivot.transform.position;

        if (MousePosPlayerPosDiff.magnitude < 0.2f)
        {
            return;
        }

        float angle = Mathf.Atan2(MousePosPlayerPosDiff.y, MousePosPlayerPosDiff.x) * Mathf.Rad2Deg;
        float finalAngle = 0f;

        if (PlayerDirection.Equals(Direction.Right))
        {
            MainHand.transform.parent.rotation = Quaternion.Euler(0f, 0f, angle);
            finalAngle = angle;
        }    

        else if (PlayerDirection.Equals(Direction.Left))
        {
            MainHand.transform.parent.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
            finalAngle = angle +180f;
        }
         
        TurnBodyAccordingToAngle(finalAngle);
        
    }
    #endregion


    //State Coroutines & methods
    #region
    public IEnumerator EnterIdleState()
    {
        
        state = PlayerState.Idle;
        movementInput = Vector2.zero;
        

        Game.Instance.ChangeCursorToCrossHair();

        while (true)
        {
            if(state.Equals(PlayerState.Idle))
            {
                TurnMainHandandBody();
            }

            else if (state.Equals(PlayerState.Moving))
            {
                yield break;
            }
            if (Input.GetMouseButtonDown(0) && !IsStateAttacking())
            {
                
                ChangeStateCoroutine(EnterAttackState(MainHand.transform.rotation));
                
            }
            yield return null;
        }

        

    }

    public IEnumerator EnterAttackState(Quaternion mainHandRotation)
    {
         
        
        state = PlayerState.Attacking;
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(intervalBeforeAttack);
        Attack(MainHand.transform.rotation);
        yield return new WaitForSeconds(intervalBetweenAttack);
        
        ChangeStateCoroutine (EnterIdleState());
        
    }

    public void NormalATK(Quaternion handRotation)
    {
        Debug.Log("normal attack done");
        MainHand.GetComponent<ArrowSpawner>().SpawnArrow(handRotation, ArrowSpawner.ArrowType.normal);
    }

    public void Ability2(Quaternion handRotation)
    {
        Debug.Log("abilioty2 attack done");
        MainHand.GetComponent<ArrowSpawner>().SpawnArrow(handRotation, ArrowSpawner.ArrowType.ability2);
    }

    public void Attack(Quaternion handRotation)
    {
        
        if (attackType.Equals(PlayerAttackType.Normal)) { NormalATK(handRotation); }
        else if (attackType.Equals(PlayerAttackType.Ability1)) { Ability1.Invoke(); }
        else if (attackType.Equals(PlayerAttackType.Ability2)) { Ability2(handRotation); }
    }

    bool CanAtk()
    {
        return true;
    }

    bool CanMove()
    {
        if (state.Equals(PlayerState.Attacking)) { return false; } ;
        return true;
    }

    public void ChangeStateCoroutine(IEnumerator coroutine)
    {
        if (StateCoroutine != null)
        {
            StopCoroutine(StateCoroutine);
        }

        StateCoroutine = StartCoroutine(coroutine);
    }


    #endregion

    //helper methods
    #region
    public void TurnBodyAccordingToAngle(float angle)
    {

        if (PlayerDirection.Equals(Direction.Left))
        {
            if (180f> angle && angle> 0f)
            {
                anim.SetFloat("movementY", -1f);
                BringMainHandToFront();
                FaceRight();
            }

            else
            {
                anim.SetFloat("movementY", 1f);
                SendMainHandToBack();
                FaceLeft();
            }
        }

        else
        {
            if (angle > 0f)
            {
                anim.SetFloat("movementY", 1f);
                SendMainHandToBack();
                FaceLeft();
            }

            else
            {
                anim.SetFloat("movementY", -1f);
                BringMainHandToFront();
                FaceRight();
            }
        }
        

    }

    public bool IsStateIdle()
    {
        return state.Equals(PlayerState.Idle);
    }

    public bool IsStateAttacking()
    {
        return state.Equals(PlayerState.Attacking);
    }

    public void FaceLeft()
    {
        PlayerDirection = Direction.Left;
        Vector3 CurrScale = transform.localScale;
        transform.localScale = new Vector3(-1f, CurrScale.y, CurrScale.z);
        
    }
    public void FaceRight()
    {
        PlayerDirection = Direction.Right;
        Vector3 CurrScale = transform.localScale;
        transform.localScale = new Vector3(1f, CurrScale.y, CurrScale.z);
        
    }
    #endregion
}