using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] float MovementSpeed;
    Vector2 movementInput = Vector2.zero;
    Rigidbody2D rb;

    Animator anim;

    public List<SpriteRenderer> RendrList;
    public SpriteRenderer rendr;

    public GameObject MainHand;
    public GameObject mainhandpivot;

    public Game.Layers PlayerLayer = Game.Layers.Layer1;

    public enum PlayerState { Idle, Attacking, Moving }

    public PlayerState state = PlayerState.Idle;

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


    private void FixedUpdate()
    {
        float VelocityX = Input.GetAxis("Horizontal");
        float VelocityY = Input.GetAxis("Vertical");

        movementInput = new Vector2(VelocityX, VelocityY).normalized;




        if (movementInput != Vector2.zero)
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
            if (!state.Equals(PlayerState.Idle))
            {
                state = PlayerState.Idle;

                StartCoroutine(EnterIdleState());
            }

            

        }




        
        


        //rb.MovePosition((Vector2)transform.position + movementInput* MovementSpeed * Time.fixedDeltaTime);
        rb.linearVelocity = movementInput * MovementSpeed;
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


    //State Coroutines
    #region
    public IEnumerator EnterIdleState()
    {
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

            else if (state.Equals(PlayerState.Attacking))
            {
                yield return new WaitUntil(IsStateIdle);
            }
            if (Input.GetMouseButtonDown(0) && !IsStateAttacking())
            {
                StartCoroutine(EnterAttackState());
            }
            yield return null;
        }

        

    }

    public IEnumerator EnterAttackState()
    {
        state = PlayerState.Attacking;
        anim.SetTrigger("attack");
        MainHand.GetComponent<ArrowSpawner>().SpawnArrow();
        yield return new WaitForSeconds(0.7f);
        state = PlayerState.Idle;
        
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