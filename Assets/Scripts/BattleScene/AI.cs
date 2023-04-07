using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static ActorStats;

public class AI : MonoBehaviour
{
    // AI Type
    public int aiType = 0;
    public GameManager gameManager;
    public bool useTeamFactors;
    private GameObject demon;
    private int skillFactor;
    private int targetFactor;

    // Team Factors
    public float attackFactor;
    public float ailmentFactor;
    public float healFactor;
    public float supportFactor;

    public float selfFactor;
    public float allyPlayerFactor;
    public float enemyPlayerFactor;
    public float allyTeamFactor;
    public float enemyTeamFactor;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AIMove()
    {
        switch(aiType)
        {
            case 1:
                SkillDependentAI();
                return;
            case 2:
                TargetDependentAI();
                return;
            default:
                RandomAI();
                return;
        }
    }

    // AI that uses skills entirely randomly
    private void RandomAI()
    {
        List<Skill> skills = gameManager.active.GetComponent<ActorStats>().stats.skills;
        NonPassiveSkill randMove = null;

        // Use a regular attack 10% of the time
        if (skills.Count == 0 || Mathf.Floor(UnityEngine.Random.Range(0f, 10f)) >= 9f)
            randMove = gameManager.NormalAttack();
        else
            randMove = (NonPassiveSkill) skills[(int) Mathf.Floor(UnityEngine.Random.Range(0f, skills.Count))];

        List<GameObject> targets = GetTargetList(randMove);
        GameObject target = targets[(int) Mathf.Floor(UnityEngine.Random.Range(0f, targets.Count))];

        //Debug.Log("MOVE: " + randMove);
        //Debug.Log("TARGET: " + target);
        gameManager.ExecuteMove(target, randMove, gameManager.opponentTeam.GetComponent<Team>().activeDemons);
    }

    // AI that picks a skill first, then a target to use the skill on
    private void SkillDependentAI()
    {
        GameObject target = null;
        NonPassiveSkill randMove = null;
        List<Skill> skills = null;
        List<float> factorArray;
        
        AIFactors activeFactors = gameManager.active.GetComponent<ActorStats>().aiFactors;
        if (useTeamFactors)
            factorArray = new List<float> { attackFactor, ailmentFactor, healFactor, supportFactor };
        else
            factorArray = new List<float> { activeFactors.attackFactor, activeFactors.ailmentFactor, activeFactors.healFactor, activeFactors.supportFactor };

        while (target == null)
        {
            // Pass if no usable skills
            if (factorArray[0] + factorArray[1] + factorArray[2] + factorArray[3] == 0f)
            {
                gameManager.Pass();
                return;
            }

            // Gets a list of skills of the particular chosen type
            skills = DetermineMoveType(factorArray[0], factorArray[1], factorArray[2], factorArray[3]);

            while (skills.Count != 0)
            {
                randMove = (NonPassiveSkill) skills[(int) Mathf.Floor(UnityEngine.Random.Range(0f, skills.Count))];
                
                List<GameObject> demons = gameManager.AliveDemons(gameManager.opponentTeam.GetComponent<Team>().activeDemons);
                target = DetermineTarget(randMove, demons);

                if (target != null)
                    break;

                skills.Remove(randMove);
            }

            factorArray[skillFactor] = 0;
        }

        //Debug.Log("MOVE: " + randMove);
        //Debug.Log("TARGET: " + target);
        gameManager.ExecuteMove(target, randMove, gameManager.opponentTeam.GetComponent<Team>().activeDemons);
    }

    // AI that picks a target first, then a skill to use on that target
    private void TargetDependentAI()
    {
        GameObject target = null;
        NonPassiveSkill randMove = null;
        List<GameObject> targets = null;
        List<Skill> skills = null;
        List<float> targetFactorArray;
        List<float> skillFactorArray;

        AIFactors activeFactors = gameManager.active.GetComponent<ActorStats>().aiFactors;
        if (useTeamFactors)
        {
            targetFactorArray = new List<float> { selfFactor, allyPlayerFactor, enemyPlayerFactor, allyTeamFactor, enemyTeamFactor };
            skillFactorArray = new List<float> { attackFactor, ailmentFactor, healFactor, supportFactor };
        }
        else
        {
            targetFactorArray = new List<float> { activeFactors.selfFactor, activeFactors.allyPlayerFactor, activeFactors.enemyPlayerFactor, activeFactors.allyTeamFactor, activeFactors.enemyTeamFactor };
            skillFactorArray = new List<float> { activeFactors.attackFactor, activeFactors.ailmentFactor, activeFactors.healFactor, activeFactors.supportFactor };
        } 

        if (gameManager.AliveTeammateCount(gameManager.opponentTeam.GetComponent<Team>().activeDemons) >= 3)
            targetFactorArray[2] = 0;

        while (randMove == null)
        {
            // Pass if no targets
            if (targetFactorArray[0] + targetFactorArray[1] + targetFactorArray[2] + targetFactorArray[3] + targetFactorArray[4] == 0f)
            {
                gameManager.Pass();
                return;
            }

            targets = DetermineAvailableTargets(targetFactorArray);

            while (targets.Count != 0)
            {
                target = targets[(int) Mathf.Floor(UnityEngine.Random.Range(0f, targets.Count))];

                if (targetFactor == 0 || targetFactor == 1 || targetFactor == 3)
                {
                    skills = DetermineMoveType(0, 0, skillFactorArray[2], skillFactorArray[3]);
                }
                else
                {
                    skills = DetermineMoveType(skillFactorArray[0], skillFactorArray[1], 0, skillFactorArray[3]);
                }

                while (skills.Count != 0)
                {
                    randMove = (NonPassiveSkill) skills[(int) Mathf.Floor(UnityEngine.Random.Range(0f, skills.Count))];

                    if (CheckIfLogical(randMove, target))
                        break;

                    skills.Remove(randMove);
                }

                targets.Remove(target);
                skillFactorArray[skillFactor] = 0;
            }

            targetFactorArray[targetFactor] = 0;
        }

        // Debug.Log("MOVE: " + randMove);
        // Debug.Log("TARGET: " + target);
        gameManager.ExecuteMove(target, randMove, gameManager.opponentTeam.GetComponent<Team>().activeDemons);
    }

    List<Skill> DetermineMoveType(float attackFactor, float ailmentFactor, float healFactor, float supportFactor)
    {
        // Add up the factors and generate a random number within the total range
        float totalFactor = attackFactor + ailmentFactor + healFactor + supportFactor;
        float randFactor = UnityEngine.Random.Range(0f, totalFactor);

        List<Skill> skills;
        // If the generated factor is an attack factor...
        if (randFactor < attackFactor)
        {
            skills = GetSkillList(0);
            skillFactor = 0;
        }
        // If the generated factor is an ailment factor...
        else if (randFactor < attackFactor + ailmentFactor)
        {
            skills = GetSkillList(1);
            skillFactor = 1;
        }
        // If the generated factor is a recovery factor...
        else if (randFactor < attackFactor + ailmentFactor + healFactor)
        {
            skills = GetSkillList(2);
            skillFactor = 2;
        }
        // If the generated factor is a support factor...
        else
        {
            skills = GetSkillList(3);
            skillFactor = 3;
        }

        return skills;
    }

    List<GameObject> DetermineAvailableTargets(List<float> targetFactorArray)
    {
        float totalFactor = targetFactorArray[0] + targetFactorArray[1] + targetFactorArray[2] + targetFactorArray[3] + targetFactorArray[4];
        float randFactor = UnityEngine.Random.Range(0f, totalFactor);

        List<GameObject> targets = new List<GameObject>();

        if (randFactor < targetFactorArray[0])
        {
            targets.Add(gameManager.active);
            targetFactor = 0;
        }
        else if (randFactor < targetFactorArray[0] + targetFactorArray[1])
        {
            targets.Add(gameManager.playerTeam.GetComponent<Team>().player);
            targetFactor = 1;
        }
        else if (randFactor < targetFactorArray[0] + targetFactorArray[1] + targetFactorArray[2])
        {
            targets.Add(gameManager.opponentTeam.GetComponent<Team>().player);
            targetFactor = 2;
        }
        else if (randFactor < targetFactorArray[0] + targetFactorArray[1] + targetFactorArray[2] + targetFactorArray[3])
        {
            targets.AddRange(gameManager.AliveDemons(gameManager.playerTeam.GetComponent<Team>().activeDemons));
            targetFactor = 3;
        }
        else
        {
            targets.AddRange(gameManager.AliveDemons(gameManager.opponentTeam.GetComponent<Team>().activeDemons));
            targetFactor = 4;
        }

        return targets;
    }

    List<Skill> GetSkillList(int moveType)
    {
        List<Skill> skills;
        skills = DetermineMoves(moveType);
        return skills;
    }

    List<GameObject> GetTargetList(NonPassiveSkill skill)
    {
        if (skill.skillID < GameManager.RECOVERY_ID || (skill.skillID >= GameManager.SUPPORT_ID && !((SupportSkill) skill).buff))
        {
            Debug.Log("Opponent Target");
            return gameManager.AliveDemons(gameManager.opponentTeam.GetComponent<Team>().activeDemons);
        }
        else
        {
            Debug.Log("Ally Target");
            return gameManager.AliveDemons(gameManager.playerTeam.GetComponent<Team>().activeDemons);
        }
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
        
        bool shouldExecute = CheckIfLogical(randMove, randTarget);
        if (shouldExecute)
            return randTarget;
        else
            return null;
    }

    bool CheckIfLogical(NonPassiveSkill randMove, GameObject randTarget)
    {
        bool shouldExecute = false;

        // Execute if attacking move
        if (randMove.GetType() == typeof(AttackSkill))
        {
            shouldExecute = AttackLogic((AttackSkill) randMove, randTarget);
        }

        // Execute if healing move
        else if (randMove.GetType() == typeof(RecoverySkill))
        {
            shouldExecute = RecoveryLogic((RecoverySkill) randMove, randTarget);
        }

        // Execute if support move
        else if (randMove.GetType() == typeof(SupportSkill))
        {
            shouldExecute = SupportLogic((SupportSkill) randMove, randTarget);
        }

        return shouldExecute;
    }

    bool AttackLogic(AttackSkill randMove, GameObject randTarget)
    {
        // Execute if attacking move
        if (randMove != null)
        {
            // Almighty skills cannot be blocked
            if(randMove.type == 7)
                return true;

            // Don't use move if target nulls move or better
            int resAmnt = gameManager.playerTeam.GetComponent<Team>().FindOpposingData(randTarget).resistances[randMove.type];
            if (resAmnt > 2 && resAmnt != 7)
            {
                return false;
            }
        }

        return true;
    }

    bool RecoveryLogic(RecoverySkill randMove, GameObject randTarget)
    {
        Debug.Log("Checked " + randTarget.name);
        ActorStats targetStats = randTarget.GetComponent<ActorStats>();

        // Do not heal if the target has full health
        if (targetStats.stats.battleStats.hp == targetStats.stats.baseStats.hp && (randMove.recoverAmnt != 0 || randMove.recoverPrct != 0) && !randMove.cure)
            return false;

        // Do not cure if the target doesn't have a status ailment
        if (targetStats.ailment[0] == 0 && randMove.recoverAmnt == 0 && randMove.recoverPrct == 0 && randMove.cure)
            return false;

        return true;
    }

    bool SupportLogic(SupportSkill randMove, GameObject randTarget)
    {
        ActorStats targetStats = randTarget.GetComponent<ActorStats>();

        // Do not use a move that can only be used on yourself on someone else
        if (randMove.selfOnly && randTarget != gameManager.active)
            return false;

        // Do not apply a charge if the target already has one
        if (targetStats.charge != 0 && randMove.charge.Count > 0)
            return false;

        // Do not apply a protective shield if the target already has one
        if (targetStats.protective != 0 && randMove.block.Count > 0)
            return false;

        // Do not buff the opponent team
        if (randMove.buff && gameManager.AliveDemons(gameManager.opponentTeam.GetComponent<Team>().activeDemons).Contains(randTarget))
            return false;

        // Do not debuff the player team
        if (!randMove.buff && gameManager.AliveDemons(gameManager.playerTeam.GetComponent<Team>().activeDemons).Contains(randTarget))
            return false;

        return true;
    }
}
