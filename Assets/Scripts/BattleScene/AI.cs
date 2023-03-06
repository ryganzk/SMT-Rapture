using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class AI : MonoBehaviour
{
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
        GameObject target = null;
        NonPassiveSkill randMove = null;
        List<Skill> skills = null;
        List<float> factorArray = new List<float> { attackFactor, ailmentFactor, healFactor, supportFactor };

        while (target == null)
        {
            skills = DetermineMoveType(factorArray[0], factorArray[1], factorArray[2], factorArray[3]);

            while (skills != null)
            {
                randMove = (NonPassiveSkill) skills[(int) Mathf.Floor(UnityEngine.Random.Range(0f, skills.Count))];
                Debug.Log("MOVE: " + randMove);
                
                List<GameObject> demons = gameManager.AliveDemons(gameManager.opponentTeam.GetComponent<Team>().activeDemons);
                target = DetermineTarget(randMove, demons);
                Debug.Log("TARGET: " + target);

                if (target != null)
                    break;

                skills.Remove(randMove);
                if (skills.Count == 0)
                    factorArray[gameManager.DetermineSkillType((Skill) randMove)] = 0;
            }
        }
        
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

    GameObject DetermineTarget(NonPassiveSkill randMove, List<GameObject> demons)
    {
        if (demons.Count == 0)
            return null;

        GameObject randTarget = demons[(int) Mathf.Floor(UnityEngine.Random.Range(0f, demons.Count))];

        // Almighty skills cannot be blocked
        if (((AttackSkill) randMove).type == 7)
            return randTarget;

        // Don't use move if target nulls move or better
        int resAmnt = gameManager.playerTeam.GetComponent<Team>().FindOpposingData(randTarget).resistances[((AttackSkill) randMove).type];
        if (resAmnt > 2 && resAmnt != 7)
        {
            demons.Remove(randTarget);
            randTarget = DetermineTarget(randMove, demons);
        }

        return randTarget;
    }
}
