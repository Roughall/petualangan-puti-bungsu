using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Defensive,
        Flee
    }

    [Header("Current State")]
    public EnemyState currentState = EnemyState.Idle;

    [Header("References")]
    private EnemyTarget target;
    private EnemyFuzzy fuzzy;
    private Rigidbody2D rb;
    private EnemyPatrol patrol;
    private Health health;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Patrol")]
    [SerializeField] private bool usePatrol = true;

    [Header("Defensive")]
    [SerializeField] private float defensiveDistance = 3f;
    [SerializeField] private float defensiveDetectionDistance = 5f;

    [Header("Flee")]
    [SerializeField] private float fleeHealthRatio = 0.5f;
    [SerializeField] private float fleeSafeDistance = 8f;
    [SerializeField] private float fleeCooldown = 3f;
    private float fleeCooldownTimer = 0f;

    [Header("Recovery")]
    [SerializeField] private float recoveryStartDelay = 10f;
    [SerializeField] private float recoveryHealthRatio = 0.6f;
    [SerializeField] private float recoveryAmountPerSecond = 1f;
    private bool isRecovering = false;
    private bool isRecoveryWaiting = false;
    private float recoveryWaitTimer = 0f;
    private float recoveryHealTimer = 0f;
    private string recoveryReason = "";
    private RecoveryPoint currentRecoveryPoint;
    private bool isHiddenRecovery = false;
    private RecoveryPoint bestRecoveryPoint;

    [Header("Cover")]
    [SerializeField] private bool useCover = true;

    [SerializeField] private float coverSearchRadius = 15f;

    [SerializeField] private LayerMask coverLayer;

    [SerializeField] private float coverReachedDistance = 1.5f;
    
    [SerializeField] private float coverOffset = 0.4f;
    private EnemyCoverPoint currentCover;
    private EnemyCoverPoint targetCover;
    private bool hasCoverTarget = false;
    private bool coverSearchAttempted = false;

    [Header("World Boundary")]
    [SerializeField] private bool useWorldBoundary = true;
    [SerializeField] private float minX = 0.6f;

    [SerializeField] private float maxX = 75.5f;

    [SerializeField] private float minY = 0.5f;

    [SerializeField] private float maxY = 41.6f;

    [SerializeField] private float boundaryMargin = 1f;
    private enum EnemyDecision
    {
        Normal,
        Attack,
        Defensive,
        Flee
    }
    void Awake()
    {
        target = GetComponent<EnemyTarget>();
        fuzzy = GetComponent<EnemyFuzzy>();
        rb = GetComponent<Rigidbody2D>();
        patrol = GetComponent<EnemyPatrol>();
        health = GetComponent<Health>();

        Debug.Log("========== EnemyBrain ==========");
        Debug.Log("EnemyBrain Ready : " + gameObject.name);

        if (target == null)
            Debug.LogError("EnemyTarget tidak ditemukan pada " + gameObject.name);

        if (fuzzy == null)
            Debug.LogWarning("EnemyFuzzy tidak ditemukan pada " + gameObject.name);

        if (rb == null)
            Debug.LogError("Rigidbody2D tidak ditemukan pada " + gameObject.name);

        if (patrol == null)
            Debug.LogWarning("EnemyPatrol tidak ditemukan pada " + gameObject.name);

        // Tentukan state awal di Awake.
        // Awake terbukti berjalan sebelum Update.
        if (usePatrol && patrol != null)
        {
            currentState = EnemyState.Patrol;

            Debug.Log(gameObject.name + " Initial State : Patrol");
        }
        else
        {
            currentState = EnemyState.Idle;

            Debug.Log(gameObject.name + " Initial State : Idle");
        }
    }

    void Start()
    {
        // Hanya satu tempat untuk menentukan state awal.
        if (usePatrol && patrol != null)
        {
            currentState = EnemyState.Patrol;

            Debug.Log(gameObject.name +" Initial State : Patrol");
        }
        else
        {
            currentState = EnemyState.Idle;

            Debug.Log(gameObject.name + " Initial State : Idle");
        }
    }
    void Update()
    {
        if (target == null)
            return;

        UpdateFSM();
    }
    private Vector2 ApplyWorldBoundary(Vector2 direction)
    {
        if (!useWorldBoundary)
            return direction;

        Vector3 pos = transform.position;

        bool blocked = false;

        if (pos.x <= minX + boundaryMargin &&
            direction.x < 0)
        {
            direction.x = 0;
            blocked = true;
        }

        if (pos.x >= maxX - boundaryMargin &&
            direction.x > 0)
        {
            direction.x = 0;
            blocked = true;
        }

        if (pos.y <= minY + boundaryMargin &&
            direction.y < 0)
        {
            direction.y = 0;
            blocked = true;
        }

        if (pos.y >= maxY - boundaryMargin &&
            direction.y > 0)
        {
            direction.y = 0;
            blocked = true;
        }

        if (blocked)
        {
            Debug.Log(
                gameObject.name +
                " [WORLD BOUNDARY BLOCK]" +
                " | Position = " + transform.position +
                " | Direction adjusted"
            );
        }

        if (direction.sqrMagnitude > 0.001f)
            direction.Normalize();

        return direction;
    }
    private bool IsNearWorldBoundary()
    {
        if (!useWorldBoundary)
            return false;

        Vector3 pos = transform.position;

        return
            pos.x <= minX + boundaryMargin ||
            pos.x >= maxX - boundaryMargin ||
            pos.y <= minY + boundaryMargin ||
            pos.y >= maxY - boundaryMargin;
    }
    private void ClampToWorldBoundary()
    {
        if (!useWorldBoundary)
            return;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(
            pos.x,
            minX,
            maxX
        );

        pos.y = Mathf.Clamp(
            pos.y,
            minY,
            maxY
        );

        transform.position = pos;
    }
    private EnemyDecision currentDecision = EnemyDecision.Normal;
    private EnemyDecision EvaluateFuzzyDecision(float distance)
    {
        if (fuzzy == null)
            return EnemyDecision.Normal;

        if (health == null)
            return EnemyDecision.Normal;

        if (isRecovering || isRecoveryWaiting)
            return EnemyDecision.Normal;

        float hpPercent =
            ((float)health.CurrentHP / health.MaxHP) * 100f;

        fuzzy.UpdateInput(
            hpPercent,
            distance
        );

        float aggressiveness = fuzzy.Evaluate();

        float fleeStrength = fuzzy.fleeStrength;

        float attackStrength = fuzzy.attackStrength;

        Debug.Log(
            gameObject.name +
            " [FUZZY DECISION]" +
            " | HP = " + hpPercent +
            "%" +
            " | Distance = " + distance +
            " | Aggressiveness = " + aggressiveness +
            " | FleeStrength = " + fleeStrength +
            " | AttackStrength = " + attackStrength
        );

        if (fleeStrength >= 0.50f)
        {
            return EnemyDecision.Flee;
        }

        if (fleeStrength >= 0.10f)
        {
            return EnemyDecision.Defensive;
        }

        if (distance <= target.attackRange &&
            attackStrength >= 0.60f)
        {
            return EnemyDecision.Attack;
        }

        return EnemyDecision.Normal;
    }
    void UpdateFSM()
    {
        float distance = target.DistanceToPlayer();

        currentDecision =
            EvaluateFuzzyDecision(distance);

        Debug.Log(
            gameObject.name +
            " [AI DECISION] = " +
            currentDecision
        );

        // =========================================
        // DECISION -> STATE
        // =========================================

        if (currentDecision == EnemyDecision.Flee)
        {
            if (currentState != EnemyState.Flee)
            {
                EnterFleeState();
            }
        }
        else if (currentDecision == EnemyDecision.Defensive)
        {
            if (currentState != EnemyState.Defensive)
            {
                EnterDefensiveState();
            }
        }

        // =========================================
        // FSM
        // =========================================

        Debug.Log(
            "FSM = " + currentState +
            " | Distance = " + distance +
            " | ChaseRange = " + target.chaseRange +
            " | AttackRange = " + target.attackRange
        );

        switch (currentState)
        {
            case EnemyState.Idle:

                StopMoving();

                // Untuk enemy yang menggunakan Patrol,
                // Idle tidak dipakai sebagai perilaku normal.
                if (usePatrol && patrol != null)
                {
                    currentState = EnemyState.Patrol;

                    Debug.Log(
                        gameObject.name +
                        " : Idle -> Patrol"
                    );

                    break;
                }

                if (distance <= target.chaseRange)
                {
                    currentState = EnemyState.Chase;

                    Debug.Log(
                        gameObject.name +
                        " : Idle -> Chase"
                    );
                }

                break;


            case EnemyState.Patrol:

                if (patrol == null)
                {
                    currentState = EnemyState.Idle;
                    break;
                }

                // =========================================
                // RECOVERY SEQUENCE
                // =========================================

                if (isRecoveryWaiting)
                {
                    StopMoving();

                    recoveryWaitTimer -= Time.deltaTime;

                    if (recoveryWaitTimer <= 0f)
                    {
                        recoveryWaitTimer = 0f;
                        isRecoveryWaiting = false;
                        isRecovering = true;

                        Debug.Log(
                            gameObject.name +
                            " [RECOVERY WAIT COMPLETE]"
                        );

                        Debug.Log(
                            gameObject.name +
                            " [RECOVERY START]"
                        );
                    }
                    else
                    {
                        Debug.Log(
                            gameObject.name +
                            " [RECOVERY COUNTDOWN] " +
                            Mathf.CeilToInt(recoveryWaitTimer) +
                            " detik"
                        );
                    }

                    break;
                }

                // =========================================
                // RECOVERY HP
                // =========================================

                if (isRecovering)
                {
                    StopMoving();

                    HandleRecovery();

                    if (!IsRecovering())
                    {
                        isRecovering = false;
                        isRecoveryWaiting = false;

                        recoveryWaitTimer = 0f;
                        recoveryHealTimer = 0f;

                        ExitRecoveryPoint();

                        currentState = EnemyState.Patrol;

                        Debug.Log(
                            gameObject.name +
                            " [RECOVERY COMPLETE]" +
                            " | HP >= " +
                            (recoveryHealthRatio * 100f) +
                            "%" 
                        );

                        Debug.Log(
                            gameObject.name +
                            " [RECOVERY -> PATROL]"
                        );

                        Debug.Log(
                            gameObject.name +
                            " [RECOVERY POSITION]" +
                            " | Enemy = " +
                            transform.position
                        );
                    }

                    break;
                }

                // =========================================
                // NORMAL PATROL
                // =========================================

                patrol.Patrol();

                Debug.Log(
                    gameObject.name +
                    " [POST RECOVERY PATROL]" +
                    " | Position = " +
                    transform.position +
                    " | Velocity = " +
                    rb.velocity
                    );

                if (distance <= target.chaseRange)
                {
                    patrol.StopPatrol();

                    currentState = EnemyState.Chase;

                    Debug.Log(
                        gameObject.name +
                        " [PATROL -> CHASE]"
                    );
                }

                break;

            case EnemyState.Chase:

                MoveToPlayer();

                // Flee memiliki prioritas tertinggi
                if (currentDecision ==
                    EnemyDecision.Flee)
                {
                    EnterFleeState();
                    break;
                }

                // Defensive memiliki prioritas berikutnya
                if (currentDecision ==
                    EnemyDecision.Defensive)
                {
                    EnterDefensiveState();
                    break;
                }

                // Attack hanya jika:
                // 1. cukup dekat
                // 2. Fuzzy mengizinkan Attack

                if (distance <= target.attackRange &&
                    currentDecision ==
                    EnemyDecision.Attack)
                {
                    StopMoving();

                    currentState =
                        EnemyState.Attack;

                    Debug.Log(
                        gameObject.name +
                        " [CHASE -> ATTACK]" +
                        " | AttackStrength = " +
                        fuzzy.attackStrength
                    );

                    break;
                }

                if (distance > target.chaseRange)
                {
                    StopMoving();

                    if (usePatrol &&
                        patrol != null)
                    {
                        currentState =
                            EnemyState.Patrol;

                        Debug.Log(
                            gameObject.name +
                            " [CHASE -> PATROL]"
                        );
                    }
                    else
                    {
                        currentState =
                            EnemyState.Idle;

                        Debug.Log(
                            gameObject.name +
                            " [CHASE -> IDLE]"
                        );
                    }
                }

                break;

            case EnemyState.Attack:

                StopMoving();

                // Flee paling tinggi
                if (currentDecision ==
                    EnemyDecision.Flee)
                {
                    EnterFleeState();
                    break;
                }

                // Defensive di bawah Flee
                if (currentDecision ==
                    EnemyDecision.Defensive)
                {
                    EnterDefensiveState();
                    break;
                }

                // Attack masih valid
                if (distance <= target.attackRange &&
                    currentDecision ==
                    EnemyDecision.Attack)
                {
                    // =================================
                    // ATTACK EXECUTION
                    // =================================
                    //
                    // Biarkan method attack Enemy
                    // yang sekarang tetap di sini.
                    //
                    // Jangan ubah mekanik serangan dulu.

                    Debug.Log(
                        gameObject.name +
                        " [ATTACK ACTIVE]" +
                        " | AttackStrength = " +
                        fuzzy.attackStrength
                    );

                    break;
                }

                // Attack tidak lagi valid
                if (distance > target.attackRange)
                {
                    currentState =
                        EnemyState.Chase;

                    Debug.Log(
                        gameObject.name +
                        " [ATTACK -> CHASE]"
                    );

                    break;
                }

                break;

            case EnemyState.Flee:

                // =========================================
                // STEP 1 - SEARCH COVER
                // =========================================

                if (useCover &&
                    !coverSearchAttempted &&
                    !hasCoverTarget)
                {
                    coverSearchAttempted = true;

                    Debug.Log(
                        gameObject.name +
                        " [COVER SEARCH]" +
                        " | Radius = " +
                        coverSearchRadius
                    );

                    targetCover = FindSafeCover();

                    if (targetCover != null)
                    {
                        hasCoverTarget = true;

                        Debug.Log(
                            gameObject.name +
                            " [COVER FOUND]" +
                            " | Target = " +
                            targetCover.name +
                            " | Position = " +
                            targetCover.transform.position
                        );
                    }
                    else
                    {
                        Debug.Log(
                            gameObject.name +
                            " [COVER NOT FOUND]" +
                            " | Fallback = Safe Distance"
                        );
                    }
                }

                // =========================================
                // STEP 2 - FLEE TO COVER
                // =========================================

                if (hasCoverTarget && targetCover != null)
                {
                    if (currentRecoveryPoint == null)
                    {
                        if (!PrepareRecoveryPoint())
                        {
                            Debug.Log(
                                gameObject.name +
                                " [RECOVERY TARGET FAILED]" +
                                " | Fallback = Normal Flee"
                            );

                            targetCover = null;
                            hasCoverTarget = false;

                            FleeFromPlayer();

                            break;
                        }
                    }

                    MoveToRecovery();

                    break;
                }

                // =========================================
                // STEP 3 - NORMAL FLEE FALLBACK
                // =========================================

                if (distance >= fleeSafeDistance)
                {
                    StopMoving();

                    recoveryReason = "SAFE_DISTANCE";

                    Debug.Log(
                        gameObject.name +
                        " [FLEE SAFE DISTANCE REACHED]" +
                        " | Distance = " +
                        distance +
                        " | SafeDistance = " +
                        fleeSafeDistance
                    );

                    StartRecovery();

                    break;
                }

                // =========================================
                // STEP 4 - FLEE FROM PLAYER
                // =========================================

                FleeFromPlayer();

                break;

            case EnemyState.Defensive:

                DefensiveMovement(distance);

                if (currentDecision == EnemyDecision.Flee)
                {
                    EnterFleeState();

                    break;
                }

                if (currentDecision == EnemyDecision.Normal)
                {
                    StopMoving();

                    if (usePatrol && patrol != null)
                    {
                        currentState = EnemyState.Patrol;

                        Debug.Log(
                            gameObject.name +
                            " [DEFENSIVE -> PATROL]"
                        );
                    }
                    else
                    {
                        currentState = EnemyState.Idle;

                        Debug.Log(
                            gameObject.name +
                            " [DEFENSIVE -> IDLE]"
                        );
                    }

                    break;
                }

                break;
        }

        if (fleeCooldownTimer > 0f)
        {
            fleeCooldownTimer -= Time.deltaTime;
        }
    }
    private void EnterFleeState()
    {
        StopMoving();

        if (patrol != null)
            patrol.StopPatrol();

        currentState = EnemyState.Flee;

        targetCover = null;
        hasCoverTarget = false;
        coverSearchAttempted = false;

        Debug.Log(
            gameObject.name +
            " [ANY STATE -> FLEE]"
        );

        // =================================
        // SEARCH SAFE COVER
        // =================================

        if (useCover)
        {
            Debug.Log(gameObject.name + " [FLEE ENTRY] " + "| HP = " +
            health.CurrentHP + "/" + health.MaxHP + " | Position = " + transform.position);

            Debug.Log(
                gameObject.name +
                " [COVER SEARCH]" +
                " | Radius = " +
                coverSearchRadius
            );

            targetCover = FindSafeCover();

            if (targetCover != null)
            {
                hasCoverTarget = true;

                Debug.Log(
                    gameObject.name +
                    " [COVER FOUND]" +
                    " | Target = " +
                    targetCover.name
                );

                if (!PrepareRecoveryPoint())
                {
                    Debug.LogWarning(
                        gameObject.name +
                        " [RECOVERY TARGET FAILED]" +
                        " | Cover tidak memiliki RecoveryPoint lengkap"
                    );

                    targetCover = null;
                    hasCoverTarget = false;
                }
            }
            else
            {
                Debug.Log(
                    gameObject.name +
                    " [COVER NOT FOUND]" +
                    " | Fallback = Safe Distance"
                );
            }
        }
        else
        {
            Debug.Log(
                gameObject.name +
                " [COVER DISABLED]"
            );
        }
    }
    private void EnterDefensiveState()
    {
        StopMoving();

        currentState = EnemyState.Defensive;

        Debug.Log(
            gameObject.name +
            " [ANY STATE -> DEFENSIVE]"
        );
    }
    bool FleeCompleted(float distance)
    {
        return distance >= fleeSafeDistance;
    }
    void MoveToPlayer()
    {
        if (target.player == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " : MoveToPlayer gagal, Player null!"
            );

            return;
        }

        if (rb == null)
        {
            Debug.LogError(
                gameObject.name +
                " : MoveToPlayer gagal, Rigidbody2D null!"
            );

            return;
        }

        Vector2 direction =
            (
                target.player.position -
                transform.position
            ).normalized;

        rb.velocity = direction * moveSpeed;
    }
    void StopMoving()
    {
        if (rb == null)
            return;

        rb.velocity = Vector2.zero;
    }
    void DefensiveMovement(float distance)
    {
        if (target == null || target.player == null)
        {
            StopMoving();
            return;
        }

        if (rb == null)
            return;

        if (distance < defensiveDistance)
        {
            Vector2 directionAway =
                (
                    transform.position -
                    target.player.position
                ).normalized;

            directionAway =
                ApplyWorldBoundary(directionAway);

            rb.velocity =
                directionAway * moveSpeed;

            Debug.Log(
                gameObject.name +
                " [DEFENSIVE MOVE]" +
                " | Distance = " + distance +
                " | TargetDistance = " + defensiveDistance +
                " | Velocity = " + rb.velocity
            );
        }
        else
        {
            StopMoving();

            Debug.Log(
                gameObject.name +
                " [DEFENSIVE HOLD]" +
                " | Distance = " + distance
            );
        }
    }
    bool ShouldFlee()
    {
        if (health == null)
            return false;

        if (isRecovering || isRecoveryWaiting)
            return false;

        float hpRatio =
            (float)health.CurrentHP /
            health.MaxHP;

        float hpPercent =
            hpRatio * 100f;

        float distance =
            target.DistanceToPlayer();

        fuzzy.UpdateInput(
            hpPercent,
            distance
        );

        float aggressiveness =
            fuzzy.Evaluate();

        Debug.Log(
            gameObject.name +
            " [FUZZY FLEE DECISION]" +
            " | HP = " + hpPercent +
            "%" +
            " | Distance = " + distance +
            " | Aggressiveness = " + aggressiveness +
            " | FleeStrength = " + fuzzy.fleeStrength
        );

        return fuzzy.fleeStrength >= 0.50f;
    }
    bool ShouldDefend()
    {
        if (health == null)
            return false;

        if (isRecovering || isRecoveryWaiting)
            return false;

        if (fuzzy == null)
            return false;

        float hpRatio =
            (float)health.CurrentHP /
            health.MaxHP;

        float hpPercent =
            hpRatio * 100f;

        float distance =
            target.DistanceToPlayer();

        fuzzy.UpdateInput(
            hpPercent,
            distance
        );

        float aggressiveness =
            fuzzy.Evaluate();

        float fleeStrength =
            fuzzy.fleeStrength;

        Debug.Log(
            gameObject.name +
            " [FUZZY DEFENSIVE DECISION]" +
            " | HP = " + hpPercent +
            "%" +
            " | Distance = " + distance +
            " | Aggressiveness = " + aggressiveness +
            " | FleeStrength = " + fleeStrength
        );

        return fleeStrength >= 0.10f &&
               fleeStrength < 0.50f &&
               distance <= defensiveDetectionDistance;
    }
    void FleeFromPlayer()
    {
        if (target == null || target.player == null)
        {
            StopMoving();
            return;
        }

        if (rb == null)
        {
            Debug.LogError(
                gameObject.name +
                " [FLEE] Rigidbody2D NULL!"
            );

            return;
        }

        Vector2 directionAway =
            (
                transform.position -
                target.player.position
            ).normalized;

        // ===========================
        // WORLD BOUNDARY CHECK
        // ===========================

        if (useWorldBoundary)
        {
            Vector3 pos = transform.position;

            if (pos.x <= minX + boundaryMargin &&
                directionAway.x < 0)
            {
                directionAway.x = 0;
            }

            if (pos.x >= maxX - boundaryMargin &&
                directionAway.x > 0)
            {
                directionAway.x = 0;
            }

            if (pos.y <= minY + boundaryMargin &&
                directionAway.y < 0)
            {
                directionAway.y = 0;
            }

            if (pos.y >= maxY - boundaryMargin &&
                directionAway.y > 0)
            {
                directionAway.y = 0;
            }

            directionAway.Normalize();
        }

        directionAway =
            ApplyWorldBoundary(directionAway);

        rb.velocity =
            directionAway * moveSpeed;

        Debug.Log(
            gameObject.name +
            " [FLEE MOVE]" +
            " | Pos = " + transform.position +
            " | Direction = " + directionAway +
            " | Velocity = " + rb.velocity
        );
    }
    bool IsRecovering()
    {
        if (health == null)
            return false;

        if (health.MaxHP <= 0)
            return false;

        float hpRatio =
            (float)health.CurrentHP /
            health.MaxHP;

        return hpRatio < recoveryHealthRatio;
    }
    void HandleRecovery()
    {
        if (health == null)
            return;

        recoveryHealTimer += Time.deltaTime;

        if (recoveryHealTimer >= 1f)
        {
            recoveryHealTimer = 0f;

            health.RecoverHealth(
                recoveryAmountPerSecond
            );

            Debug.Log(
                gameObject.name +
                " [RECOVERY TICK]"
            );
        }
    }
    private void StartRecovery()
    {
        StopMoving();

        hasCoverTarget = false;
        coverSearchAttempted = false;

        isRecoveryWaiting = true;
        isRecovering = false;

        recoveryWaitTimer = recoveryStartDelay;
        recoveryHealTimer = 0f;

        currentState = EnemyState.Patrol;

        Debug.Log( gameObject.name +
        " [RECOVERY WAIT START]" +
        " | Reason = " + recoveryReason +
        " | RecoveryPoint = " + (currentRecoveryPoint != null ? currentRecoveryPoint.name : "NULL") +
        " | Countdown = " + recoveryWaitTimer +" detik");
    }
    private void ExitRecoveryPosition()
    {
        Collider2D[] nearby =
            Physics2D.OverlapCircleAll(
                transform.position,
                0.5f,
                coverLayer
            );

        foreach (Collider2D collider in nearby)
        {
            if (collider == null)
                continue;

            Vector2 safePosition =
                collider.bounds.ClosestPoint(
                    transform.position
                );

            Vector2 direction =
                (
                    (Vector2)transform.position -
                    safePosition
                ).normalized;

            if (direction == Vector2.zero)
            {
                direction = Vector2.up;
            }

            float exitDistance = 0.6f;

            Vector2 newPosition =
                safePosition +
                direction * exitDistance;

            transform.position =
                newPosition;

            Debug.Log(
                gameObject.name +
                " [RECOVERY EXIT]" +
                " | Collider = " +
                collider.name +
                " | New Position = " +
                newPosition
            );

            break;
        }
    }
    private EnemyCoverPoint FindBestCover()
    {
        DebugCoverScan();
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                coverSearchRadius,
                coverLayer
            );

        EnemyCoverPoint bestCover = null;
        float bestScore = float.MinValue;

        foreach (Collider2D hit in hits)
        {
            EnemyCoverPoint cover =
                hit.GetComponent<EnemyCoverPoint>();

            if (cover == null)
                cover =
                    hit.GetComponentInChildren<EnemyCoverPoint>();

            if (cover == null)
                continue;

            if (!cover.isActive)
                continue;

            float distance =
                Vector2.Distance(
                    transform.position,
                    cover.transform.position
                );

            float score =
                cover.coverScore -
                (distance / coverSearchRadius);

            if (score > bestScore)
            {
                bestScore = score;
                bestCover = cover;
            }
        }

        return bestCover;
    }
    private bool IsCoveredFromPlayer(EnemyCoverPoint cover)
    {
        if (target == null || target.player == null)
            return false;

        Vector2 playerPos =
            target.player.position;

        Vector2 coverPos =
            cover.transform.position;

        Vector2 direction =
            coverPos - playerPos;

        float distance =
            direction.magnitude;

        RaycastHit2D hit =
            Physics2D.Raycast(
                playerPos,
                direction.normalized,
                distance,
                coverLayer
            );

        bool blocked =
            hit.collider != null;

        Debug.Log(
            gameObject.name +
            " [COVER LOS CHECK]" +
            " | Cover = " +
            cover.name +
            " | Blocked = " +
            blocked
        );

        return blocked;
    }
    private bool IsCoverBetweenPlayerAndEnemy(EnemyCoverPoint cover)
    {
        if (target == null || target.player == null)
            return false;

        Vector2 player =
            target.player.position;

        Vector2 enemy =
            transform.position;

        Vector2 coverPosition =
            cover.transform.position;

        Vector2 playerToEnemy =
            enemy - player;

        Vector2 playerToCover =
            coverPosition - player;

        if (playerToCover.magnitude >=
            playerToEnemy.magnitude)
        {
            return false;
        }

        Vector2 direction =
            playerToCover.normalized;

        float angle =
            Vector2.Angle(
                direction,
                playerToEnemy.normalized
            );

        return angle < 35f;
    }
    private EnemyCoverPoint FindSafeCover()
    {
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                coverSearchRadius,
                coverLayer
            );

        Debug.Log(
            gameObject.name +
            " [COVER CANDIDATES] = " +
            hits.Length
        );

        EnemyCoverPoint bestCover = null;
        float bestScore = float.MinValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            EnemyCoverPoint cover =
                hit.GetComponent<EnemyCoverPoint>();

            if (cover == null)
                cover =
                    hit.GetComponentInParent<EnemyCoverPoint>();

            if (cover == null)
                cover =
                    hit.GetComponentInChildren<EnemyCoverPoint>();

            if (cover == null)
                continue;

            if (!cover.isActive)
                continue;

            // =========================================
            // CARI RECOVERY POINT
            // =========================================

            RecoveryPoint recovery =
                cover.GetComponentInChildren<RecoveryPoint>();

            if (recovery == null)
            {
                Debug.Log(
                    gameObject.name +
                    " [COVER SKIP]" +
                    " | " + cover.name +
                    " tidak memiliki RecoveryPoint"
                );

                continue;
            }

            // =========================================
            // VALIDASI HIDE POINT
            // =========================================

            if (recovery.hidePoint == null)
            {
                Debug.Log(
                    gameObject.name +
                    " [COVER SKIP]" +
                    " | " + cover.name +
                    " RecoveryHidePoint NULL"
                );

                continue;
            }

            // =========================================
            // VALIDASI EXIT POINT
            // =========================================

            if (recovery.exitPoint == null)
            {
                Debug.Log(
                    gameObject.name +
                    " [COVER SKIP]" +
                    " | " + cover.name +
                    " RecoveryExitPoint NULL"
                );

                continue;
            }

            if (!recovery.isActive)
            {
                Debug.Log(
                    gameObject.name +
                    " [COVER SKIP]" +
                    " | RecoveryPoint inactive"
                );

                continue;
            }

            // =========================================
            // HITUNG SCORE
            // =========================================

            float distance =
                Vector2.Distance(
                    transform.position,
                    cover.transform.position
                );

            float score =
                cover.coverScore -
                (distance / coverSearchRadius);

            Debug.Log(
                gameObject.name +
                " [COVER VALID]" +
                " | Cover = " +
                cover.name +
                " | RecoveryPoint = " +
                recovery.name +
                " | HidePoint = " +
                recovery.hidePoint.name +
                " | ExitPoint = " +
                recovery.exitPoint.name +
                " | Distance = " +
                distance +
                " | Score = " +
                score
            );

            if (score > bestScore)
            {
                bestScore = score;
                bestCover = cover;
            }
        }

        return bestCover;
    }
    private bool PrepareRecoveryPoint()
    {
        if (targetCover == null)
            return false;

        currentRecoveryPoint =
            targetCover.GetComponentInChildren<RecoveryPoint>();

        if (currentRecoveryPoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " [RECOVERY POINT NOT FOUND]" +
                " | Cover = " +
                targetCover.name
            );

            return false;
        }

        if (!currentRecoveryPoint.isActive)
        {
            Debug.LogWarning(
                gameObject.name +
                " [RECOVERY POINT INACTIVE]" +
                " | RecoveryPoint = " +
                currentRecoveryPoint.name
            );

            currentRecoveryPoint = null;

            return false;
        }

        if (currentRecoveryPoint.hidePoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " [RECOVERY HIDE POINT NOT FOUND]" +
                " | RecoveryPoint = " +
                currentRecoveryPoint.name
            );

            currentRecoveryPoint = null;

            return false;
        }

        if (currentRecoveryPoint.exitPoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " [RECOVERY EXIT POINT NOT FOUND]" +
                " | RecoveryPoint = " +
                currentRecoveryPoint.name
            );

            currentRecoveryPoint = null;

            return false;
        }

        Debug.Log(
            gameObject.name +
            " [RECOVERY TARGET READY]" +
            " | Cover = " +
            targetCover.name +
            " | Recovery = " +
            currentRecoveryPoint.name +
            " | HidePoint = " +
            currentRecoveryPoint.hidePoint.name +
            " | ExitPoint = " +
            currentRecoveryPoint.exitPoint.name
        );

        return true;
    }
    private Vector2 GetDynamicCoverPosition()
{
    if (targetCover == null)
        return transform.position;

    if (target == null || target.player == null)
        return targetCover.transform.position;

    // ==========================================
    // POSISI PLAYER DAN COVER
    // ==========================================

    Vector2 playerPos =
        target.player.position;

    Vector2 coverCenter =
        targetCover.transform.position;

    // Arah dari PLAYER menuju COVER
    Vector2 directionFromPlayer =
        (coverCenter - playerPos).normalized;

    // ==========================================
    // AMBIL COLLIDER COVER
    // ==========================================

    Collider2D coverCollider =
        targetCover.GetComponentInChildren<Collider2D>();

    if (coverCollider == null)
    {
        Debug.LogWarning(
            gameObject.name +
            " [COVER COLLIDER NOT FOUND]" +
            " | Using CoverPoint position"
        );

        return coverCenter;
    }

    // ==========================================
    // JARAK AMAN DARI PUSAT COLLIDER
    // ==========================================

    float safeDistance =
        coverCollider.bounds.extents.magnitude +
        coverOffset;

    // ==========================================
    // POSISI DI BELAKANG COVER
    // ==========================================

    Vector2 hidingPosition =
        coverCenter +
        directionFromPlayer *
        safeDistance;

    Debug.Log(
        gameObject.name +
        " [DYNAMIC COVER POSITION]" +
        " | Center = " +
        coverCenter +
        " | Destination = " +
        hidingPosition +
        " | SafeDistance = " +
        safeDistance
    );

    return hidingPosition;
}
    private void MoveToRecovery()
    {
        if (currentRecoveryPoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " [MOVE RECOVERY FAILED]" +
                " | RecoveryPoint NULL" +
                " | Fallback = Flee"
            );

            targetCover = null;
            hasCoverTarget = false;

            FleeFromPlayer();
            return;
        }

        if (currentRecoveryPoint.hidePoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " [MOVE RECOVERY FAILED]" +
                " | HidePoint NULL" +
                " | Fallback = Flee"
            );

            targetCover = null;
            hasCoverTarget = false;

            FleeFromPlayer();
            return;
        }

        if (rb == null)
        {
            Debug.LogError(
                gameObject.name +
                " [MOVE RECOVERY FAILED]" +
                " | Rigidbody2D NULL"
            );

            return;
        }

        Vector2 destination =
            currentRecoveryPoint.hidePoint.position;

        Vector2 currentPosition =
            transform.position;

        Vector2 direction =
            destination - currentPosition;

        float distance =
            direction.magnitude;


        // =====================================================
        // RECOVERY ARRIVAL THRESHOLD
        // =====================================================

        float arrivalThreshold = 1.5f;


        // =====================================================
        // SUDAH CUKUP DEKAT DENGAN HIDE POINT
        // =====================================================

        if (distance <= arrivalThreshold)
        {
            StopMoving();

            Debug.Log(
                gameObject.name +
                " [RECOVERY HIDE POINT REACHED]" +
                " | Distance = " +
                distance +
                " | Threshold = " +
                arrivalThreshold +
                " | HidePoint = " +
                currentRecoveryPoint.hidePoint.name
            );


            // =================================================
            // SNAP KE HIDE POINT
            // =================================================

            transform.position =
                currentRecoveryPoint.hidePoint.position;


            Debug.Log(
                gameObject.name +
                " [RECOVERY SNAP]" +
                " | Position = " +
                transform.position
            );


            // =================================================
            // SEMBUNYI
            // =================================================

            HideForRecovery(
                currentRecoveryPoint
            );


            recoveryReason =
                "SAFE_COVER";


            // =================================================
            // MULAI RECOVERY
            // =================================================

            StartRecovery();

            return;
        }


        // =====================================================
        // MASIH JAUH → TERUS BERGERAK
        // =====================================================

        direction.Normalize();


        // =====================================================
        // WORLD BOUNDARY
        // =====================================================

        direction =
            ApplyWorldBoundary(direction);


        rb.velocity =
            direction * moveSpeed;


        Debug.Log(
            gameObject.name +
            " [MOVE TO RECOVERY]" +
            " | Cover = " +
            (targetCover != null
                ? targetCover.name
                : "NULL") +
            " | Recovery = " +
            currentRecoveryPoint.name +
            " | Destination = " +
            destination +
            " | Distance = " +
            distance +
            " | Threshold = " +
            arrivalThreshold +
            " | Velocity = " +
            rb.velocity
        );
    }
    private bool IsCoverDestinationBlocked(Vector2 destination)
    {
        Collider2D hit =
            Physics2D.OverlapPoint(
                destination,
                coverLayer
            );

        if (hit != null)
        {
            Debug.Log(
                gameObject.name +
                " [COVER DESTINATION BLOCKED]" +
                " | Object = " +
                hit.name +
                " | Destination = " +
                destination
            );

            return true;
        }

        return false;
    }

    private void DebugRecoveryEnvironment()
    {
        Collider2D[] nearby =
            Physics2D.OverlapCircleAll(
                transform.position,
                1.0f,
                coverLayer
            );

        Debug.Log(
            gameObject.name +
            " [RECOVERY ENVIRONMENT]" +
            " | Nearby Cover Collider = " +
            nearby.Length
        );

        foreach (Collider2D col in nearby)
        {
            if (col == null)
                continue;

            float distance =
                Vector2.Distance(
                    transform.position,
                    col.bounds.ClosestPoint(
                        transform.position
                    )
                );

            Debug.Log(
                gameObject.name +
                " [RECOVERY COLLIDER]" +
                " | Object = " +
                col.name +
                " | Distance = " +
                distance
            );
        }
    }
    private RecoveryPoint GetRecoveryPoint()
    {
        if (targetCover == null)
            return null;

        RecoveryPoint recovery =
            targetCover.GetComponentInChildren<RecoveryPoint>();

        if (recovery == null)
        {
            Debug.Log(
                gameObject.name +
                " [COVER WITHOUT RECOVERY]" +
                " | Cover = " +
                targetCover.name
            );

            return null;
        }

        if (!recovery.isActive)
        {
            Debug.Log(
                gameObject.name +
                " [RECOVERY POINT INACTIVE]" +
                " | Recovery = " +
                recovery.name
            );

            return null;
        }

        Debug.Log(
            gameObject.name +
            " [RECOVERY POINT VALID]" +
            " | Cover = " +
            targetCover.name +
            " | Recovery = " +
            recovery.name
        );

        return recovery;
    }
    private void HideForRecovery(RecoveryPoint recoveryPoint)
    {
        if (recoveryPoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " [HIDE RECOVERY FAILED] | RecoveryPoint NULL"
            );
            return;
        }

        if (recoveryPoint.hidePoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " [HIDE RECOVERY FAILED] | HidePoint NULL"
            );
            return;
        }

        // ==========================================
        // STOP PHYSICS
        // ==========================================

        StopMoving();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }


        // ==========================================
        // PINDAH KE HIDE POINT
        // ==========================================

        transform.position =
            recoveryPoint.hidePoint.position;


        // ==========================================
        // SIMPAN RECOVERY POINT
        // ==========================================

        currentRecoveryPoint =
            recoveryPoint;

        isHiddenRecovery = true;


        // ==========================================
        // HILANGKAN SPRITE
        // ==========================================

        SpriteRenderer[] sprites =
            GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.enabled = false;
        }


        // ==========================================
        // MATIKAN COLLIDER ENEMY
        // ==========================================

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }


        Debug.Log(
            gameObject.name +
            " [RECOVERY HIDDEN]" +
            " | HidePoint = " +
            recoveryPoint.hidePoint.position +
            " | Enemy Hidden = TRUE"
        );
    }
    private void ExitRecoveryPoint()
    {
        if (!isHiddenRecovery)
            return;

        // --------------------------------
        // PINDAHKAN KE EXIT POINT
        // --------------------------------

        if (currentRecoveryPoint != null &&
            currentRecoveryPoint.exitPoint != null)
        {
            transform.position =
                currentRecoveryPoint.exitPoint.position;
        }

        // --------------------------------
        // HIDUPKAN SPRITE
        // --------------------------------

        SpriteRenderer[] sprites =
            GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.enabled = true;
        }

        // --------------------------------
        // HIDUPKAN COLLIDER
        // --------------------------------

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = true;
        }

        isHiddenRecovery = false;
        currentRecoveryPoint = null;

        Debug.Log(
            gameObject.name +
            " [RECOVERY EXIT]" +
            " | Enemy kembali terlihat"
        );
    }
    private void DebugCoverScan()
    {
        Collider2D[] allColliders =
            Physics2D.OverlapCircleAll(
                transform.position,
                coverSearchRadius
            );

        Debug.Log(
            gameObject.name +
            " [DEBUG ALL COLLIDERS]" +
            " | Radius = " +
            coverSearchRadius +
            " | Found = " +
            allColliders.Length
        );

        foreach (Collider2D col in allColliders)
        {
            if (col == null)
                continue;

            float distance =
                Vector2.Distance(
                    transform.position,
                    col.transform.position
                );

            EnemyCoverPoint cover =
                col.GetComponentInParent<EnemyCoverPoint>();

            if (cover == null)
            {
                cover =
                    col.GetComponentInChildren<EnemyCoverPoint>();
            }

            Debug.Log(
                gameObject.name +
                " [DEBUG COLLIDER]" +
                " | Object = " +
                col.name +
                " | Layer = " +
                LayerMask.LayerToName(col.gameObject.layer) +
                " | Distance = " +
                distance +
                " | EnemyCoverPoint = " +
                (cover != null ? cover.name : "NULL")
            );
        }
    }
    private void LateUpdate()
    {
        if (!useWorldBoundary)
            return;

        if (IsNearWorldBoundary())
        {
            ClampToWorldBoundary();
        }
    }
}