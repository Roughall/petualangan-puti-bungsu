using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    [Header("Current State")]
    public EnemyState currentState = EnemyState.Idle;

    void Awake()
    {
        Debug.Log("Enemy FSM Ready");
        Debug.Log("Current State : " + currentState);
    }
}