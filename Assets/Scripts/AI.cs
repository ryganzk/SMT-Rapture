using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class AI : MonoBehaviour
{
    private const int ATTACK_ID = 182;
    private const int AILMENT_ID = 200;
    private const int RECOVERY_ID = 220;
    private const int SUPPORT_ID = 258;

    // Move Factors
    public float attackFactor;
    public float ailmentFactor;
    public float healFactor;
    public float supportFactor;
    public GameManager gameManager;
    private GameObject demon;

    // Target Factors (for healing/support moves)
    public float selfFactor;
    public float teamFactor;

    // Priorities
    public float scoutWeaknesses;
    public float exploitWeaknesses;
    public float defeatWeakEnemies;
    public float inflictAilments;
    public float healTeammates;
    public float cureTeammates;
    public float applyCharges;
    public float buffTeammates;
    public float debuffEnemies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AIMove()
    {
        List<Skill> skills = DetermineMoveType(attackFactor, ailmentFactor, healFactor, supportFactor);
        NonPassiveSkill randMove = (NonPassiveSkill) skills[(int) Mathf.Floor(UnityEngine.Random.Range(0f, skills.Count))];
        GameObject target = DetermineTarget();
        
        gameManager.ExecuteMove(target, randMove, gameManager.opponentTeam.GetComponent<Team>().activeDemons);
    }

    List<Skill> DetermineMoveType(float attackFactor, float ailmentFactor, float healFactor, float supportFactor)
    {
        float totalFactor = attackFactor + ailmentFactor + healFactor + supportFactor;
        float randFactor = UnityEngine.Random.Range(0f, totalFactor);

        List<Skill> skills;

        if (randFactor < attackFactor)
            skills = GetSkillList(0);
        else if (randFactor < attackFactor + ailmentFactor)
            skills = GetSkillList(1);
        else if (randFactor < attackFactor + ailmentFactor + healFactor)
            skills = GetSkillList(2);
        else
            skills = GetSkillList(3);

        return skills;

    }

    List<Skill> GetSkillList(int moveType)
    {
        List<Skill> skills;
        skills = DetermineMoves(moveType);

        if (skills.Count == 0) {
            switch (moveType)
            {
                case 0:  
                    skills = DetermineMoveType(0, ailmentFactor, healFactor, supportFactor);
                    break;
                case 1:  
                    skills = DetermineMoveType(attackFactor, 0, healFactor, supportFactor);
                    break;
                case 2:
                    skills = DetermineMoveType(attackFactor, ailmentFactor, 0, supportFactor);
                    break;
                default:
                    skills = DetermineMoveType(attackFactor, ailmentFactor, healFactor, 0);
                    break;
            }
        }
        return skills;
    }

    List<Skill> DetermineMoves(int moveType)
    {
        List<Skill> skills = new List<Skill>();
        foreach (Skill skill in gameManager.active.GetComponent<ActorStats>().stats.skills)
        {
            switch (moveType)
            {
                case 0:
                    if (skill.skillID <= ATTACK_ID)
                        skills.Add(skill);
                    continue;
                case 1:  
                    if (skill.skillID > ATTACK_ID && skill.skillID <= AILMENT_ID)
                        skills.Add(skill);
                    continue;
                case 2:
                    if (skill.skillID > AILMENT_ID && skill.skillID <= RECOVERY_ID)
                        skills.Add(skill);
                    continue;
                default:
                    if (skill.skillID > RECOVERY_ID && skill.skillID <= SUPPORT_ID)
                        skills.Add(skill);
                    continue;
            }
        }

        return skills;
    }

    public GameObject DetermineTarget()
    {
        List<GameObject> demons = gameManager.AliveDemons(gameManager.opponentTeam.GetComponent<Team>().activeDemons);
        GameObject randTarget = demons[(int) Mathf.Floor(UnityEngine.Random.Range(0f, demons.Count))];
        return randTarget;
    }
}
