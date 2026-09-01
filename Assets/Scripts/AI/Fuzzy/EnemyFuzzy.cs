using UnityEngine;

public class EnemyFuzzy : MonoBehaviour
{
    [Header("Input")]
    public float currentHP = 100f;
    public float maxHP = 100f;
    public float distanceToPlayer = 10f;
    private EnemyTarget target;
    private Health health;

    [Header("Output")]
    [Range(0, 100)]
    public float aggressiveness;
    [Range(0, 1)]
    public float fleeStrength;
    public float attackStrength;
    // =========================================
    // UPDATE
    // =========================================
    void Update()
    {
        if (target != null)
        {
            distanceToPlayer = target.currentDistance;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TestFuzzy();
        }

    }
    void Awake()
    {
        target = GetComponent<EnemyTarget>();
        health = GetComponent<Health>();

        if (target == null)
        {
            Debug.LogError("EnemyTarget tidak ditemukan pada EnemyFuzzy!");
        }

        if (health == null)
        {
            Debug.LogError("Health tidak ditemukan pada EnemyFuzzy!");
        }
        else
        {
            Debug.Log(
                "EnemyFuzzy: Health ditemukan. Max HP = "
                + health.maxHP
            );
        }
    }
    // =========================================
    // INPUT UPDATE
    // =========================================
    public void UpdateInput(float hp, float distance)
    {
        currentHP = hp;
        distanceToPlayer = distance;
    }
    // =========================================
    // FUZZY OUTPUT
    // =========================================
    public float Evaluate()
    {
        // ==============================
        // FUZZIFICATION
        // ==============================

        float hpLow = HP_Low(currentHP);
        float hpMedium = HP_Medium(currentHP);
        float hpHigh = HP_High(currentHP);

        float distanceNear = Distance_Near(distanceToPlayer);
        float distanceMedium = Distance_Medium(distanceToPlayer);
        float distanceFar = Distance_Far(distanceToPlayer);


        // ==============================
        // RULE EVALUATION
        // ==============================

        // R1: IF HP High AND Distance Near THEN Attack
        float rule1 = Mathf.Min(hpHigh, distanceNear);

        // R2: IF HP High AND Distance Medium THEN Attack
        float rule2 = Mathf.Min(hpHigh, distanceMedium);

        // R3: IF HP High AND Distance Far THEN Neutral
        float rule3 = Mathf.Min(hpHigh, distanceFar);

        // R4: IF HP Medium AND Distance Near THEN Neutral
        float rule4 = Mathf.Min(hpMedium, distanceNear);

        // R5: IF HP Medium AND Distance Medium THEN Neutral
        float rule5 = Mathf.Min(hpMedium, distanceMedium);

        // R6: IF HP Medium AND Distance Far THEN Neutral
        float rule6 = Mathf.Min(hpMedium, distanceFar);

        // R7: IF HP Low AND Distance Near THEN Flee
        float rule7 = Mathf.Min(hpLow, distanceNear);

        // R8: IF HP Low AND Distance Medium THEN Flee
        float rule8 = Mathf.Min(hpLow, distanceMedium);

        // R9: IF HP Low AND Distance Far THEN Flee
        float rule9 = Mathf.Min(hpLow, distanceFar);
        // ==============================
        // AGGREGATION
        // ==============================
        float attackRule1 = Mathf.Min(hpHigh,distanceNear);

        float attackRule2 =Mathf.Min(hpHigh,distanceMedium);

        float attackRule3 =Mathf.Min(hpMedium,distanceNear);

        float attackHigh = attackRule1;

        float attackMedium = Mathf.Max(attackRule2,attackRule3);

        attackStrength = (attackHigh * 1.0f + attackMedium * 0.5f)
        / Mathf.Max(attackHigh + attackMedium,0.0001f);

        float neutralStrength =
            Mathf.Max(rule3, rule4, rule5, rule6);

        fleeStrength =
            Mathf.Max(rule7, rule8, rule9);

        // ==============================
        // DEFUZZIFICATION
        // ==============================

        float attackValue = 100f;
        float neutralValue = 50f;
        float fleeValue = 0f;

        float totalStrength =
            attackStrength +
            neutralStrength +
            fleeStrength;

        if (totalStrength <= 0f)
        {
            aggressiveness = 50f;
            return aggressiveness;
        }

        aggressiveness =
            (
                attackStrength * attackValue +
                neutralStrength * neutralValue +
                fleeStrength * fleeValue
            )
            / totalStrength;

        Debug.Log("FUZZY OUTPUT" +" | HP = " + currentHP +" | Distance = " + distanceToPlayer +
            " | Aggressiveness = " + aggressiveness +" | FleeStrength = " + fleeStrength +
            " | AttackStrength = " + attackStrength);

        return aggressiveness;
    }

    // =========================================
    // HP MEMBERSHIP
    // =========================================

    float HP_Low(float hp)
    {
        if (hp <= 30f)
            return 1f;

        if (hp >= 55f)
            return 0f;

        return (55f - hp) / 30f;
    }
    float HP_Medium(float hp)
    {
        if (hp <= 30f || hp >= 80f)
            return 0f;

        if (hp == 55f)
            return 1f;

        if (hp < 55f)
            return (hp - 30f) / 25f;

        return (80f - hp) / 25f;
    }
    float HP_High(float hp)
    {
        if (hp <= 50f)
            return 0f;

        if (hp >= 80f)
            return 1f;

        return (hp - 50f) / 30f;
    }


    // =========================================
    // DISTANCE MEMBERSHIP
    // =========================================

    float Distance_Near(float distance)
    {
        if (distance <= 1.5f)
            return 1f;

        if (distance >= 3f)
            return 0f;

        return (3f - distance) / 1.5f;
    }


    float Distance_Medium(float distance)
    {
        if (distance <= 1.5f || distance >= 5f)
            return 0f;

        if (distance == 3f)
            return 1f;

        if (distance < 3f)
            return (distance - 1.5f) / 1.5f;

        return (5f - distance) / 2f;
    }


    float Distance_Far(float distance)
    {
        if (distance <= 3f)
            return 0f;

        if (distance >= 5f)
            return 1f;

        return (distance - 3f) / 2f;
    }

    public void TestFuzzy()
    {
        float result = Evaluate();

        Debug.Log(
            "===== FUZZY TEST ====="
        );

        Debug.Log(
            "HP = " + currentHP
        );

        Debug.Log(
            "Distance = " + distanceToPlayer
        );

        Debug.Log(
            "Aggressiveness = " + result
        );

        Debug.Log(
            "======================"
        );
    }
    public void SyncHealth()
    {
        if (health == null)
            return;

        if (health.maxHP <= 0)
            return;

        currentHP =
            ((float)health.CurrentHP / health.maxHP) * 100f;

        maxHP = 100f;

        Debug.Log(
            "Fuzzy HP Sync : "
            + health.CurrentHP
            + " / "
            + health.maxHP
            + " = "
            + currentHP
        );
    }
}