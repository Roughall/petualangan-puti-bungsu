using UnityEngine;

public class BossBrain : MonoBehaviour
{
    public enum BossState
    {
        IDLE,
        CHASE,
        ATTACK
    }

    [Header("Target")]
    public Transform target;

    [Header("References")]
    public BossCombatController combatController;

    [Header("Detection")]
    public float detectionRange = 6f;
    public float attackRange = 1.5f;

    [Header("State")]
    [SerializeField]
    private BossState currentState = BossState.IDLE;

    [Header("Debug")]
    public bool enableDebugLog = true;

    public BossState CurrentState
    {
        get { return currentState; }
    }

    private void Start()
    {
        DebugState("START");
        ChangeState(BossState.IDLE);
    }

    private void Update()
    {
        if (target == null)
        {
            if (currentState != BossState.IDLE)
                ChangeState(BossState.IDLE);

            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            target.position);

        switch (currentState)
        {
            case BossState.IDLE:
                UpdateIdle(distance);
                break;

            case BossState.CHASE:
                UpdateChase(distance);
                break;

            case BossState.ATTACK:
                UpdateAttack(distance);
                break;
        }
    }

    private void UpdateIdle(float distance)
    {
        if (distance <= detectionRange)
        {
            DebugLog(
                "[BOSS AI] PLAYER DETECTED | Distance = " +
                distance.ToString("F2"));

            ChangeState(BossState.CHASE);
        }
    }

    private void UpdateChase(float distance)
    {
        if (distance > detectionRange)
        {
            DebugLog(
                "[BOSS AI] PLAYER LOST | Distance = " +
                distance.ToString("F2"));

            ChangeState(BossState.IDLE);
            return;
        }

        if (distance <= attackRange)
        {
            ChangeState(BossState.ATTACK);
            return;
        }

        DebugLog(
            "[BOSS AI] CHASE | Distance = " +
            distance.ToString("F2"));
    }

    private void UpdateAttack(float distance)
    {
        if (distance > attackRange)
        {
            ChangeState(BossState.CHASE);
            return;
        }

        DebugLog(
            "[BOSS AI] ATTACK READY | Distance = " +
            distance.ToString("F2"));
    }

    private void ChangeState(BossState newState)
    {
        if (currentState == newState)
            return;

        BossState previousState = currentState;
        currentState = newState;

        DebugLog(
            "[BOSS AI] STATE CHANGE | " +
            previousState + " -> " +
            currentState);

        if (currentState == BossState.ATTACK)
        {
            DebugLog("[BOSS AI] ATTACK REQUEST");

            if (combatController != null)
            {
                combatController.Attack();
            }
            else
            {
                DebugLog("[BOSS AI] ATTACK FAILED | Combat Controller NOT FOUND");
            }
        }
    }

    private void DebugState(string source)
    {
        DebugLog(
            "[BOSS AI] " +
            source +
            " | STATE = " +
            currentState);
    }

    private void DebugLog(string message)
    {
        if (enableDebugLog)
            Debug.Log(message);
    }
}
