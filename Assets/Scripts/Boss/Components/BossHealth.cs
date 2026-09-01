using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Health")]
    public int maxHP = 100;

    private int currentHP;
    private bool isDead = false;

    public int CurrentHP
    {
        get { return currentHP; }
    }

    public int MaxHP
    {
        get { return maxHP; }
    }

    public bool IsDead
    {
        get { return isDead; }
    }

    private void Awake()
    {
        currentHP = maxHP;
        isDead = false;

        Debug.Log(
            "[BOSS HEALTH] Initialized | HP = " +
            currentHP + "/" + maxHP);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            Debug.Log("[BOSS HEALTH] Damage ignored - Boss is DEAD");
            return;
        }

        if (damage <= 0)
        {
            Debug.LogWarning(
                "[BOSS HEALTH] Invalid damage = " + damage);
            return;
        }

        currentHP -= damage;

        if (currentHP < 0)
            currentHP = 0;

        Debug.Log(
            "[BOSS HEALTH] Damage = " +
            damage +
            " | HP = " +
            currentHP +
            "/" +
            maxHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("[BOSS HEALTH] DEATH");
    }
}