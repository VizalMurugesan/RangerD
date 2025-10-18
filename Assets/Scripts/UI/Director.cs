using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Director : MonoBehaviour
{
    public static Director Instance;

    List<Horde> Hordes;
    List<Enemy> LastHordeSpawned;
    [Header("Enemy Lists For Object Pooling")]
    public Enemy[] EnemiesType1;
    public Enemy[] EnemiesType2;

    public List<Enemy> AllInActiveEnemies;

    [Header("Float Variables")]
    public float IntervalBetweenDecisions;
    public float PressureStateCooldownTimer;
    public float DecisionCooldownTimer;
    public float TotalTime;
    public float PeakPressure;
    public float CurrentPressure;
    public float MinimalPressureMultiplier;
    public float SpawnDistance;
    

    //Durations
    public float BuildPressureDuration;
    public float MaintainPeakPressureDuration;
    public float ReducePressureToMinimalDuration;
    public float MaintainMinimalPressureDuration;



    List<PressureState> PressureStates;
    PressureState CurrentState;
    PressureState DefaultState;
    BTNode RootNode;

    enum StateEnum { BuildingPressure, MaintainingPeakPressure, ReducingPressureToMinimal, MaintainingMinimalPressure}
    StateEnum State;

    [Header("Temporary Variables that Game Manager should have")]
    Vector3 PlayerDirection;
    enum Difficulty { Easy, Normal, Hard, NightMare }
    [SerializeField]Difficulty difficulty;


    [Header("Debug")]
    public float CohesiveStrength;
    public float SeperationStrength;
    public float SeperationRadius;
    public TextMeshProUGUI StateText;
    //public TextMeshProUGUI Pressure;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (Instance != this)
        {
            Destroy(gameObject);
            return;

        }

        
        Hordes = new List<Horde>();
        PressureStates = new List<PressureState>();
        AllInActiveEnemies = new List<Enemy>();
        PeakPressure = PeakPressure * GetDifficultyMultiplier();
        InitializePressureStates();
        CurrentState = DefaultState;
        Debug.Log("State Set to " + CurrentState.Name);
        State = ChangeStateEnumAccordingToCurrent(CurrentState.Name);
        AllInActiveEnemies.AddRange(EnemiesType1);
        AllInActiveEnemies.AddRange(EnemiesType2);
    }

    public void FixedUpdate()
    {
        RemoveInActiveDeadEnemies();
        CurrentPressure = CheckForCurrrentPressure();

        //Rudimentary Decision Handling (will substitute DT/BT later)
        if (DecisionCooldownTimer > IntervalBetweenDecisions)
        {
            
            //Debug.Log("Current Pressure Before Update" + CurrentPressure);
            DecisionCooldownTimer = 0f;
            CurrentState.InvokeActionMethod();
            GetHordeLeadersToChasePlayer();
            CurrentPressure = CheckForCurrrentPressure();
            //CheckForMerges();

        }

        if (PressureStateCooldownTimer >= CurrentState.Duration)
        {
            ChangeToNextState();
        }



        HandleHordeFlocking();

        TotalTime += Time.fixedDeltaTime;
        DecisionCooldownTimer += Time.fixedDeltaTime;
        PressureStateCooldownTimer += Time.fixedDeltaTime;
    }


    // HordeMethods
    #region
    public void SpawnHorde(List<Enemy> enemies, float CohesiveStrength, float SeperationStrength, float SeperationRadius, Vector2 SpawnPos)
    {
        Horde horde = new Horde(enemies, CohesiveStrength, SeperationStrength, SeperationRadius);
        Hordes.Add(horde);
        horde.SetHordeActive(SpawnPos);
    }

    public void HandleHordeFlocking()
    {
        foreach (Horde horde in Hordes)
        {
            Dictionary<GameObject, Vector3> flockDir = horde.GetFlockToPosOfEntireHorde();
            //Debug.Log("Setting Flocking direction for " + Hordes.IndexOf(horde));
            foreach (var pair in flockDir)
            {
                pair.Key.GetComponent<Rigidbody2D>().linearVelocity = pair.Value;
            }

        }
    }

    void RemoveInActiveDeadEnemies()
    {
        for (int i =0;i<Hordes.Count; i++)
        {
            Hordes[i].RemoveInActiveEnemies();        
            
        }

        Hordes = Hordes.Where(h => (h.enemies != null && h.enemies.Count > 0)).ToList();
        AddingInActiveEnemiesBackIntoTheArray();
    }

    float CheckForCurrrentPressure()
    {
        float val = 0f;
        foreach(Horde horde in Hordes)
        {
            val += horde.GetHordePressureValue();
        }
        return val;
    }

    List<Enemy> CalculateHordeToSpawn(float Pressure)
    {
        Debug.Log("Calculating Horde To Spawn For " + Pressure);
        List<Enemy>horde = new List<Enemy>();
        float TotalPressure = 0f;
        int random = 0;
        int iterations = 0;

        while(TotalPressure < Pressure && iterations<=100)
        {
            if(AllInActiveEnemies.Count == 0) { return horde; }

            random = UnityEngine.Random.Range(0, AllInActiveEnemies.Count);
            Enemy RandomEnemy = AllInActiveEnemies[random];

            //I am Applying a softcap here.
            if (TotalPressure + RandomEnemy.PressureValue > Pressure * 1.2f)
            {
                iterations++;
                continue;
            }
            horde.Add(RandomEnemy);
            AllInActiveEnemies.Remove(RandomEnemy);
            
            TotalPressure += RandomEnemy.PressureValue;
            iterations++;
        }


        return horde;
    }

    Vector3 CalculatePositionToSpawn()
    {
        Vector3 SpawnPosOffset = Vector2.zero;
        if (Hordes == null || Hordes.Count == 0)
        {
           SpawnPosOffset =  UnityEngine.Random.insideUnitCircle.normalized * SpawnDistance;

        }

        else
        {
            Vector3 avgHordePos = Vector3.zero;
            foreach (Horde horde in Hordes)
            {
                if (horde.Leader != null)
                {
                    avgHordePos += horde.Leader.transform.position;
                }
            }
            avgHordePos /= Hordes.Count;
            Vector3 pointOfEngagement = (Game.Instance.player.transform.position + avgHordePos) / 2f;
            Vector3 dirToEngagement = (pointOfEngagement - Game.Instance.player.transform.position).normalized;

            SpawnPosOffset = -dirToEngagement * SpawnDistance;
        }
        Vector3 playerPos = Game.Instance.player.GetPlayerPosition();
        //Debug.Log("PlayerPos: " + playerPos + ", SpawnPos: " + playerPos+SpawnPosOffset);
        if(!Game.Instance.pathFinder.IsCordOutttaWorld(playerPos + SpawnPosOffset))
        {
            List<Node> path = Game.Instance.pathFinder.FindPath(playerPos, playerPos + SpawnPosOffset);
            if (path != null && path.Count > 0) { return playerPos + SpawnPosOffset; }
        }
        return Game.Instance.player.transform.position + new Vector3(-8f, -8f, 0);

    }

    void GetHordeLeadersToChasePlayer()
    {
        if (Hordes.Count == 0 ) { }
        else
        {
            foreach (Horde horde in Hordes)
            {
                if(horde.enemies.Count!=0) horde.GetLeader().ChasePlayer();
            }
        }
    }

    void CheckForMerges()
    {
        bool merge = false;
        foreach(Horde horde in Hordes)
        {
            foreach(Horde other in Hordes)
            {
                if (other == null || other == horde) continue;
                float distance = Vector3.Distance(horde.Leader.transform.position, other.Leader.transform.position);
                if (!IsPlayerBetweenTwoHordes(horde, other))
                {
                    horde.MergeHorde(other);
                    Hordes.Remove(other);
                    Debug.Log("MergedHorde");
                    merge = true;
                    break;
                }
            }
            if (merge) { break; }
        }
    }

    bool IsPlayerBetweenTwoHordes(Horde horde, Horde other)
    {
        Vector2 FirstHordePos = horde.Leader.transform.position;
        Vector2 SecondHordePos = other.Leader.transform.position;
        float DistanceBetweenHordes = Vector2.Distance(FirstHordePos, SecondHordePos);
        
        Vector2 PlayerPos = Game.Instance.player.GetPlayerPosition();
        float DistanceBetweenFirstAndPlayer = Vector2.Distance(FirstHordePos, PlayerPos);
        float DistanceBetweenSecondAndPlayer = Vector2.Distance(SecondHordePos, PlayerPos);

        //Horizontal Between Handling
        if ((SecondHordePos.x < PlayerPos.x && PlayerPos.x < FirstHordePos.x ||
           SecondHordePos.x > PlayerPos.x && PlayerPos.x > FirstHordePos.x) &&
            (DistanceBetweenHordes< DistanceBetweenFirstAndPlayer && DistanceBetweenHordes< DistanceBetweenSecondAndPlayer))
        {
            return true;
        }

        //Vertical Between Handling
        if ((SecondHordePos.y < PlayerPos.y && PlayerPos.y < FirstHordePos.y ||
           SecondHordePos.y > PlayerPos.y && PlayerPos.y > FirstHordePos.y) &&
            (DistanceBetweenHordes < DistanceBetweenFirstAndPlayer && DistanceBetweenHordes < DistanceBetweenSecondAndPlayer))
        {
            return true;
        }


        return false;
    }
    #endregion


    // PressureStateMethods
    #region
    void ChangeToNextState()
    {
        PressureStateCooldownTimer = 0f;
        CurrentState = CurrentState.NextState;
        CurrentState.ResetMaxPressure();
        Debug.Log("Changed State to " + CurrentState.Name);
        StateText.text = CurrentState.Name;
        State = ChangeStateEnumAccordingToCurrent(CurrentState.Name);
        
    }

    StateEnum ChangeStateEnumAccordingToCurrent(string StateName)
    {

        Debug.Log("Changed StateEnumTo" + StateName);
        switch(StateName)
        {
            case "BuildPressureState":
                return StateEnum.BuildingPressure;
                
            case "MaintainPeakPressureState":
                return StateEnum.MaintainingPeakPressure;
                
            case "ReducePressureToMinimalState":
                return StateEnum.ReducingPressureToMinimal;

            case "MaintainMinimalPressureState":
                return StateEnum.MaintainingMinimalPressure;
            default:
                return StateEnum.BuildingPressure;

        }
    }

    bool CanAddPressure()
    {
        Debug.Log("Currentstate max pressure : " + CurrentState.MaxPressure);
        if (CurrentState.MaxPressure-CurrentPressure>=299f) { return true; }
        return false;
    }

    //NoPressure
    public bool ShouldThereBeNoPressure()
    {
        return true;
    }

    public void SetPressureToZero()
    {

    }

    

    //BuildPressure
    public bool CanStartBuildingPressure()
    {
        if (State == StateEnum.BuildingPressure) { return true; }

        return false;
        
    }

    public void BuildPressure()
    {
        if (!CanAddPressure()) { /**Debug.Log("Cannot Add More Pressure");**/ SetBuildPressureForNextTic();  return; }
        float PressureDeficit = CurrentState.MaxPressure - CurrentPressure;
        if(PressureDeficit!= 300f) { PressureDeficit = Mathf.Max(300, CurrentState.MaxPressure - CurrentPressure); }
        
        List<Enemy> HordeToSpawn = CalculateHordeToSpawn(PressureDeficit);

        SetBuildPressureForNextTic();

        if (HordeToSpawn==null || HordeToSpawn.Count ==0) { Debug.Log("HordeToReturn was null"); return; }
        Vector2 SpawnPos = CalculatePositionToSpawn();

        SpawnHorde(HordeToSpawn, CohesiveStrength, SeperationStrength, SeperationRadius, SpawnPos);
        LastHordeSpawned = HordeToSpawn;

        
        

    }
    void SetBuildPressureForNextTic()
    {
        float PressureToSetForNextTic = Mathf.Min(PeakPressure, CurrentState.MaxPressure + (PeakPressure - PeakPressure * MinimalPressureMultiplier) * IntervalBetweenDecisions / CurrentState.Duration);
        CurrentState.SetMaxPressure(PressureToSetForNextTic);
        Debug.Log("SetMaxPressure of current state to " + PressureToSetForNextTic);
    }

    

    //MaintainPeakPressure
    public bool ShouldMaintainPeakPressure()
    {
        if (State == StateEnum.MaintainingPeakPressure) { return true; }

        return false;
    }

    public void MaintainPeakPressure()
    {
        if (!CanAddPressure()) { return; }
        float PressureOfHordeToSpawn = Mathf.Max(300, CurrentState.MaxPressure - CurrentPressure);
        List<Enemy> HordeToSpawn = CalculateHordeToSpawn(PressureOfHordeToSpawn);
        if (HordeToSpawn == null || HordeToSpawn.Count == 0) { Debug.Log("HordeToReturn was null"); return; }
        Vector2 SpawnPos = CalculatePositionToSpawn();

        SpawnHorde(HordeToSpawn, CohesiveStrength, SeperationStrength, SeperationRadius, SpawnPos);
        LastHordeSpawned = HordeToSpawn;
    }

    

    //LetThePressureGoDownToMinimal
    public bool ShouldLetThePressureGoDownToMinimal()
    {
        if (State == StateEnum.ReducingPressureToMinimal) { return true; }

        return false;
    }

    public void LetThePressureGoDownToMinimal()
    {
        if (!CanAddPressure()) { /**Debug.Log("Cannot Add More Pressure");**/ SetReductionPressureForNextTic(); return; }
        float PressureDeficit = CurrentState.MaxPressure - CurrentPressure;
        if (PressureDeficit != 300f) { PressureDeficit = Mathf.Max(300, CurrentState.MaxPressure - CurrentPressure); }

        List<Enemy> HordeToSpawn = CalculateHordeToSpawn(PressureDeficit);

        SetReductionPressureForNextTic();

        if (HordeToSpawn == null || HordeToSpawn.Count == 0) { Debug.Log("HordeToReturn was null"); return; }
        Vector2 SpawnPos = CalculatePositionToSpawn();

        SpawnHorde(HordeToSpawn, CohesiveStrength, SeperationStrength, SeperationRadius, SpawnPos);
        LastHordeSpawned = HordeToSpawn;
    }
    
    void SetReductionPressureForNextTic()
    {
        float PressureToSetForNextTic = Mathf.Max(PeakPressure*MinimalPressureMultiplier, CurrentState.MaxPressure - (PeakPressure - PeakPressure * (1-MinimalPressureMultiplier)) * IntervalBetweenDecisions / CurrentState.Duration);
        CurrentState.SetMaxPressure(PressureToSetForNextTic);
        Debug.Log("SetMaxPressure of current state to " + PressureToSetForNextTic);
    }

    //MaintainMinimalPressure

    public bool ShouldMaintainMinimalPressure()
    {
        if (State == StateEnum.MaintainingMinimalPressure) { return true; }

        return false;
    }

    public void MaintainMinimalPressure()
    {
        if (!CanAddPressure()) { return; }
        float PressureOfHordeToSpawn = Mathf.Max(300, CurrentState.MaxPressure - CurrentPressure);
        List<Enemy> HordeToSpawn = CalculateHordeToSpawn(PressureOfHordeToSpawn);
        if (HordeToSpawn == null || HordeToSpawn.Count == 0) { Debug.Log("HordeToReturn was null"); return; }
        Vector2 SpawnPos = CalculatePositionToSpawn();

        SpawnHorde(HordeToSpawn, CohesiveStrength, SeperationStrength, SeperationRadius, SpawnPos);
        LastHordeSpawned = HordeToSpawn;
    }
    

    void InitializePressureStates()
    {
        
        PressureState BuildPressureState = new PressureState("BuildPressureState",BuildPressureDuration, PeakPressure * MinimalPressureMultiplier, null, CanStartBuildingPressure, BuildPressure);
        PressureState MaintainPeakPressureState = new PressureState("MaintainPeakPressureState",MaintainPeakPressureDuration,PeakPressure , null, ShouldMaintainPeakPressure, MaintainPeakPressure);
        PressureState ReducePressureToMinimalState = new PressureState("ReducePressureToMinimalState",ReducePressureToMinimalDuration, PeakPressure , null, ShouldLetThePressureGoDownToMinimal, LetThePressureGoDownToMinimal);
        PressureState MaintainMinimalPressureState = new PressureState("MaintainMinimalPressureState",MaintainMinimalPressureDuration, PeakPressure * MinimalPressureMultiplier, null, ShouldMaintainMinimalPressure, MaintainMinimalPressure);
        

        MaintainMinimalPressureState.SetNextState(BuildPressureState);
        BuildPressureState.SetNextState(MaintainPeakPressureState);
        MaintainPeakPressureState.SetNextState(ReducePressureToMinimalState);
        ReducePressureToMinimalState.SetNextState(MaintainMinimalPressureState);

        
        PressureStates.Add(BuildPressureState);
        PressureStates.Add(ReducePressureToMinimalState);
        PressureStates.Add(MaintainMinimalPressureState);
        PressureStates.Add(MaintainPeakPressureState);

        DefaultState = BuildPressureState;

        Debug.Log("Initialized Pressure States");
    }

    #endregion
    // Helper/Extra Methods
    #region
    float GetDifficultyMultiplier()
    {
        float val = 1f;
        switch(difficulty)
        {
            case Difficulty.Easy:
                val = 1f;
                break;
            case Difficulty.Normal:
                val = 2f;
                break;
            case Difficulty.Hard:
                val = 3f;
                break;
            case Difficulty.NightMare:
                val = 4f;
                break;
            default:
                val = 1f;
                break;

        }
        return val;
    }

    void AddingInActiveEnemiesBackIntoTheArray()
    {
        foreach(Enemy enemy in EnemiesType1)
        {
            if(!enemy.gameObject.activeInHierarchy && !AllInActiveEnemies.Contains(enemy)) { AllInActiveEnemies.Add(enemy); }
        }
        foreach (Enemy enemy in EnemiesType2)
        {
            if (!enemy.gameObject.activeInHierarchy && !AllInActiveEnemies.Contains(enemy)) { AllInActiveEnemies.Add(enemy); }
        }
    }

    #endregion
}


//Behavior Tree
#region
public abstract class BTNode
{
    public abstract BTNode Evaluate();
}

public class ConditionNode : BTNode
{
    Func<bool> condition;
    BTNode TrueNode;
    BTNode FalseNode;

    public ConditionNode(Func<bool> condition, BTNode trueNode, BTNode falseNode)
    {
        this.condition = condition;
        TrueNode = trueNode;
        FalseNode = falseNode;
    }

    public override BTNode Evaluate()
    {
        if (condition.Invoke() == true) { return TrueNode; }
        else { return FalseNode; }
    }
}

public class ActionNode : BTNode
{
    Action action;

    public ActionNode(Action action)
    {
        this.action = action;
    }
    public override BTNode Evaluate()
    {
        action.Invoke();
        return this;
    }
}
#endregion