from serializer import *
import copy
import json
import random

DAMAGE_SKILL_INDEX = 182
AILMENT_SKILL_INDEX = 200
RECOVERY_SKILL_INDEX = 221
SUPPORT_SKILL_INDEX = 259
SIDE_EFFECT_RATE = 40

def openState(filename):
    f = open(filename, 'r')
    data = json.load(f)
    return data

def closeState(data, filename):
    newState = to_json(data)
    with open(filename, 'w') as f:
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
    if (data["turn"][0] == 0 and attacker) or (data["turn"][0] == 1 and not attacker):
        return data["party1"]
    else:
        return data["party2"]

def determineAttacker(data):
    team = determineParty(data, True)
    
    if data["turn"][1] == 0:
        return team["player"]
    return team["active"][data["turn"][1] - 1]

def determineDefender(data, target, attackingParty=False):
    
    team = determineParty(data, attackingParty)
    
    return team["active"][target]

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
    attacker = determineAttacker(data)
    actionType = actionTypeParser(attacker, action[0])
    skill = attacker["skills"][action[0]]

    if actionType == 0:
        defender = determineDefender(data, action[1], False)
        outcome = calculateDamage(data, skill, attacker, defender)
    if actionType == 1:
        defender = determineDefender(data, action[1], False)
        outcome = calculateAilment(skill, attacker, defender)
    if actionType == 2:
        defender = determineDefender(data, action[1], True)
        outcome = calculateHeal(skill, attacker, defender)
    if actionType == 3:
        if "selfOnly" in skill:
            defender = attacker
        elif "blockID" in skill or skill["buff"]:
            defender = determineDefender(data, action[1], True)
        else:
            defender = determineDefender(data, action[1], False)
        outcome = calculateSupport(skill, attacker, defender)
    
    turnEnd(data, attacker)
    nextTurn(data, 1)
    return

def calculateDamage(data, skill, attacker, defender):
    damage = 0
    defense = determineParty(data, False)

    for i in range(len(defense["active"])):
        if (defense["active"][i]["boosts"] and defense["active"][i]["boosts"][4][0] != 0):
            taunt_chance = 30 * (((defense["active"][i]["baseStats"]["luck"] / 2) + (defense["active"][i]["baseStats"].agility / 4))
                                / ((attacker["baseStats"]["luck"] / 2) + (attacker["baseStats"].agility / 4)))

            if (random.random() * 100 < taunt_chance):
                # print("{} TOOK THE ATTACK MEANT FOR {}".format(defense["active"][i]["name"], defender["name"]))
                defender = defense["active"][i]
                break

    res_mod = calculateResistance(skill, attacker, defender)
    affected = defender
    charge_val = 1

    # PROTECTIVE BARRIER
    if (defender["boosts"][2] == skill["type"] + 3):
        res_mod[0] = 0
        res_mod[1] = "NULL!"

    # REFLECTIVE BARRIER
    if ((defender["boosts"][2] == 1 and skill["type"] == 0) or (defender["boosts"][2] == 2 and skill["type"] > 1 and skill["type"] < 7)):
        res_mod[0] = res_mod[0]
        res_mod[1] = "REFLECT!"

    if (res_mod[0] > 0):
        # GUARDING
        if (defender["guard"] == 1 and res_mod[0] > 0.5):
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
                res_mod[0] *= (1.5 * (1 + (attacker["passives"][21] * 0.3)))
            else:
                res_mod[0] *= (1 - (attacker["passives"][21] * 0.1))

    # REFLECT
    if (res_mod[0] == -0.5 or (defender["boosts"][2] == 1 and skill["type"] == 0) or (defender["boosts"][2] == 2 and skill["type"] > 1 and skill["type"] < 7)):
        res_mod[1] = "REPEL!"
        res_mod[0] = 1
        affected = attacker

    # CALCULATES OHKO IF WEAK
    if (res_mod[0] == 2 and "ohko" in skill and calculateInstaKill(skill, attacker, defender)):
        res_mod[1] += " OHKO!"
        affected["battleStats"]["hp"] = 0

    # CALCULATES CHARGE
    if (attacker["boosts"][1] == 3 or (skill["physical"] and attacker["boosts"][1] == 1) or (not skill["physical"] and attacker["boosts"][1] == 2)):
        charge_val = 1.8

    # MAGIC OR STRENGTH
    dem_power = 0
    if skill["physical"]:
            dem_power = attacker["baseStats"]["strength"]
    else:
        dem_power = attacker["baseStats"]["magic"]

    damage = int(
        (
            dem_power ** 2
            / (defender["baseStats"]["vitality"] * 1.5)
            * (1 + (skill["power"] / 100))
            * res_mod[0]
            * ((random.random() / 3) + 1)
            * calculateBattleBuff(attacker, 0)
            * (2 - calculateBattleBuff(defender, 1))
            * (1 - (defender["boosts"][3] * 0.3))
            * charge_val
            * passiveIncreaser(attacker["passives"][skill["type"] + 2])
            + (1 * res_mod[0])
        )
    )

    affected["battleStats"]["hp"] -= damage

    # IN CASE DRAIN OVERHEALS
    if affected["battleStats"]["hp"] > affected["baseStats"]["hp"]:
        affected["battleStats"]["hp"] = affected["baseStats"]["hp"]

    # if skill["name"] == "Attack":
        # print(f"{attacker["name"]} ATTACKS {affected["name"]} FOR {damage}HP!{res_mod[1]}")
    # else:
        # print(f"{attacker["name"]} USES {skill["name"]} ON {affected["name"]} FOR {damage}HP!{res_mod[1]}")

    result = 0
    if res_mod[1][1:6] in ["WEAK!", "CRITI"]:
        if attacker["passives"][12] != 0:
            heal_amnt = (
                restoreAmnt(attacker["passives"][12])
                if restoreAmnt(attacker["passives"][12])
                > attacker["baseStats"]["hp"] - attacker["battleStats"]["hp"]
                else attacker["baseStats"]["hp"] - attacker["battleStats"]["hp"]
            )
            attacker["battleStats"]["hp"] += heal_amnt
            # print(f"{attacker["name"]} WAS HEALED FOR {heal_amnt}HP!")
        result = 1
    elif res_mod[1][1:6] in ["NULL!", "MISS!"]:
        result = 2
    elif res_mod[1][1:6] in ["REPEL", "DRAIN"]:
        result = 3

    # Apply additional support effects if move wasn't blocked/evaded/reflected/absorbed
    if result < 2 and "support" in skill:
        calculateSupport(skill, affected, affected)

    return result

def calculateAilment(skill, active, defender):
    for i in range(len(skill["ailments"])):
        ailType = getAilmentType(skill["ailments"][i])
        if defender["ailmentResistances"][ailType] == 3:
            # print(f"{defender["name"]} AVOIDED BEING INFLICTED WITH {ailType.upper()}! NULL!")
            return 2
        elif defender["ailmentResistances"][ailType] == 0:
            # print(f"{defender["name"]} BECAME INFLICTED WITH {ailType.upper()}! WEAK!")
            calculateSupport(skill, active, defender)
            curseSiphon(active)
            defender["boosts"][0] = [skill["ailments"][i] + 1, 0, active["passives"][17]]
            return 1
        else:
            if calculateHit(skill, active, defender, ailType):
                # print(f"{defender["name"]} BECAME INFLICTED WITH {ailType.upper()}!")
                calculateSupport(skill, active, defender)
                curseSiphon(active)
                defender["boosts"][0] = [skill["ailments"][i] + 1, 0, active["passives"][17]]
                return 0
            # else:
                # print(f"{defender["name"]} AVOIDED BEING INFLICTED WITH {ailType.upper()}!")
    return 0

def getAilmentType(num):
    if num == 0:
        return "poison"
    if num == 1:
        return "confusion"
    if num == 2:
        return "charm"
    if num == 3:
        return "seal"
    if num == 4:
        return "sleep"
    if num == 5:
        return "mirage"
    
def calculateHeal(skill, active, recipient):
    recovery = 1
    overheal = 0

    # BOWL OF HYGIEIA
    if active["boosts"][1] == 5:
        recovery = 1.5
        overheal = 1

    if "recoverAmnt" in skill:
        recovery *= int(skill["recoverAmnt"] * (((active["baseStats"]["magic"]**2) / 225) + 1))
    else:
        recovery *= int((skill["recoverPrct"] / 100) * recipient["baseStats"]["hp"])

    # print(f"RECOVERY: {recovery}")

    # If recovery is greater than the allowed limit, reduce it
    # Overheal formula
    if recipient["battleStats"]["hp"] + recovery > recipient["baseStats"]["hp"] * overhealExpr(overheal):
        recovery = int((recipient["baseStats"]["hp"] * overhealExpr(overheal)) - recipient["battleStats"]["hp"])

    recipient["battleStats"]["hp"] += recovery
    # print(f"{active["name"]} USES {skill["name"]} ON {recipient["name"]} FOR {recovery}HP!")

# Returns 1 if false, 1.3 if true
def overhealExpr (overheal):
    return (1 + (overheal * 0.3))

def calculateSupport(skill, caster, recipient):
    if "support" in skill:
        calculateBuff(skill, caster, recipient)
    if "chargeID" in skill:
        calculateCharge(skill, recipient)
    if "blockID" in skill:
        calculateProtection(skill, recipient)
    if "veil" in skill:
        calculateDamageDown(recipient)
    if "taunt" in skill:
        calculateTaunt(recipient)
        
def calculateBuff(skill, caster, recipient):
    quote = ""
    for i in range(len(skill["support"])):
        if skill["support"][i] == 0:
            quote = buffHelper(skill["support"][i], skill, caster, recipient)
            # print("{}'s ATTACK WAS {}".format(recipient["name"], quote))
        elif skill["support"][i] == 1:
            quote = buffHelper(skill["support"][i], skill, caster, recipient)
            # print("{}'s DEFENSE WAS {}".format(recipient["name"], quote))
        elif skill["support"][i] == 2:
            quote = buffHelper(skill["support"][i], skill, caster, recipient)
            # print("{}'s ACCURACY/EVASION WAS {}".format(recipient["name"], quote))
        elif skill["support"][i] == 3:
            for i in range(5, 8):
                recipient["boosts"][i] = [0, 0]
            # print("{}'s STATS RETURNED TO NORMAL".format(recipient["name"]))

def buffHelper(effect, skill, caster, recipient):
    quote = ""

    # If buff...
    if skill["buff"] and recipient["boosts"][effect + 5][0] < 2:
        recipient["boosts"][effect + 5][0] += skill["suppAmnt"]
        quote = "RAISED BY {} STAGE(S)!".format(skill["suppAmnt"])
        if recipient["boosts"][effect + 5][0] >= 2:
            quote = "RAISED BY {} STAGE(S)! REACHED MAXIMUM AMOUNT!".format(skill["suppAmnt"])
    # If debuff...
    elif not skill["buff"] and recipient["boosts"][effect + 5][0] > -2:
        recipient["boosts"][effect + 5][0] -= skill["suppAmnt"]
        quote = "LOWERED BY {} STAGE(S)!".format(skill["suppAmnt"])
        if recipient["boosts"][effect + 5][0] <= -2:
            quote += " REACHED MINIMUM AMOUNT!"
    # If nothing happens...
    else:
        quote = "UNCHANGED!"

    # Boost timer resets everytime a new modifier is added
    recipient["boosts"][effect + 5][1] = 3 + caster["passives"][18]

    return quote
        
def calculateCharge(skill, recipient):
    recipient["boosts"][1] = skill["chargeID"][0] + 1
    # print(f"{recipient["name"]} IS CHARGING ITS POWER...")

def removeCharge(skill, recipient):
    if (skill["skillID"] < DAMAGE_SKILL_INDEX and (skill["physical"] and (recipient["boosts"][1] == 1 or recipient["boosts"][1] == 4) 
            or (not skill["physical"]) and recipient["boosts"][1] == 2 or recipient["boosts"][1] == 3) 
            or (skill["skillID"] < RECOVERY_SKILL_INDEX and recipient["boosts"][1] == 5)):
        recipient["boosts"][1] = 0

        
def calculateProtection(skill, recipient):
    recipient["boosts"][2] = skill["blockID"] + 1
    # print(f"{recipient['name']} IS BEING PROTECTED BY A MAGICAL BARRIER...")

def expireProtection(party):
    if ((party["player"]["boosts"] and party["player"]["boosts"][2] != 0) or party["player"]["boosts"][3] != 0):
        # print(f"{party['player']['name']}'S PROTECTIVE BARRIER DISSIPATED...")
        party["player"]["boosts"][2] = 0
        party["player"]["boosts"][3] = 0
    for i in range(len(party["active"])):
        if ((party["active"][i]["boosts"] and party["active"][i]["boosts"][2] != 0)
                or (party["active"][i]["boosts"] and party["active"][i]["boosts"][3] != 0)):
            # print(f"{party['active'][i]['name']}'S PROTECTIVE BARRIER DISSIPATED...")
            party["active"][i]["boosts"][2] = 0
            party["player"]["boosts"][3] = 0

def breakProtection(skill, demon):
    # Resets protective status is used
    if ((demon["boosts"][2] == skill["type"] + 3) or (demon["boosts"][2] == 1 and skill["type"] == 0)
            or (demon["boosts"][2] == 2 and skill["type"] > 0 and skill["type"] < 7)):
        # print(f"{demon['name']}'S PROTECTIVE BARRIER SHATTERED!")
        demon["boosts"][2] = 0

    # Awakens sleeping demons if hit
    elif demon["boosts"][0][0] == 5:
        # print(f"{demon['name']} WAS ABRUPTLY AWAKENED!")
        demon["boosts"][0] = [0, 0, 0]
        
def calculateDamageDown(recipient):
    recipient["boosts"][3] = 1
    # print(f"{recipient['name']} IS BEING PROTECTED BY A MAGICAL BARRIER...")

def calculateTaunt(recipient):
    recipient["boosts"][4] = [1, 3]
    # print(f"{recipient['name']} IS DRAWING IN THE ENEMY'S FOCUS")

def calculateHit(skill, attacker, defender, ail_type):
    # Guaranteed hit if the opponent is asleep or the user is boosted by Critical Aura
    if defender["boosts"][0][0] == 5 or defender["boosts"][1] == 4:
        return True

    accuracy = skill["accuracy"] * (calculateDemonHitAvoid(attacker) / calculateDemonHitAvoid(defender)) * passiveIncreaser(defender["passives"][15])

    # If ailment skill, take the defender's resistances to determine accuracy
    if skill["skillID"] >= DAMAGE_SKILL_INDEX:
        accuracy *= (1 / defender["ailmentResistances"][ail_type]) * passiveDecreaser(defender["passives"][11])

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
    if "critBonus" in skill:
        crit_bonus = skill["critBonus"]
    else:
        crit_bonus = 0

    # Guaranteed crit moves
    if crit_bonus == 200:
        return True

    # CRITICAL AURA
    if skill["physical"] and attacker["boosts"][1] == 4:
        return True

    crit = crit_bonus + (calculateDemonCritAvoid(attacker) / calculateDemonCritAvoid(defender)) * passiveIncreaser(attacker["passives"][16]) + 6.25

    if (random.random() * 100) < crit:
        return True
    else:
        return False

def calculateDemonCritAvoid(demon):
    return (demon["baseStats"]["luck"] / 2.0) + (demon["baseStats"]["agility"] / 4.0)

def calculateResistance(skill, attacker, defender):
    skillType = ""

    if skill["type"] == 0:
        skillType = "physical"
    elif skill["type"] == 1:
        skillType = "fire"
    elif skill["type"] == 2:
        skillType = "ice"
    elif skill["type"] == 3:
        skillType = "electric"
    elif skill["type"] == 4:
        skillType = "force"
    elif skill["type"] == 5:
        skillType = "light"
    elif skill["type"] == 6:
        skillType = "dark"
    elif skill["type"] == 7:
        return [1, ""]

    if skill["pierce"] or attacker["boosts"][1] == 3:
        if defender["resistances"][skillType] == 0:
            return [2, "WEAK!"]
        else:
            return [1, ""]

    if defender["resistances"][skillType] == 0:
        return [2, " WEAK!"]
    elif defender["resistances"][skillType] == 1:
        return [1, ""]
    elif defender["resistances"][skillType] == 2:
        return [0.5, " RESIST!"]
    elif defender["resistances"][skillType] == 3:
        return [0, " NULL!"]
    elif defender["resistances"][skillType] == 4:
        return [-0.5, " REPEL!"]
    elif defender["resistances"][skillType] == 5:
        return [-1, " DRAIN!"]


def calculateBattleBuff(demon, type):
    return (1 + demon["boosts"][type + 5][0] * 0.2)

def calculateInstaKill(skill, attacker, defender):
    kill_chance = skill["ohko"] * (calculateDemonInstaKillChance(attacker) / calculateDemonInstaKillChance(defender)) * passiveDecreaser(defender["passives"][11])
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
    
def restoreAmnt(amount):
    if amount == 1:
        return 10
    elif amount == 2:
        return 20
    elif amount == 3:
        return 30
    
def curseSiphon(demon):
    if demon["passives"][13] != 0:
        heal_amnt = restoreAmnt(demon["passives"][13]) if restoreAmnt(demon["passives"][13]) <= demon["baseStats"]["mp"] - demon["battleStats"]["mp"] else demon["baseStats"]["mp"] - demon["battleStats"]["mp"]
        demon["battleStats"]["mp"] += heal_amnt
        # print(f"{demon["name"]} REGAINED {heal_amnt}MP!")

def poisonRate(amnt):
    if amnt == 0:
            return 16
    if amnt == 1:
            return 12
    if amnt == 2:
            return 8
    if amnt == 3:
            return 4
        
def turnEnd(data, attacker):
    if attacker["boosts"][0][0] == 1:
        damage = int(attacker["baseStats"]["hp"] / poisonRate(attacker["boosts"][0][2]))
    
        if damage > attacker["battleStats"]["hp"]:
            damage = attacker["battleStats"]["hp"] - 1

        # print(f"{attacker['name']} TOOK {damage}HP WORTH OF POISON DAMAGE!")
        attacker["battleStats"]["hp"] -= damage

        if attacker["battleStats"]["hp"] <= 1:
            attacker["boosts"][0] = [0, 0, 0]
            # print(f"{attacker['name']} IS NO LONGER AFFLICTED WITH POISON!")

    if not checkIfPlayersAlive(data):
        completed(data)

def checkIfPlayersAlive(data):
    if data["party1"]["player"]["battleStats"]["hp"] <= 0:
        # print(f"{self.attack['player']['name']} HAS BEEN DEFEATED! {self.defense['player']['name']} HAS WON THE BATTLE")
        return False
    elif data["party2"]["player"]["battleStats"]["hp"] <= 0:
        # print(f"{self.defense['player']['name']} HAS BEEN DEFEATED! {self.attack['player']['name']} HAS WON THE BATTLE")
        return False
    return True


def nextState(filename, action=[0, 0]):
    data = openState(filename)
    performAction(data, action)
    closeState(data, filename)
    return data

def completed(data):
    data["complete"] = True

# nextState([1, 0])
# print("Complete!")