using UnityEngine;

public class BossCombatController : MonoBehaviour
{
    [Header("Attack")]
    public float attackRadius = 0.8f;
    public int attackDamage = 1;

    public LayerMask playerLayer;
    public Transform attackPoint;

    [Header("Testing")]
    public KeyCode attackKey = KeyCode.K;

    private bool isAttacking = false;

    private void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (isAttacking)
        {
            Debug.Log("[BOSS COMBAT] Attack blocked - already attacking");
            return;
        }

        if (attackPoint == null)
        {
            Debug.LogError(
                "[BOSS COMBAT] Attack Point NOT FOUND");
            return;
        }

        isAttacking = true;

        Debug.Log("[BOSS COMBAT] ATTACK EXECUTE");
        Debug.Log("========== BOSS ATTACK ==========");

        Debug.Log(
            "[BOSS COMBAT] Attack Point = " +
            attackPoint.position);

        Debug.Log(
            "[BOSS COMBAT] Radius = " +
            attackRadius);

        Debug.Log(
            "[BOSS COMBAT] Damage = " +
            attackDamage);

        Debug.Log(
            "[BOSS COMBAT] Player LayerMask = " +
            playerLayer.value);

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRadius,
                playerLayer);

        Debug.Log(
            "[BOSS COMBAT] Hit Count = " +
            hits.Length);

        foreach (Collider2D hit in hits)
        {
            Debug.Log(
                "[BOSS COMBAT] Target = " +
                hit.name);

            Health hp = hit.GetComponent<Health>();

            if (hp != null)
            {
                Debug.Log(
                    "[BOSS COMBAT] PLAYER HIT | " +
                    hit.name +
                    " | Damage = " +
                    attackDamage);

                hp.TakeDamage(attackDamage);
            }
            else
            {
                Debug.Log(
                    "[BOSS COMBAT] TARGET HAS NO HEALTH | " +
                    hit.name);
            }
        }

        Debug.Log("==============================");

        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius);
    }
}