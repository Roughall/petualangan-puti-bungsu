using UnityEngine;

[CreateAssetMenu(
    fileName = "BossData",
    menuName = "Game/Boss Data"
)]
public class BossData : ScriptableObject
{
    [Header("Boss Identity")]
    public string bossID;
    public string bossName;

    [Header("Boss Stats")]
    public int maxHP = 100;
    public int attack = 10;
    public int defense = 5;

    [Header("Boss Scene")]
    public string arenaScene;

    [Header("Quiz")]
    public string quizID;

    [Header("Reward")]
    public string rewardID;
}