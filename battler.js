/*********************************************************************************
 * battler.js
 * 
 * Controls gameplay
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

import promptSync from 'prompt-sync'
import {PassiveWriter} from './data/skills/passiveWriter.js'

const prompt = promptSync()
const passiveWriter = new PassiveWriter()

const DAMAGE_SKILL_INDEX = 182
const AILMENT_SKILL_INDEX = 200
const RECOVERY_SKILL_INDEX = 221
const SUPPORT_SKILL_INDEX = 259
const SIDE_EFFECT_RATE = 40

export class Battler {

    constructor(attackingParty, defendingParty)
    {
        this.attack = attackingParty
        this.defense = defendingParty

        for (let j = 0; j < this.attack.player.skills.length; ++j) {
            if (this.attack.player.skills[j].skillID >= SUPPORT_SKILL_INDEX) {
                passiveWriter.updateWithPassive(this.attack.player, this.attack.player.skills[j])
            }
        }

        for (let i = 0; i < this.attack.actors.length; ++i) {
            for (let j = 0; j < this.attack.actors[i].skills.length; ++j) {
                if (this.attack.actors[i].skills[j].skillID >= SUPPORT_SKILL_INDEX) {
                    passiveWriter.updateWithPassive(this.attack.actors[i], this.attack.actors[i].skills[j])
                }
            }
        }

        for (let j = 0; j < this.defense.player.skills.length; ++j) {
            if (this.defense.player.skills[j].skillID >= SUPPORT_SKILL_INDEX) {
                passiveWriter.updateWithPassive(this.defense.player, this.defense.player.skills[j])
            }
        }

        for (let i = 0; i < this.defense.actors.length; ++i) {
            for (let j = 0; j < this.defense.actors[i].skills.length; ++j) {
                if (this.defense.actors[i].skills[j].skillID >= SUPPORT_SKILL_INDEX) {
                    passiveWriter.updateWithPassive(this.defense.actors[i], this.defense.actors[i].skills[j])
                }
            }
        }
    }

    begin() {
        console.log(`BATTLE HAS BEGUN BETWEEN ${this.attack.player.name} AND ${this.defense.player.name}`)
        this.turn()
    }

    turn() {
        while(true) {
            let counter = 0
            let activeDemons = this.amountOfActiveDemons(this.attack)
            let maxDemons = this.maxActive(this.attack)

            this.attack.pressTurns = [activeDemons + 1, 0]
            while (!(this.attack.pressTurns[0] <= 0 && this.attack.pressTurns[1] <= 0)) {
                console.log(`TURNS LEFT: ${this.attack.pressTurns}`)
                let active = {}
                while(Object.keys(active).length == 0) {
                    if ((counter % (maxDemons + 1)) == 0) {
                        active = this.attack.player
                    }
                    else {
                        if (this.attack.active[(counter % (maxDemons + 1)) - 1].battleStats) {
                            active = this.attack.active[(counter % (maxDemons + 1)) - 1]
                        }
                    }
                    ++counter
                }

                active.guard = 0
                let back = true

                // Execute if the current demon is afflicted with ailments
                if (active.boosts[0][0] != 0) {
                    if ((Math.random() * 100) <= active.boosts[0][1] * 25) {
                        console.log(`${active.name} IS NO LONGER AFFLICTED WITH ${this.getAilmentType(--active.boosts[0][0]).toUpperCase()}!`)
                        active.boosts[0] = [0, 0, 0]
                    }
                    else {
                        // If asleep...
                        if (active.boosts[0][0] == 5) {
                            console.log(`${active.name} IS SLEEPING...`)
                            this.loseTurn(1)
                            back = false
                        }

                        // If confused or charmed...
                        if ((active.boosts[0][0] == 2 || active.boosts[0][0] == 3) && (Math.random() * 100) < SIDE_EFFECT_RATE) {
                            console.log(`${active.name} IS AFFECTED BY ${this.getAilmentType(active.boosts[0][0] - 1).toUpperCase()}! IMMOBILIZED`)
                            this.loseTurn(1)
                            back = false
                        }

                        ++active.boosts[0][1]
                    }
                }

                while(back) {
                    console.log(`WHAT WILL ${active.name} DO?`)
                    console.log(`0: SKILLS\n1: ITEMS\n2: GUARD\n3: TALK\n4: CHANGE\n5: ESCAPE\n6: PASS\n`)
                    const choice = prompt()
                    switch (choice) {
                        case '0':
                            back = this.skill(active)
                            break
                        case '1':
                            back = this.item(active)
                            break
                        case '2':
                            back = this.guard(active)
                            break
                        case '3':
                            back = this.talk(active)
                            break
                        case '4':
                            if (active == this.attack.player) {
                                back = this.changePlayer()
                            }
                            else {
                                back = this.change(active)
                            }
                            break
                        case '5':
                            back = this.escape(this.attack.player)
                            return
                        case '6':
                            back = this.extraTurn()
                            break
                        default:
                            console.log('NOT VALID ATM')
                    }

                    // If the active demon is poisoned, calculate damage
                    if (active.boosts[0][0] == 1) {
                        console.log(active.boosts[0])
                        let damage = Math.floor(active.baseStats.hp / this.poisonRate(active.boosts[0][2]))

                        if (damage > active.battleStats.hp) {
                            damage = active.battleStats.hp - 1
                        }

                        console.log(`${active.name} TOOK ${damage}HP WORTH OF POISON DAMAGE!`)
                        active.battleStats.hp -= damage

                        if (active.battleStats.hp <= 1) {
                            active.boosts[0] = [0, 0, 0]
                            console.log(`${active.name} IS NO LONGER AFFLICTED WITH POISON!`)
                        }
                    }

                    if (!this.checkIfPlayersAlive()) {
                        return
                    }
                }
            }
            console.log(`SWITCH!!!`)

            // Switches sides
            let temp = this.attack
            this.attack = this.defense
            this.defense = temp

            this.reduceBuffDurations(this.attack)
            this.expireProtection(this.attack)
        }
    }

    skill(active) {
        let back = true
        while (back) {
            // Allows actors to use a regular attack if needed
            console.log(`${active.battleStats.mp} MP LEFT`)
            console.log(`0: Regular Attack`)

            let skillList = []

            // Only allows skills to be used if the current demon isn't afflicted with seal
            if (active.boosts[0][0] != 4) {
                for (let i = 0; i < active.skills.length; ++i)
                {
                    // Don't display passive skills or skills with insufficient mp costs
                    if (active.skills[i].skillID < SUPPORT_SKILL_INDEX && active.battleStats.mp >= active.skills[i].cost) {
                        skillList.push(active.skills[i])
                        console.log(`${skillList.length}: ${active.skills[i].name} - ${active.skills[i].cost}MP`)
                    }
                }
            }

            console.log(`${skillList.length + 1}: BACK`)

            let skillChoice = parseInt(prompt())

            if (skillChoice == skillList.length + 1) {
                return true
            }

            if (skillChoice == 0) {
                back = this.selectOpponent({
                    name: "Attack",
                    skillID: 0,
                    cost: 0,
                    targets: 0,
                    power: 100,
                    type: 0,
                    accuracy: 98,
                    hits: [1],
                    physical: true,
                    pierce: false
                }, active)
            }

            // All damage/ailment skills affect opponents
            else if (skillList[--skillChoice].skillID < AILMENT_SKILL_INDEX) {
                back = this.selectOpponent(skillList[skillChoice], active)
            }

            // All recovery skills affect allies
            else if (skillList[skillChoice].skillID < RECOVERY_SKILL_INDEX) {
                back = this.selectAlly(skillList[skillChoice], active)
            }

            else {
                // Skills which raise ally buffs, block, protect, charge, or taunt must call selectAlly
                if (skillList[skillChoice].buff || skillList[skillChoice].blockID || skillList[skillChoice].veil
                        || skillList[skillChoice].chargeID || skillList[skillChoice].taunt) {
                    back = this.selectAlly(skillList[skillChoice], active)
                }
                else {
                    back = this.selectOpponent(skillList[skillChoice], active)
                }
            }

            this.removeCharge(skillList[skillChoice], active)
        }
        return false
    }

    selectOpponent(skill, active) {
        let aliveActs = []

        for (let i = 0; i < this.maxActive(this.defense); ++i)
        {
            if (this.defense.active[i].battleStats) {
                aliveActs.push(this.defense.active[i])
            }
        }

        //If player can be targeted, allow it
        if (aliveActs.length <= 2)
        {
            aliveActs.unshift(this.defense.player)
        }

        if (skill.targets == 0)
        {
            for (let i = 0; i < aliveActs.length; ++i)
            {
                console.log(`${i}: ${aliveActs[i].name} - ${aliveActs[i].battleStats.hp}HP`)
            }

            console.log(`${aliveActs.length}: BACK`)

            let demonChoice = parseInt(prompt())

            if(demonChoice == aliveActs.length) {
                return true
            }

            active.battleStats.mp -= skill.cost

            // If the attacker is inflicted with mirage, could attack a different demon
            if (active.boosts[0][0] == 6 && (Math.random() * 100) < SIDE_EFFECT_RATE) {
                aliveActs.splice(aliveActs[demonChoice], 1)
                demonChoice = aliveActs[Math.floor(Math.random() * aliveActs.length)]
                console.log(`${active.name} ATTACKED ${demonChoice.name} INSTEAD!`)
            }
            else {
                demonChoice = Object.assign(aliveActs[demonChoice])
            }

            let type = 0
            if (skill.skillID < DAMAGE_SKILL_INDEX) {
                type = this.calculateDamage(skill, active, demonChoice)
                this.breakProtection(skill, demonChoice)
            }
            else if (skill.skillID < AILMENT_SKILL_INDEX) {
                type = this.calculateAilment(skill, active, demonChoice)
            }
            else {
                this.calculateSupport(skill, active, demonChoice)
                type = 0
            }

            this.progressBattle(active, type)
        }
        else {
            console.log(`0: ALL ENEMIES\n1: BACK`)

            if (parseInt(prompt()) == 0) {
                let results = 0
                active.battleStats.mp -= skill.cost
                if (skill.targets == 1) {
                    if (skill.skillID < DAMAGE_SKILL_INDEX) {
                        for (let i = 0; i < aliveActs.length; ++i) {
                            let resType = this.calculateDamage(skill, active, aliveActs[i])
                            this.breakProtection(skill, aliveActs[i])
                            if (resType > results) {
                                results = resType
                            }
                        }
                    }
                    else {
                        for (let i = 0; i < aliveActs.length; ++i) {
                            let resType = this.calculateAilment(skill, active, aliveActs[i])
                            if (resType > results) {
                                results = resType
                            }
                        }
                    }
                }
                else {
                    let attacked = []
                    for (let i = 0; i < Math.floor(Math.random() * (skill.hits[1] - skill.hits[0] + 1) + skill.hits[0]); ++i) {
                        let targetDemon = aliveActs[Math.floor(Math.random() * aliveActs.length)]

                        if (!attacked.includes(targetDemon)) {
                            attacked.push(targetDemon)
                        }

                        let resType = this.calculateDamage(skill, active, targetDemon)
                        if (resType > results) {
                            results = resType
                        }
                    }

                    for (let i = 0; i < attacked.length; ++i) {
                        this.breakProtection(skill, attacked[i])
                    }
                }
                this.progressBattle(active, results)
            }
            else {
                return true
            }
        }

        this.checkIfActive(this.defense)
        return false
    }

    selectAlly (skill, active) {
        let acts = []
        let overheal = 0

        // Checks if demon has Bowl of Hygieia active
        if (active.boosts[1] == 5) {
            overheal = 1
        }

        if (skill.targets == 0) {
            if(skill.revive)
            {
                for (let i = 0; i < this.attack.actors.length; ++i)
                {
                    if (this.attack.actors[i].battleStats.hp == 0) {
                        console.log(`${acts.length}: ${this.attack.actors[i].name} - ${this.attack.actors[i].battleStats.hp}HP`)
                        acts.push(this.attack.actors[i])
                    }
                }
            }
            else if (skill.skillID < RECOVERY_SKILL_INDEX) {
                acts = this.calculateHealableDems(overheal, true)
            }
            else {
                if (skill.selfOnly) {
                    acts.push(active)
                    console.log(`0: ${active.name}`)
                }
                else {
                    console.log(`0: ${this.attack.player.name}`)
                    acts.push(this.attack.player)
                    for (let i = 0; i < this.maxActive(this.attack); ++i) {
                        acts.push(this.attack.active[i])
                        console.log(`${i + 1}: ${this.attack.active[i].name}`)
                    } 
                }
            }

            console.log(`${acts.length}: BACK`)
            let demonChoice = parseInt(prompt())

            if (demonChoice == acts.length) {
                return true
            }

            active.battleStats.mp -= skill.cost

            if (skill.skillID < RECOVERY_SKILL_INDEX) {
                this.calculateHeal(skill, active, acts[demonChoice])
            }
            else {
                this.calculateSupport(skill, active, acts[demonChoice])
            }
        }
        else {
            if (skill.skillID < RECOVERY_SKILL_INDEX) {
                acts = this.calculateHealableDems(overheal, false)
                console.log(`ACTS: ${acts}`)

                // Cannot heal any demons, return to previous menu
                if (acts.length == 0) {
                    console.log(`0: BACK`)

                    if (parseInt(prompt()) == 0) {
                        return true
                    }
                }

                // Can heal demons, give the option to
                else {
                    console.log(`0: ALL ALLIES\n1: BACK`)
                    if (parseInt(prompt()) == 0) {
                        active.battleStats.mp -= skill.cost
                        for (let i = 0; i < acts.length; ++i) {
                            this.calculateHeal(skill, active, acts[i])
                        }
                    }
                    else if (parseInt(prompt()) == 1) {
                        return true
                    }
                }
            }
            else {
                console.log(`0: ALL ALLIES\n1: BACK`)
                let demonChoice = parseInt(prompt())
                if (demonChoice == 1) {
                    return true
                }

                active.battleStats.mp -= skill.cost
                acts.push(this.attack.player)
                for (let i = 0; i < this.maxActive(this.attack); ++i) {
                    if (this.attack.active[i].battleStats) {
                        acts.push(this.attack.active[i])
                    }
                }

                for (let i = 0; i < acts.length; ++i) {
                    this.calculateSupport(skill, active, acts[i])
                }
            }
            
        }
        this.loseTurn(1)
        return false
    }

    calculateHealableDems (overheal, display) {
        let acts = []
        if (this.attack.player.battleStats.hp != this.attack.player.baseStats.hp * this.overhealExpr(overheal)) {
            acts.push(this.attack.player)
        }
        for (let i = 0; i < this.maxActive(this.attack); ++i) {
            if (this.attack.active[i].battleStats.hp != 0 && this.attack.active[i].battleStats.hp < this.attack.active[i].baseStats.hp * this.overhealExpr(overheal)) {
                if (display) {
                    console.log(`${acts.length}: ${this.attack.active[i].name} - ${this.attack.active[i].battleStats.hp}HP`)
                }
                acts.push(this.attack.active[i])
            }
        }
        return acts
    }

    item(active) {
        let back = true
        while(back) {
            let itemList = []
            for (let i = 0; i < this.attack.items.length; ++i) {
                if (this.attack.items[i].amount > 0) {
                    console.log(`${itemList.length}: ${this.attack.items[i].name} - ${this.attack.items[i].amount}`)
                    itemList.push(this.attack.items[i].name)
                }
            }
            console.log(`${itemList.length}: BACK`)

            let itemChoice = parseInt(prompt())
            if (itemChoice == itemList.length) {
                return true
            }

            back = this.selectOpponent(this.attack.items[itemChoice].skill, active)

            if (!back) {
                --this.attack.items[itemChoice].amount
            }
        }
    }

    guard(active) {
        console.log(`${active.name} IS GUARDING...`)
        active.guard = 1
        this.loseTurn(1)
    }

    talk(active) {
        console.log("NOT IMPLEMENTED ATM")
        this.loseTurn(1)
    }

    change(active) {
        let switcheableDemons = []
        let activeID = 0

        for (let i = 0; i < this.attack.active.length; ++i)
        {
            if (this.attack.active[i].name == active.name) {
                activeID = i
                break
            }
        }

        // Returns list of demons to the user
        for (let i = 0; i < this.attack.actors.length; ++i)
        {
            if (this.attack.actors[i] == active) {
                continue
            }

            if (this.attack.actors[i].battleStats.hp > 0) {
                console.log(`${switcheableDemons.length}: ${this.attack.actors[i].name} - ${this.attack.actors[i].battleStats.hp}HP`)
                switcheableDemons.push(this.attack.actors[i])
            }
        }
        console.log(`${switcheableDemons.length}: BACK`)
        const changeChoice = parseInt(prompt())

        // If BACK is selected, return to previous menu
        if (changeChoice == switcheableDemons.length) {
            return true
        }

        // Else, swap the demons
        else if (changeChoice < switcheableDemons.length) {
            for (let i = 0; i < this.attack.active.length; ++i) {
                if (this.attack.active[i] == switcheableDemons[changeChoice]) {
                    this.attack.active[i] = this.attack.active[activeID]
                    break
                }
            }
            this.attack.active[activeID] = switcheableDemons[changeChoice]
        }

        this.extraTurn()
    }

    changePlayer() {
        let back = true
        while (back) {
            for (let i = 0; i < this.attack.active.length; ++i) {
                if (!this.attack.active[i].name) {
                    console.log(`${i}: EMPTY`)
                    continue
                }
                console.log(`${i}: ${this.attack.active[i].name} - ${this.attack.active[i].battleStats.hp}HP`)
            }
            console.log(`3: BACK`)

            let demonChoice = parseInt(prompt())

            if (demonChoice == 3) {
                return true
            }

            back = this.change(this.attack.active[demonChoice])
        }
    }

    escape(player) {
        console.log(`${player.name} FORFEITS!`)
    }

    calculateDamage(skill, attacker, defender) {
        let damage = 0

        // Check if other enemies are drawing attention to themselves
        for (let i = 0; i < this.defense.active.length; ++i) {
            if (this.defense.active[i].boosts && this.defense.active[i].boosts[4][0] != 0) {
                let tauntChance = 30 * (((this.defense.active[i].baseStats.luck / 2.0) + (this.defense.active[i].baseStats.agility / 4.0))
                    / ((attacker.baseStats.luck / 2.0) + (attacker.baseStats.agility / 4.0)))

                if ((Math.random() * 100) < tauntChance) {
                    console.log(`${this.defense.active[i].name} TOOK THE ATTACK MEANT FOR ${defender.name}`)
                    defender = this.defense.active[i]
                    break
                }
            }
        }

        let resMod = this.calculateResistance(skill, attacker, defender)
        let affected = defender
        let chargeVal = 1

        // PROTECTIVE BARRIER
        if (defender.boosts[2] == skill.type + 3) {
            resMod[0] = 0
            resMod[1] = " NULL!"
        }

        // REFLECTIVE BARRIER
        if ((defender.boosts[2] == 1 && skill.type == 0) || (defender.boosts[2] == 2 && skill.type > 1 && skill.type < 7)) {
            resMod[0] = 
            resMod[1] = " REFLECT!"
        }

        if (resMod[0] > 0) {
            // GUARDING
            if (defender.guard == 1 && resMod[0] > 0.5) {
                resMod[1] = " GUARD!"
                resMod[0] = 0.8
            }

            // MISS
            if (!this.calculateHit(skill, attacker, defender, 0)) {
                resMod[1] = " MISS!"
                resMod[0] = 0
            }

            // CRITICAL
            else {
                if (this.calculateCritRate(skill, attacker, defender)) {
                    resMod[1] += " CRITICAL!"
                    resMod[0] *= (1.5 * (1 + (attacker.passives[21] * 0.3)))
                } else {
                    resMod[0] *= (1 - (attacker.passives[21] * 0.1))
                }
            }
        }

        // REFLECT
        if (resMod[0] == -0.5 || (defender.boosts[2] == 1 && skill.type == 0) || (defender.boosts[2] == 2 && skill.type > 1 && skill.type < 7)) {
            resMod[1] = " REPEL!"
            resMod[0] = 1
            affected = attacker
        }

        // CALCULATES OHKO IF WEAK
        if (resMod[0] == 2 && skill.ohko && this.calculateInstaKill(skill, attacker, defender)) {
            resMod[1] += " OHKO!"
            affected.battleStats.hp = 0
        }

        // CALCULATES CHARGE
        if (attacker.boosts[1] == 3 || (skill.physical && attacker.boosts[1] == 1) || (!skill.physical && attacker.boosts[1] == 2)) {
            chargeVal = 1.8
        }

        // Determine whether the attack relies on strength or magic
        let demPower = 0
        if (skill.physical) {
            demPower = attacker.baseStats.strength
        }
        else {
            demPower = attacker.baseStats.magic
        }

        damage = Math.floor(Math.pow(demPower, 2) / (defender.baseStats.vitality * 1.5)
            * (1 + (skill.power / 100)) * resMod[0] * ((Math.random() / 3) + 1)
            * this.calculateBattleBuff(attacker, 0) * (2 - this.calculateBattleBuff(defender, 1))
            * (1 - (defender.boosts[3] * 0.3)) * chargeVal
            * this.passiveIncreaser(attacker.passives[skill.type + 2]) + (1 * resMod[0]))

        affected.battleStats.hp -= damage

        // IN CASE DRAIN OVERHEALS
        if (affected.battleStats.hp > affected.baseStats.hp) {
            affected.battleStats.hp = affected.baseStats.hp
        }

        if (skill.name == "Attack") {
            console.log(`${attacker.name} ATTACKS ${affected.name} FOR ${damage}HP!${resMod[1]}`)
        }
        else {
            console.log(`${attacker.name} USES ${skill.name} ON ${affected.name} FOR ${damage}HP!${resMod[1]}`)
        }

        let result = 0
        switch (resMod[1].substring(1, 6)) {
            case "WEAK!":
            case "CRITI":
                if (attacker.passives[12] != 0) {
                    let healAmnt = this.restoreAmnt(attacker.passives[12]) > attacker.baseStats.hp - attacker.battleStats.hp
                        ? attacker.baseStats.hp - attacker.battleStats.hp : this.restoreAmnt(attacker.passives[12])
                    attacker.battleStats.hp += healAmnt
                    console.log(`${attacker.name} WAS HEALED FOR ${healAmnt}HP!`)
                }
                result = 1
                break
            case "NULL!":
            case "MISS!":
                result = 2
                break
            case "REPEL":
            case "DRAIN":
                result = 3
                break
        }

        // Apply additional support effects if move wasn't blocked/evaded/reflected/absorbed
        if (result < 2 && skill.support) {
            this.calculateSupport(skill, affected, affected)
        }

        return result
    }

    calculateAilment(skill, active, defender) {
        for (let i = 0; i < skill.ailments.length; ++i) {
            let ailType = this.getAilmentType(skill.ailments[i])
            if (defender.ailmentResistances[skill.ailments[i]] == 3) {
                console.log(`${defender.name} AVOIDED BEING INFLICTED WITH ${ailType.toUpperCase()}! NULL!`)
                return 2
            }
            else if (defender.ailmentResistances[skill.ailments[i]] == 0) {
                console.log(`${defender.name} BECAME INFLICTED WITH ${ailType.toUpperCase()}! WEAK!`)
                this.calculateSupport(skill, active, defender)
                this.curseSiphon(active)
                defender.boosts[0] = [skill.ailments[i] + 1, 0, active.passives[17]]
                return 1
            }
            else {
                if (this.calculateHit(skill, active, defender, ailType)) {
                    console.log(`${defender.name} BECAME INFLICTED WITH ${ailType.toUpperCase()}!`)
                    this.calculateSupport(skill, active, defender)
                    this.curseSiphon(active)
                    defender.boosts[0] = [skill.ailments[i] + 1, 0, active.passives[17]]
                    return 0
                }
                else {
                    console.log(`${defender.name} AVOIDED BEING INFLICTED WITH ${ailType.toUpperCase()}!`)
                }
            }
        }
        return 0
    }

    getAilmentType(num) {
        switch(num) {
            case 0:
                return "poison"
            case 1:
                return "confusion"
            case 2:
                return "charm"
            case 3:
                return "seal"
            case 4:
                return "sleep"
            case 5:
                return "mirage"
        }
    }

    calculateHeal(skill, active, recipient) {
        let recovery = 1
        let overheal = 0

        // BOWL OF HYGIEIA
        if (active.boosts[1] == 5) {
            recovery = 1.5
            overheal = 1
        }

        if (skill.recoverAmnt) {
            recovery *= Math.floor(skill.recoverAmnt * ((Math.pow(active.baseStats.magic, 2) / 225) + 1))
        }
        else {
            recovery *= Math.floor((skill.recoverPrct / 100) * recipient.baseStats.hp)
        }

        console.log(`RECOVERY: ${recovery}`)

        // If recovery is greater than the allowed limit, reduce it
        // Overheal formula
        if (recipient.battleStats.hp + recovery > recipient.baseStats.hp * this.overhealExpr(overheal)) {
            recovery = Math.floor((recipient.baseStats.hp * this.overhealExpr(overheal)) - recipient.battleStats.hp)
        }

        recipient.battleStats.hp += recovery
        console.log(`${active.name} USES ${skill.name} ON ${recipient.name} FOR ${recovery}HP!`)
    }

    // Returns 1 if false, 1.3 if true
    overhealExpr (overheal) {
        return (1 + (overheal * 0.3))
    }

    calculateSupport(skill, caster, recipient) {
        if (skill.support) {
            this.calculateBuff(skill, caster, recipient)
        }
        if (skill.chargeID) {
            this.calculateCharge(skill, recipient)
        }
        if (skill.blockID) {
            this.calculateProtection(skill, recipient)
        }
        if (skill.veil) {
            this.calculateDamageDown(recipient)
        }
        if (skill.taunt) {
            this.calculateTaunt(recipient)
        }
    }

    calculateBuff(skill, caster, recipient) {
        let quote = ``
        for (let i = 0; i < skill.support.length; ++i) {
            switch (skill.support[i]) {
                case 0:
                    quote = this.buffHelper(skill.support[i], skill, caster, recipient)
                    console.log(`${recipient.name}'s ATTACK WAS ${quote}`)
                    break
                case 1:
                    quote = this.buffHelper(skill.support[i], skill, caster, recipient)
                    console.log(`${recipient.name}'s DEFENSE WAS ${quote}`)
                    break
                case 2:
                    quote = this.buffHelper(skill.support[i], skill, caster, recipient)
                    console.log(`${recipient.name}'s ACCURACY/EVASION WAS ${quote}`)
                    break
                case 3:
                    for (let i = 5; i < 8; ++i) {
                        recipient.boosts[i] = [0, 0]
                    }
                    console.log(`${recipient.name}'s STATS RETURNED TO NORMAL`)
            }
        }
    }

    buffHelper(effect, skill, caster, recipient) {
        let quote = ""

        // If buff...
        if (skill.buff && recipient.boosts[effect + 5][0] < 2) {
            recipient.boosts[effect + 5][0] += skill.suppAmnt
            quote = `RAISED BY ${skill.suppAmnt} STAGE(S)!`

            if (recipient.boosts[effect + 5][0] >= 2) {
                quote = `RAISED BY ${skill.suppAmnt} STAGE(S)! REACHED MAXIMUM AMOUNT!`
            }
        }

        // If debuff...
        else if (!skill.buff && recipient.boosts[effect + 5][0] > -2) {

            recipient.boosts[effect + 5][0] -= skill.suppAmnt
            quote = `LOWERED BY ${skill.suppAmnt} STAGE(S)!`

            if (recipient.boosts[effect + 5][0] <= -2) {
                quote += ` REACHED MINIMUM AMOUNT!`
            }
        }

        // If nothing happens...
        else {
            quote = `UNCHANGED!`
        }

        // Boost timer resets everytime a new modifier is added
        recipient.boosts[effect + 5][1] = 3 + caster.passives[18]

        return quote
    }

    calculateCharge(skill, recipient) {
        recipient.boosts[1] = +skill.chargeID + 1
        console.log(`${recipient.name} IS CHARGING ITS POWER...`)
    }

    removeCharge(skill, recipient) {
        if (skill.skillID < DAMAGE_SKILL_INDEX && (skill.physical && (recipient.boosts[1] == 1
                || recipient.boosts[1] == 4) || !skill.physical && recipient.boosts[1] == 2
                || recipient.boosts[1] == 3) || (skill.skillId < RECOVERY_SKILL_INDEX && recipient.boosts[1] == 5)) {
            recipient.boosts[1] = 0
        }
    }

    calculateProtection(skill, recipient) {
        recipient.boosts[2] = skill.blockID + 1
        console.log(`${recipient.name} IS BEING PROTECTED BY A MAGICAL BARRIER...`)
    }

    expireProtection(party) {
        if ((party.player.boosts && party.player.boosts[2] != 0) || party.player.boosts[3] != 0) {
            console.log(`${party.player.name}'S PROTECTIVE BARRIER DISSIPATED...`)
            party.player.boosts[2] = 0
            party.player.boosts[3] = 0
        }
        for (let i = 0; i < party.active.length; ++i) {
            if ((party.active[i].boosts && party.active[i].boosts[2] != 0)
                    || (party.active[i].boosts && party.active[i].boosts[3] != 0)) {
                console.log(`${party.active[i].name}'S PROTECTIVE BARRIER DISSIPATED...`)
                party.active[i].boosts[2] = 0
                party.player.boosts[3] = 0
            }
        }
    }

    breakProtection(skill, demon) {
        // Resets protective status is used
        if ((demon.boosts[2] == skill.type + 3) || (demon.boosts[2] == 1 && skill.type == 0)
                || (demon.boosts[2] == 2 && skill.type > 0 && skill.type < 7)) {
            console.log(`${demon.name}'S PROTECTIVE BARRIER SHATTERED!`)
            demon.boosts[2] = 0
        }

        // Awakens sleeping demons if hit
        else if (demon.boosts[0][0] == 5) {
            console.log(`${demon.name} WAS ABRUPTLY AWAKENED!`)
            demon.boosts[0] = [0, 0, 0]
        }
    }

    calculateDamageDown(recipient) {
        recipient.boosts[3] = 1
        console.log(`${recipient.name} IS BEING PROTECTED BY A MAGICAL BARRIER...`)
    }

    calculateTaunt(recipient) {
        recipient.boosts[4] = [1, 3]
        console.log(`${recipient.name} IS DRAWING IN THE ENEMY'S FOCUS`)
    }

    calculateHit(skill, attacker, defender, ailType) {
        // Guaranteed hit if the opponent is asleep or the user is boosted by Critical Aura
        if (defender.boosts[0][0] == 5 || defender.boosts[1] == 4) {
            return true
        }

        let accuracy = skill.accuracy * (this.calculateDemonHitAvoid(attacker) / this.calculateDemonHitAvoid(defender)) * this.passiveIncreaser(defender.passives[15])

        // If ailment skill, take the defender's resistances to determine accuracy
        if (skill.skillID >= DAMAGE_SKILL_INDEX) {
            accuracy *= (1 / defender.ailmentResistances[ailType]) * this.passiveDecreaser(defender.passives[11])
        }

        // If attacker is inflicted with mirage, lower accuracy
        if (attacker.boosts[0][0] == 6) {
            accuracy /= 2
        }

        if ((Math.random() * 100) < accuracy) {
            return true
        }
        else {
            return false
        }
    }

    calculateDemonHitAvoid(demon) {
        return (demon.baseStats.agility / 2.0) + (demon.baseStats.luck / 4.0)  * this.calculateBattleBuff(demon, 2)
    }

    calculateInstaKill(skill, attacker, defender) {
        let killChance = skill.ohko * (this.calculateDemonInstaKillChance(attacker) / this.calculateDemonInstaKillChance(defender)) * this.passiveDecreaser(defender.passives[11])

        if ((Math.random() * 100) < killChance) {
            return true
        }
        else {
            return false
        }
    }

    calculateDemonInstaKillChance(demon) {
        return (demon.baseStats.luck / 2.0) + (demon.baseStats.agility / 4.0)
    }

    calculateCritRate(skill, attacker, defender) {
        let critBonus = skill.critBonus
        if (isNaN(critBonus)) {
            critBonus = 0
        }

        // Guaranteed crit moves
        if (critBonus == 200) {
            return true
        }

        // CRITICAL AURA
        if (skill.physical && attacker.boosts[1] == 4) {
            return true
        }

        let crit = critBonus + (this.calculateDemonCritAvoid(attacker) / this.calculateDemonCritAvoid(defender)) * this.passiveIncreaser(attacker.passives[16]) + 6.25

        if ((Math.random() * 100) < crit) {
            return true
        }
        else {
            return false
        }
    }

    calculateDemonCritAvoid(demon) {
        return (demon.baseStats.luck / 2.0) + (demon.baseStats.agility / 4.0)
    }

    calculateBattleBuff(demon, type) {
        return (1 + demon.boosts[type + 5][0] * 0.2)
    }

    calculateResistance(skill, attacker, defender) {
        let skillType = ""

        switch(skill.type) {
            case 0:
                skillType = "physical"
                break
            case 1:
                skillType = "fire"
                break
            case 2:
                skillType = "ice"
                break
            case 3:
                skillType = "electric"
                break
            case 4:
                skillType = "force"
                break
            case 5:
                skillType = "light"
                break
            case 6:
                skillType = "dark"
                break
            case 7:
                return [1, ""]
        }

        // CHECKS IF SKILL PIERCES RESISTANCES
        if (skill.pierce || attacker.boosts[1] == 3)
        {
            switch(defender.resistances[skillType]) {
                case 0:
                    return [2, "WEAK!"]
                default:
                    return [1, ""]
            }
        }

        switch(defender.resistances[skillType]) {
            case 0:
                return [2, " WEAK!"]
            case 1:
                return [1, ""]
            case 2:
                return [0.5, " RESIST!"]
            case 3:
                return [0, " NULL!"]
            case 4:
                return [-0.5, " REPEL!"]
            case 5:
                return [-1, " DRAIN!"]
        }
    }

    checkIfActive(party) {
        for (let i = 0; i < party.active.length; ++i) {
            if (!party.active[i].battleStats) {
                continue
            }
            else if (party.active[i].battleStats.hp <= 0) {
                if (party.active[i].passives[19] == 0) {
                    party.active[i].battleStats.hp = 0
                    console.log(`${party.active[i].name} DEFEATED!`)
                    party.active[i] = {}
                }

                // If demon has an enduring skill, activate instead of killing the demon
                else {
                    if (party.active[i].passives[19] == 1) {
                        party.active[i].battleStats.hp = 1
                        console.log(`${party.active[i].name} WITHSTOOD THE ATTACK!`)
                    }
                    else {
                        party.active[i].battleStats.hp = party.active[i].baseStats.hp
                        console.log(`${party.active[i].name} WITHSTOOD THE ATTACK AND HEALED BACK TO FULL!`)
                    }
                    party.active[i].passives[19] = 0
                }
            }
        }
    }

    progressBattle(active, results) {
        if (active.passives[20] == 1 && results >= 2) {
            console.log(`${active.name}'S SAFEGUARD PREVENTED PRESS TURNS FROM BEING CONSUMED!`)
            results = 0
        }

        switch(results) {
            case 0:
                this.loseTurn(1)
                break
            case 1:
                this.extraTurn()
                break
            case 2:
                this.loseTurn(2)
                break
            case 3:
                this.loseTurn(4)
        }
    }

    reduceBuffDurations(party) {
        this.reduceBuffHelper(party.player)
        for (let i = 0; i < party.active.length; ++i) {
            this.reduceBuffHelper(party.active[i])
        }
    }

    reduceBuffHelper(demon) {
        // Makes sure a demon exists at the position
        if (Object.keys(demon).length == 0) {
            return
        }

        // Reduces the boost of the current active member
        for (let i = 0; i < 3; ++i) {
            // Handle cases where boost runs out completely
            if (demon.boosts[i + 5][1] == 1) {
                demon.boosts[i + 5] = [0, 0]

                let quote = ``
                switch (i) {
                    case 0:
                        quote = `ATTACK`
                        break
                    case 1:
                        quote = `DEFENSE`
                        break
                    case 2:
                        quote = `ACCURACY/EVASION`
                        break
                }
                console.log(`${demon.name}'S ${quote} RETURNED TO NORMAL...`)
            }
            else if (demon.boosts[i + 5][1] > 0) {
                --demon.boosts[i + 5][1]
            }
        }

        // Taunt reductions
        if (demon.boosts[4][1] == 1) {
            demon.boosts[4] = [0, 0]
            console.log(`${demon.name} IS NO LONGER DRAWING THE ENEMY'S ATTENTION`)
        } else if (demon.boosts[4][1] > 1) {
            --demon.boosts[4][1]
        }
    }

    passiveIncreaser(amount) {
        switch (amount) {
            case 1:
                return 1.2
            case 2:
                return 1.35
            case 3:
                return 1.55
            default:
                return 1
        }
    }

    passiveDecreaser(amount) {
        switch (amount) {
            case 1:
                return 0.8
            case 2:
                return 0.65
            case 3:
                return 0.45
            default:
                return 1
        }
    }

    restoreAmnt(amount) {
        switch (amount) {
            case 1:
                return 10
            case 2:
                return 20
            case 3:
                return 30
        }
    }

    curseSiphon(demon) {
        if (demon.passives[13] != 0) {
            let healAmnt = this.restoreAmnt(demon.passives[13]) > demon.baseStats.mp - demon.battleStats.mp
                ? demon.baseStats.mp - demon.battleStats.mp : this.restoreAmnt(demon.passives[13])
            demon.battleStats.mp += healAmnt
            console.log(`${demon.name} REGAINED ${healAmnt}MP!`)
        }
    }

    poisonRate(amnt) {
        switch(amnt) {
            case 0:
                return 16
            case 1:
                return 12
            case 2:
                return 8
            case 3:
                return 4
        }
    }

    loseTurn(amnt) {
        for(let i = 0; i < amnt; ++i) {
            if (this.attack.pressTurns[1] != 0) {
                --this.attack.pressTurns[1]
            }
            else {
                --this.attack.pressTurns[0]
            }
        }
    }

    extraTurn() {
        if (this.attack.pressTurns[0] != 0) {
            ++this.attack.pressTurns[1]
            --this.attack.pressTurns[0]
        }
        else {
            --this.attack.pressTurns[1]
        }
    }

    amountOfActiveDemons(party) {
        let activeDemons = 0
        for (let i = 0; i < this.maxActive(party); ++i) {
            if (party.active[i].name) {
                ++activeDemons
            }
        }
        return activeDemons
    }

    maxActive(party) {
        return (party.active.length > 3 ? 3 : party.active.length)
    }

    checkIfPlayersAlive() {
        if (this.attack.player.battleStats.hp <= 0) {
            console.log(`${this.attack.player.name} HAS BEEN DEFEATED! ${this.defense.player.name} HAS WON THE BATTLE`)
            return false
        }
        else if (this.defense.player.battleStats.hp <= 0) {
            console.log(`${this.defense.player.name} HAS BEEN DEFEATED! ${this.attack.player.name} HAS WON THE BATTLE`)
            return false
        }
        return true
    }
 }
