from serializer import *
import json
import random

DAMAGE_SKILL_INDEX = 182
AILMENT_SKILL_INDEX = 200
RECOVERY_SKILL_INDEX = 221
SUPPORT_SKILL_INDEX = 259
SIDE_EFFECT_RATE = 40

def openState():
    f = open('state.json', 'r')
    data = json.load(f)
    return data

def closeState(data):
    newState = to_json(data)
    with open('state.json', 'w') as f:
        f.write(newState)

def nextTurn(data, amnt=0):

    # Glowing turn
    if amnt == 1:
        data["pressTurns"][0] += 1
        data["pressTurns"][1] -= 1

    # Decrease press turns
    else:
        count = 1
        # If move missed/nullified
        if amnt == 2:
            count = 2
        # If move was repelled/drained
        if amnt == 3:
            count = 4

        for x in range(0, count):
            if data["pressTurns"][0] != 0:
                data["pressTurns"][0] -= 1
            else:
                data["pressTurns"][1] -= 1

    # Switch team if needed
    if data["pressTurns"][1] <= 0:
        if data["turn"][0] == 0:
            data["turn"] = [1, 0]
        else:
            data["turn"] = [0, 0]
        data["pressTurns"] = [0, 4]

    # Else call next demon
    else:
        if data["turn"] == 3:
            data["turn"][1] = 0
        else:
            data["turn"][1] += 1
    
    return

def determineParty(data, attacker=True):
    if (data["turn"][0] == 0 and attacker) or (data["turn"][1] and not attacker):
        return data["party1"]
    else:
        return data["party2"]

def determineActor(data, attacking=True):
    team = determineParty(data, attacking)
    
    if data["turn"][1] == 0:
        return team["player"]
    return team["active"][data["turn"][1] - 1]

def actionTypeParser(attacker, action):
    actionID = attacker["skills"][action]["skillID"]
    # Attacking Skill
    if actionID < DAMAGE_SKILL_INDEX:
        return 0
    # Ailment Skill
    elif actionID < AILMENT_SKILL_INDEX:
        return 1
    # Recovery Skill
    elif actionID < RECOVERY_SKILL_INDEX:
        return 2
    # Support Skill
    elif actionID < SUPPORT_SKILL_INDEX:
        return 3

def attack():
    return

def performAction(data, action):
    global outcome
    attacker = determineActor(data, True)
    defender = determineActor(data, False)
    actionType = actionTypeParser(attacker, action[0])

    if actionType == 0:
        outcome = calculateDamage(data, attacker["skills"][action[0]], attacker, defender)
    
    nextTurn(data, 1)
    return

def calculateDamage(data, skill, attacker, defender):
    damage = 0
    defense = determineParty(data, False)

    for i in range(len(defense.active)):
        if (defense.active[i].boosts and defense.active[i].boosts[4][0] != 0):
            taunt_chance = 30 * (((defense.active[i].baseStats.luck / 2) + (defense.active[i].baseStats.agility / 4))
                                / ((attacker.baseStats.luck / 2) + (attacker.baseStats.agility / 4)))

            if (random.random() * 100 < taunt_chance):
                print("{} TOOK THE ATTACK MEANT FOR {}".format(defense.active[i].name, defender.name))
                defender = defense.active[i]
                break

    res_mod = calculateResistance(skill, attacker, defender)
    affected = defender
    charge_val = 1

    # PROTECTIVE BARRIER
    if (defender.boosts[2] == skill.type + 3):
        res_mod[0] = 0
        res_mod[1] = "NULL!"

    # REFLECTIVE BARRIER
    if ((defender.boosts[2] == 1 and skill.type == 0) or (defender.boosts[2] == 2 and skill.type > 1 and skill.type < 7)):
        res_mod[0] = res_mod[0]
        res_mod[1] = "REFLECT!"

    if (res_mod[0] > 0):
        # GUARDING
        if (defender.guard == 1 and res_mod[0] > 0.5):
            res_mod[1] = "GUARD!"
            res_mod[0] = 0.8

        # MISS
        if (not calculateHit(skill, attacker, defender, 0)):
            res_mod[1] = "MISS!"
            res_mod[0] = 0

        # CRITICAL
        else:
            if (calculateCritRate(skill, attacker, defender)):
                res_mod[1] += " CRITICAL!"
                res_mod[0] *= (1.5 * (1 + (attacker.passives[21] * 0.3)))
            else:
                res_mod[0] *= (1 - (attacker.passives[21] * 0.1))

    # REFLECT
    if (res_mod[0] == -0.5 or (defender.boosts[2] == 1 and skill.type == 0) or (defender.boosts[2] == 2 and skill.type > 1 and skill.type < 7)):
        res_mod[1] = "REPEL!"
        res_mod[0] = 1
        affected = attacker

    # CALCULATES OHKO IF WEAK
    if (res_mod[0] == 2 and skill.ohko and calculateInstaKill(skill, attacker, defender)):
        res_mod[1] += " OHKO!"
        affected.battleStats.hp = 0

    # CALCULATES CHARGE
    if (attacker.boosts[1] == 3 or (skill.physical and attacker.boosts[1] == 1) or (not skill.physical and attacker.boosts[1] == 2)):
        charge_val = 1.8

    dem_power = 0
    if skill.physical:
            dem_power = attacker.base_stats.strength
    else:
        dem_power = attacker.base_stats.magic

    damage = int(
        (
            dem_power ** 2
            / (defender.base_stats.vitality * 1.5)
            * (1 + (skill.power / 100))
            * res_mod[0]
            * ((random.random() / 3) + 1)
            * calculate_battle_buff(attacker, 0)
            * (2 - calculate_battle_buff(defender, 1))
            * (1 - (defender.boosts[3] * 0.3))
            * charge_val
            * passive_increaser(attacker.passives[skill.type + 2])
            + (1 * res_mod[0])
        )
    )

    affected.battle_stats.hp -= damage

    # IN CASE DRAIN OVERHEALS
    if affected.battle_stats.hp > affected.base_stats.hp:
        affected.battle_stats.hp = affected.base_stats.hp

    if skill.name == "Attack":
        print(f"{attacker.name} ATTACKS {affected.name} FOR {damage}HP!{res_mod[1]}")
    else:
        print(f"{attacker.name} USES {skill.name} ON {affected.name} FOR {damage}HP!{res_mod[1]}")

    result = 0
    if res_mod[1][1:6] in ["WEAK!", "CRITI"]:
        if attacker.passives[12] != 0:
            heal_amnt = (
                restore_amnt(attacker.passives[12])
                if restore_amnt(attacker.passives[12])
                > attacker.base_stats.hp - attacker.battle_stats.hp
                else attacker.base_stats.hp - attacker.battle_stats.hp
            )
            attacker.battle_stats.hp += heal_amnt
            print(f"{attacker.name} WAS HEALED FOR {heal_amnt}HP!")
        result = 1
    elif res_mod[1][1:6] in ["NULL!", "MISS!"]:
        result = 2
    elif res_mod[1][1:6] in ["REPEL", "DRAIN"]:
        result = 3

    # Apply additional support effects if move wasn't blocked/evaded/reflected/absorbed
    if result < 2 and skill.support:
        calculate_support(skill, affected, affected)

    return result



def calculateHit(skill, attacker, defender, ail_type):
    # Guaranteed hit if the opponent is asleep or the user is boosted by Critical Aura
    if defender["boosts"][0][0] == 5 or defender["boosts"][1] == 4:
        return True

    accuracy = skill.get("accuracy", 1) * (calculateDemonHitAvoid(attacker) / calculateDemonHitAvoid(defender)) * passiveIncreaser(defender["passives"][15])

    # If ailment skill, take the defender's resistances to determine accuracy
    if skill["skillID"] >= DAMAGE_SKILL_INDEX:
        accuracy *= (1 / defender.get("ailmentResistances", {}).get(ail_type, 1)) * passiveDecreaser(defender.get("passives", {}).get(11))

    # If attacker is inflicted with mirage, lower accuracy
    if attacker["boosts"][0][0] == 6:
        accuracy /= 2

    if (random.random() * 100) < accuracy:
        return True
    else:
        return False

def calculateDemonHitAvoid(demon):
    return (demon["baseStats"]["agility"] / 2.0) + (demon["baseStats"]["luck"] / 4.0)  * calculateBattleBuff(demon, 2)


def calculateCritRate(skill, attacker, defender):
    crit_bonus = skill.get("critBonus", 0)
    if type(crit_bonus) != int:
        crit_bonus = 0

    # Guaranteed crit moves
    if crit_bonus == 200:
        return True

    # CRITICAL AURA
    if skill.get("physical") and attacker.get("boosts", {}).get(1) == 4:
        return True

    crit = crit_bonus + (calculateDemonCritAvoid(attacker) / calculateDemonCritAvoid(defender)) * passiveIncreaser(attacker["passives"][16]) + 6.25

    if (random.random() * 100) < crit:
        return True
    else:
        return False

def calculateDemonCritAvoid(demon):
    return (demon["baseStats"]["luck"] / 2.0) + (demon["baseStats"]["agility"] / 4.0)

def calculateBattleBuff(demon, type):
    return (1 + demon["boosts"][type + 5][0] * 0.2)

def calculateInstaKill(skill, attacker, defender):
    kill_chance = skill.get("ohko", 0) * (calculateDemonInstaKillChance(attacker) / calculateDemonInstaKillChance(defender)) * passiveDecreaser(defender.get("passives", {}).get(11))
    if (random.random() * 100) < kill_chance:
        return True
    else:
        return False

def calculateDemonInstaKillChance(demon):
    return (demon["baseStats"]["luck"] / 2.0) + (demon["baseStats"]["agility"] / 4.0)

def passiveIncreaser(amount):
    if amount == 1:
        return 1.2
    elif amount == 2:
        return 1.35
    elif amount == 3:
        return 1.55
    else:
        return 1

def passiveDecreaser(amount):
    if amount == 1:
        return 0.8
    elif amount == 2:
        return 0.65
    elif amount == 3:
        return 0.45
    else:
        return 1

def nextState(action=[0, 0]):
    data = openState()
    performAction(data, action)
    closeState(data)
    return

nextState([1, 0])
print("Complete!")