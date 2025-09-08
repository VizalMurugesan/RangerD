using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy : Character
{
    private bool IsAggroed;
    Vector3 SpawnPos;
    public float detectionRange;
    public float MaximumRange;
    public float AttackRange;
    public float intervalBetweenStates;
    public float AttackDuration;
    public float AttackCooldown;
    public float EXPtogive;
    public int ignoreVal;
    public enum EnemyStateEnum { Chilling, RunningToSpawn, Chasing, Attacking, None}
    EnemyStateEnum state = EnemyStateEnum.None;

    Coroutine moveCoroutine = null;

    public List<EnemyState> EnemyStates;

    
    public int CurrentPathOffset = 2;
   
    public void Start()
    {
        SpawnPos = transform.position;
        CurrentPath = new List<Node>();
        //STATES
        #region
        EnemyStates = new List<EnemyState>();
        EnemyState chilling = new EnemyState(ChillingReq,Idle,this, "chilling");
        EnemyState chasing = new EnemyState(ChasePlayerReq,ChasePlayer,this, "chasing");
        EnemyState Attacking = new EnemyState(AttackReq, Attack,this, "attacking");
        EnemyState runToSpawn = new EnemyState(RunToSpawnReq, RunToSpawn,this, "runningtospawn");
        EnemyStates.Add(chilling);
        EnemyStates.Add(chasing);
        EnemyStates.Add(runToSpawn);
        EnemyStates.Add(Attacking);
        #endregion
        
    }

    

    public EnemyState DecideState()
    {
        foreach (EnemyState state in EnemyStates)
        {
            if (state.CheckRequirement()) { return state; }
        }

        return null;
    }
    //STATE FUNCTIONS
    #region
    public bool ChillingReq()
    {
        if (IsAggroed) { return false; }
        if (!IsInSpawnPos()) {  return false; }
        if (IsPlayerWithinDetectionRange()) {  return false; }
        
        return true;
    }

    public void Idle()
    {
        if (state.Equals(EnemyStateEnum.Chilling)) { return; }
        state = EnemyStateEnum.Chilling;
    }
    
    public bool RunToSpawnReq()
    {
        return IsAggroed && IsPlayerOutOfRange() && !IsInSpawnPos();
    }

    public IEnumerator RunToSpawn()
    {
        IsAggroed = false;
        
        if (state.Equals(EnemyStateEnum.RunningToSpawn)) { yield break; }
        SetCurrentpathReservedToFalse();
        List<Node> path = Game.Instance.pathFinder.FindPath(transform.position, SpawnPos, this);
        if(path!= null)
        {
            state = EnemyStateEnum.RunningToSpawn;
            IsAggroed = false;
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move(0));
            SetCurrentpathReservedToFalse();
            CurrentPath = path;
            
        }
        else
        { 
            

        }

        yield return null;
    }

    public bool ChasePlayerReq()
    {
        
        if (IsPlayerWithinDetectionRange() && !IsAggroed) { return true; }
        else if ((state.Equals(EnemyStateEnum.Chasing) || state.Equals(EnemyStateEnum.None))
                && !IsPlayerOutOfRange() && !IsInAttackRange()) { return true; }
        return false;
    }
    public void ChasePlayer()
    {
        if ((IsPlayerOutOfRange() || IsInAttackRange()) && moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            state = EnemyStateEnum.None;
            

        }
        state = EnemyStateEnum.Chasing;
        IsAggroed = true;
        SetCurrentpathReservedToFalse();
        List<Node> path = Game.Instance.pathFinder.FindPath(transform.position, Game.Instance.player.GetPlayerPosition(), this);
        if (path != null)
        {
            
            if (moveCoroutine != null)
            {
                
                StopCoroutine(moveCoroutine);    
                
            }
            if (!gameObject.activeInHierarchy) { StopAllCoroutines(); }
            CurrentPath = path;
            moveCoroutine = StartCoroutine(Move(1));
            
            
            
        }
        else {  }
        
    }

    public bool AttackReq()
    {
        return IsAggroed && IsInAttackRange()&& !state.Equals(EnemyStateEnum.Attacking);
    }

    public IEnumerator Attack()
    {
        state = EnemyStateEnum.Attacking;
        Vector2 StartPos = transform.position;
        Vector2 localScale = transform.localScale;
        Vector2 NewScale = localScale * 1.25f;
        float t = 0f;
        while (t<AttackDuration)
        {
            transform.position = Vector2.Lerp(StartPos, Game.Instance.player.GetPlayerPosition(), t/AttackDuration);
            transform.localScale = Vector2.Lerp(localScale, NewScale, t/AttackDuration);
            t += Time.deltaTime;
            yield return null;
        }

        Vector2 NewStartPos = transform.position;
        t = 0f;
        while (t < AttackDuration)
        {
            transform.position = Vector2.Lerp(NewStartPos, StartPos, t / AttackDuration);
            transform.localScale = Vector2.Lerp( NewScale, localScale, t / AttackDuration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = StartPos;
        transform.localScale = localScale;

        yield return new WaitForSeconds(AttackCooldown);
        state = EnemyStateEnum.None;


    }
    

    bool IsInAttackRange()
    {
        return MathF.Abs(Vector2.Distance(Game.Instance.player.GetPlayerPosition(), transform.position)) <= AttackRange;
    }

    bool IsPlayerOutOfRange()
    {
        return MathF.Abs(Vector2.Distance(Game.Instance.player.transform.position, SpawnPos)) > MaximumRange;
    }

    bool IsPlayerWithinDetectionRange()
    {
        return MathF.Abs(Vector2.Distance(Game.Instance.player.transform.position, SpawnPos)) < detectionRange;
    }

    bool IsInSpawnPos()
    {
        return MathF.Abs(Vector2.Distance(transform.position, SpawnPos)) <= 0.5f;
    }

    #endregion

    void AddPathToCurrentPath(List<Node> path)
    {
        for(int i = ignoreVal; i<CurrentPath.Count; i++)
        {
            CurrentPath.RemoveAt(i);
        }
        CurrentPath.AddRange(path);
    }
    public void SetAggroTrue()
    {
        IsAggroed = true;
    }

    public void SetAggroFalse()
    {
        IsAggroed = false;
    }

    
}

#region
public class EnemyState
{
    Func<bool> RequirementMet;
    Action StateAction;
    MonoBehaviour enemy;
    public string Name;
    
    Func<IEnumerator> StateCoroutine;
    public EnemyState(Func<bool> RequirementMet, Action StateAction, MonoBehaviour enemy, string name)
    {
        this.RequirementMet = RequirementMet;
        this.StateAction = StateAction;
        this.enemy = enemy;
        Name = name;
    }

    public EnemyState(Func<bool> RequirementMet, Func<IEnumerator> StateCoroutine,MonoBehaviour enemy, string name)
    {
        this.RequirementMet = RequirementMet;
        this.StateCoroutine = StateCoroutine;
        this.enemy = enemy;
        Name = name;
        
    }

    public bool CheckRequirement()
    {
        return RequirementMet.Invoke();
    }

    public void StateActionInvoke()
    {
        if(StateCoroutine!= null)
        {
            enemy.StartCoroutine(StateCoroutine());
        }
        StateAction?.Invoke();
    }

    


}
#endregion