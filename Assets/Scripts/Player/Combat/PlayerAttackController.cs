using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [Header("Attack")]
    public float attackRadius = 0.6f;

    public LayerMask enemyLayer;

    public Transform attackPoint;

    [Header("Damage")]
    public int attackDamage = 10;

    [Header("Input")]
    public KeyCode attackKey = KeyCode.Space;

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("========== PLAYER ATTACK ==========");

        if (attackPoint == null)
        {
            Debug.LogError("[PLAYER ATTACK] Attack Point NOT FOUND");
            return;
        }

        Debug.Log("[PLAYER ATTACK] Attack Point = " + attackPoint.position);
        Debug.Log("[PLAYER ATTACK] Radius = " + attackRadius);
        Debug.Log("[PLAYER ATTACK] LayerMask = " + enemyLayer.value);
        Debug.Log("[PLAYER ATTACK] Damage = " + attackDamage);

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRadius,
                enemyLayer);

        Debug.Log("[PLAYER ATTACK] Hit Count = " + hits.Length);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("[PLAYER ATTACK] Target = " + hit.name);

            // ==========================================
            // 1. ENEMY
            // ==========================================

            Health hp = hit.GetComponent<Health>();

            if (hp != null)
            {
                Debug.Log(
                    "[PLAYER ATTACK] ENEMY HIT | " +
                    hit.name +
                    " | Damage = " +
                    attackDamage);

                hp.TakeDamage(attackDamage);

                continue;
            }

            // ==========================================
            // 2. BOSS
            // ==========================================

            BossHealth bossHP = hit.GetComponent<BossHealth>();

            if (bossHP != null)
            {
                Debug.Log(
                    "[PLAYER ATTACK] BOSS HIT | " +
                    hit.name +
                    " | Damage = " +
                    attackDamage);

                bossHP.TakeDamage(attackDamage);

                continue;
            }

            // ==========================================
            // 3. TARGET TIDAK MEMILIKI HEALTH
            // ==========================================

            Debug.Log(
                "[PLAYER ATTACK] TARGET HAS NO DAMAGE COMPONENT | " +
                hit.name);
        }

        Debug.Log("==============================");
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