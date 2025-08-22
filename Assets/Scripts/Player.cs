using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("float Variables")]
    [SerializeField] float MovementSpeed;
    [SerializeField] float MovementSpeedInstance;
    [SerializeField] float RunSpeed;
    [SerializeField] float RunStaminaCost;
    [SerializeField] float intervalBetweenAttack = 0.5f;
    [SerializeField] float intervalBeforeAttack = 0.15f;

    Vector2 movementInput = Vector2.zero;

    [Header("Components")]
    Rigidbody2D rb;
    Animator anim;
    public List<SpriteRenderer> RendrList;
    public SpriteRenderer rendr;

    public GameObject MainHand;
    public GameObject mainhandpivot;
    PlayerHealth playerHealth;
    UIBar playerStamina;

    public Game.Layers PlayerLayer = Game.Layers.Layer1;

    [Header("Player Abilities")]
    public Action Ability1;
    public enum PlayerState { Idle, Attacking, Moving, Running }
    public enum PlayerAttackType { Normal, Ability1, Ability2 }
    public Image AttackIcon;
    public Image Ability2Icon;
    public Image AttackIconParent;
    public Image Ability2IconParent;
    public Sprite SelectedIconSprite;
    public Sprite defaultIconSprite;
    
    
    public Coroutine StateCoroutine;

    [Header("Player State And Enums")]
    public PlayerState state = PlayerState.Idle;
    PlayerAttackType attackType = PlayerAttackType.Normal;
    public enum Direction {  Left, Right};
    public Direction PlayerDirection;
    public enum Quadrant { first,  second, third , fourth};
    public Quadrant quadrant;



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

        AttackIconParent = AttackIcon.transform.parent.GetComponent<Image>();
        Ability2IconParent = Ability2Icon.transform.parent.GetComponent<Image>();

        playerHealth = GetComponent<PlayerHealth>();
        playerStamina = GetComponent<UIBar>();

    }

    private void Update()
    {
        //keys
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (attackType.Equals(PlayerAttackType.Ability2))
            {
                ChangeAbilityToNormal();
                //Debug.Log("changed to" + attackType);
            }
            else
            {
                ChangeAbilityToAbility2();
                //Debug.Log("changed to" + attackType);
            }
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            playerHealth.TakeDamage(20f);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            playerHealth.AddHealth(20f);
        }
        /**if (Input.GetKeyDown(KeyCode.Q))
        {
            if (attackType.Equals(PlayerAttackType.Ability1))
            {
                attackType = PlayerAttackType.Normal;
            }
            else
            {
                attackType = PlayerAttackType.Ability1;
            }
        }**/

        if( Input.GetKeyDown(KeyCode.G))
        {
            Game.Instance.EnableNoticePanel("hahahaha");
        }
        if (Input.GetMouseButtonDown(0) && !IsStateAttacking() &&CanAtk())
        {
            if (!MainHand.activeInHierarchy)
            {
                MainHand.SetActive(true);
            }
            TurnMainHandandBody();
            
            ChangeStateCoroutine(EnterAttackState(MainHand.transform.rotation, Game.Instance.GetCursorPosition()));

        }

    }
    private void FixedUpdate()
    {
        float VelocityX = Input.GetAxis("Horizontal");
        float VelocityY = Input.GetAxis("Vertical");

        movementInput = new Vector2(VelocityX, VelocityY).normalized;

        


        if (movementInput != Vector2.zero && CanMove())
        {
            anim.SetBool("IsRunning", true);
            state = PlayerState.Moving;
            anim.SetFloat("movementX", movementInput.x);
            anim.SetFloat("movementY", movementInput.y);
            Game.Instance.ChangeCursorToDefault();

            if (Input.GetKey(KeyCode.LeftShift) && CanMove() &&playerStamina.CurrentValue>1f)
            {
                state = PlayerState.Running;
                MovementSpeedInstance = RunSpeed;
                playerStamina.ReduceValue(RunStaminaCost);

            }
            else
            {
                state = PlayerState.Moving;
                MovementSpeedInstance = MovementSpeed;
            }

            //Direction setting while moving
            if (!PlayerDirection.Equals(Direction.Right) && movementInput.x > 0) { FaceRight(); }
     
            else if(!PlayerDirection.Equals(Direction.Left) && movementInput.x < 0) { FaceLeft(); }

            rb.linearVelocity = movementInput * MovementSpeedInstance;



        }

        else
        {
            anim.SetBool("IsRunning", false);
            rb.linearVelocity = Vector2.zero;
            if (!state.Equals(PlayerState.Idle) && CanMove())
            {
                state = PlayerState.Idle;
                ChangeStateCoroutine(EnterIdleState());
                
            }

        }
        

       


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
    public Quadrant GetQuadrant(float angle)
    {
        angle = angle % 360f;
        
        if (0f <= angle && angle < 90f) {  quadrant = Quadrant.first; return Quadrant.first; }
        else if (90f < angle && angle < 175f) {  quadrant = Quadrant.second; return Quadrant.second; }
        else if (-90f <= angle && angle < 0f) {  quadrant = Quadrant.third; return Quadrant.third; }
        else if(-181f<= angle && angle< -90f) {  quadrant = Quadrant.fourth; return Quadrant.fourth; }
        else { return quadrant; }
    }
    public void TurnMainHandandBody()
    {
        Vector3 MousePos = Game.Instance.GetCursorPosition();
        Vector3 MousePosPlayerPosDiff = MousePos - mainhandpivot.transform.position;
        
        if (Game.Instance.IsAllCoordinatesLessThan((Vector2)MousePosPlayerPosDiff,0.5f) )
        {
            return;
        }

        float angle = Mathf.Atan2(MousePosPlayerPosDiff.y, MousePosPlayerPosDiff.x) * Mathf.Rad2Deg;
        ChangeAccordingToQuadrant(angle);


    }

    public void ChangeAccordingToQuadrant(float angle)
    {
        Quadrant quadrant = GetQuadrant(angle);
        if (quadrant.Equals(Quadrant.first) || quadrant.Equals(Quadrant.second))
        {
            FaceLeft();
            anim.SetFloat("movementY", 1f);
            SetMainHandAngle(180f + angle);
        }

        else if (quadrant.Equals(Quadrant.third) || quadrant.Equals(Quadrant.fourth))
        {
            FaceRight();
            anim.SetFloat("movementY", -1f);
            SetMainHandAngle(angle);
        }

        
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
            if(state.Equals(PlayerState.Idle) && CanAtk())
            {
                TurnMainHandandBody();
            }

            else if (state.Equals(PlayerState.Moving))
            {
                yield break;
            }
            if (Input.GetMouseButtonDown(0) && !IsStateAttacking())
            {
                
                ChangeStateCoroutine(EnterAttackState(MainHand.transform.rotation, Game.Instance.GetCursorPosition()));
                
            }
            yield return null;
        }

        

    }

    public IEnumerator EnterAttackState(Quaternion mainHandRotation, Vector3 TargetPos)
    {
        anim.SetTrigger("attack");
        rb.linearVelocity = Vector2.zero;
        state = PlayerState.Attacking;
        
        yield return new WaitForSeconds(intervalBeforeAttack);
        Attack(MainHand.transform.rotation, TargetPos);
        yield return new WaitForSeconds(intervalBetweenAttack);
        
        ChangeStateCoroutine (EnterIdleState());
        
    }

    public void NormalATK(Quaternion handRotation, Vector3 targetPos)
    {
        
        MainHand.GetComponent<ArrowSpawner>().SpawnArrow(handRotation, ArrowSpawner.ArrowType.normal, targetPos);
    }

    public void Ability2(Quaternion handRotation, Vector3 targetPos)
    {
        
        MainHand.GetComponent<ArrowSpawner>().SpawnArrow(handRotation, ArrowSpawner.ArrowType.ability2, targetPos );
    }

    public void Attack(Quaternion handRotation, Vector3 TargetPos)
    {
        

        if (attackType.Equals(PlayerAttackType.Normal)) { NormalATK(handRotation, TargetPos); }
        else if (attackType.Equals(PlayerAttackType.Ability1)) { Ability1.Invoke(); }
        else if (attackType.Equals(PlayerAttackType.Ability2)) { Ability2(handRotation, TargetPos); }
    }

    public bool CanAtk()
    {
        if (Game.Instance.inventoryManager.InventoryActive) { return false; }
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

    public void SetMainHandAngle(float Angle)
    {
        MainHand.transform.parent.rotation = Quaternion.Euler(0f, 0f, Angle);
    }
    #endregion
    //ATK helper methods
    #region
    public void ChangeAbilityToAbility1()
    {

    }
    public void ChangeAbilityToAbility2()
    {
        ResetAllAbilities();
        attackType = PlayerAttackType.Ability2;
        Ability2IconParent.rectTransform.localScale = new Vector3(1.2f, 1.2f, 1f);
        Ability2IconParent.sprite = SelectedIconSprite;

    }
    public void ChangeAbilityToNormal()
    {
        ResetAllAbilities();
        attackType = PlayerAttackType.Normal;
        AttackIconParent.rectTransform.localScale = new Vector3(1.2f, 1.2f, 1f);
        AttackIconParent.sprite = SelectedIconSprite;

    }

    public void ResetAllAbilities()
    {
        AttackIconParent.rectTransform.localScale = Vector3.one;
        Ability2IconParent.rectTransform.localScale = Vector3.one;
        Ability2IconParent.sprite = defaultIconSprite;
        AttackIconParent.sprite = defaultIconSprite;

    }
    #endregion
}