using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Detection")]

    public float chaseRange = 5f;

    public float attackRange = 1.5f;

    [HideInInspector]
    public float currentDistance;

    void Awake()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
        {
            player = obj.transform;
            Debug.Log("Target ditemukan : " + player.name);
        }
        else
        {
            Debug.LogError("Player tidak ditemukan!");
        }
        
    }
    void Update()
    {
        if (player == null)
            return;

        currentDistance =
        Vector2.Distance(
            transform.position,
            player.position);

        Debug.Log("Distance = " + currentDistance);
        }
    public float DistanceToPlayer()
    {
        if (player == null)
        return Mathf.Infinity;

        return Vector2.Distance(transform.position, player.position);
    }
}