/*********************************************************************************
 * battler.js
 * 
 * Controls gameplay
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

import promptSync from 'prompt-sync'

const prompt = promptSync()

export class Battler {

    constructor(attackingParty, defendingParty)
    {
        this.attack = attackingParty
        this.defense = defendingParty
    }

    begin() {
        console.log(`BATTLE HAS BEGUN BETWEEN ${this.attack.player.name} AND ${this.defense.player.name}`)
        this.turn()
    }

    turn() {
        while(true) {
            let counter = 0
            let activeDemons = this.amountOfActiveDemons(this.attack)

            this.attack.pressTurns = [activeDemons + 1, 0]
            while (!(this.attack.pressTurns[0] <= 0 && this.attack.pressTurns[1] <= 0)) {
                console.log(`TURNS LEFT: ${this.attack.pressTurns}`)
                let active = {}
                while(Object.keys(active).length == 0) {
                    if ((counter % (activeDemons + 1)) == 0) {
                        active = Object.assign(this.attack.player)
                    }
                    else {
                        if (this.attack.actors[(counter % (activeDemons + 1)) - 1].battleStats.hp > 0) {
                            active = Object.assign(this.attack.actors[(counter % (activeDemons + 1)) - 1])
                        }
                    }
                    ++counter
                }

                active.guard = 0;
                
                let back = true
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
                            back = this.change(active)
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
                }
            }
            console.log(`SWITCH!!!`)

            // Switches sides
            let temp = this.attack
            this.attack = this.defense
            this.defense = temp
        }
    }

    skill(active) {
        let back = true
        while (back) {
            // Allows actors to use a regular attack if needed
            console.log(`0: Regular Attack`)

            let i = 0
            for (i; i < active.skills.length; ++i)
            {
                console.log(`${i + 1}: ${active.skills[i].name} - ${active.skills[i].cost}MP`)
            }
            console.log(`${++i}: BACK`)

            let skillChoice = parseInt(prompt())

            if (skillChoice == i) {
                return true
            }

            if (skillChoice == 0) {
                this.selectOpponent({
                    name: "Attack",
                    cost: 5,
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
            else if (active.skills[--skillChoice].skillID < 196) {
                back = this.selectOpponent(active.skills[skillChoice], active)
            }

            // All recovery skills affect allies
            else if (active.skills[skillChoice].skillID < 216) {
                back = this.selectAlly(active.skills[skillChoice], active)
            }
        }
        return false
    }

    selectOpponent(skill, active) {
        let aliveActs = []

        if (this.maxActive(this.defense) < 3) {
            aliveActs.push(Object.assign(this.defense.player))
        }

        if (skill.targets == 0) {
            //If user can be targeted, allow it
            if (aliveActs.length == 1)
            {
                console.log(`0: ${this.defense.player.name} - ${this.defense.player.battleStats.hp}HP`)
            }

            for (let i = 0; i < this.maxActive(this.defense); ++i)
            {
                if (this.defense.actors[i].battleStats.hp > 0) {
                    console.log(`${aliveActs.length}: ${this.defense.actors[i].name} - ${this.defense.actors[i].battleStats.hp}HP`)

                    aliveActs.push(Object.assign(this.defense.actors[i]))
                }
            }

            console.log(`${aliveActs.length}: BACK`)

            let demonChoice = parseInt(prompt())

            if(demonChoice == aliveActs.length) {
                return true
            }

            demonChoice = Object.assign(this.defense.actors[demonChoice])

            let type = this.calculateDamage(skill, active, demonChoice)

            this.progressBattle(type)
        }
        else {
            console.log(`0: ALL ENEMIES\n1: BACK`)
            for (let i = 0; i < this.maxActive(this.attack); ++i) {
                if (this.defense.actors[i].battleStats.hp > 0) {
                    aliveActs.push(Object.assign(this.defense.actors[i]))
                }
            }

            if (parseInt(prompt()) == 0) {
                let results = 0
                if (skill.targets == 1) {
                    for (let i = 0; i < aliveActs.length; ++i) {
                        let resType = this.calculateDamage(skill, active, Object.assign(aliveActs[i]))
                        if (resType > results) {
                            results = resType
                        }
                    }
                }
                else {
                    for (let i = 0; i < Math.floor(Math.random() * (skill.hits[1] - skill.hits[0] + 1) + skill.hits[0]); ++i) {
                        let resType = this.calculateDamage(skill, active, Object.assign(aliveActs[Math.floor(Math.random() * aliveActs.length)]))
                        if (resType > results) {
                            results = resType
                        }
                    }
                }
                this.progressBattle(results)
            }
            else {
                return true
            }
        }
        return false
    }

    selectAlly (skill, active) {
        let acts = []

        if (skill.targets == 0) {
            if(skill.revive)
            {
                for (let i = 0; i < this.attack.actors.length; ++i)
                {
                    if (this.attack.actors[i].battleStats.hp <= 0) {
                        console.log(`${acts.length}: ${this.attack.actors[i].name} - ${this.attack.actors[i].battleStats.hp}HP`)
                        acts.push(Object.assign(this.attack.actors[i]))
                    }
                }
            }
            else {
                if (this.attack.player.battleStats.hp != this.attack.player.baseStats.hp) {
                    aliveActs.push(Object.assign(this.attack.player))
                }
                for (let i = 0; i < this.maxActive(this.attack); ++i) {
                    if (this.attack.actors[i].battleStats.hp != 0 && this.attack.actors[i].battleStats.hp < this.attack.actors[i].baseStats.hp) {
                        console.log(`${acts.length}: ${this.attack.actors[i].name} - ${this.attack.actors[i].battleStats.hp}HP`)
                        acts.push(Object.assign(this.attack.actors[i]))
                    }
                }
            }
            console.log(`${acts.length}: BACK`)

            let demonChoice = parseInt(prompt())

            if (demonChoice == acts.length) {
                return true
            }

            this.calculateHeal(skill, active, Object.assign(acts[demonChoice]))
        }
        else {
            console.log(`0: ALL ALLIES\n1: BACK`)
            if (demonChoice == 1) {
                return true
            }
        }
        return false
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
        let activeListID = 0

        for (let i = 0; i < this.attack.actors.length; ++i)
        {
            if (this.attack.actors[i].name == active.name) {
                activeListID = i
                break
            }
        }

        for (let i = 0; i < this.attack.actors.length; ++i)
        {
            if (this.attack.actors[i].name == active.name) {
                activeID = i
                continue
            }

            if (this.attack.actors[i].battleStats.hp != 0) {
                console.log(`${switcheableDemons.length}: ${this.attack.actors[i].name}`)
                switcheableDemons.push(this.attack.actors.indexOf(this.attack.actors[i]))
            }
        }
        console.log(`${switcheableDemons.length}: BACK`)
        const changeChoice = parseInt(prompt())

        if (changeChoice == switcheableDemons.length) {
            return true
        }

        if (changeChoice <= switcheableDemons.length) {
            let temp = this.attack.actors[activeID]
            this.attack.actors[activeID] = this.attack.actors[switcheableDemons[changeChoice]]
            this.attack.actors[switcheableDemons[changeChoice]] = temp
        }

        this.extraTurn()
    }

    escape(player) {
        console.log(`${player.name} FORFEITS!`)
    }

    calculateDamage(skill, attacker, defender) {
        let damage = 0
        let resMod = this.calculateResistance(skill, defender)
        let affected = defender

        // REFLECT
        if (resMod[0] == -0.5) {
            resMod = 1
            affected = attacker
        }

        // GUARDING
        if (defender.guard == 1 && resMod[0] > 0) {
            resMod[1] = " GUARD!"
            resMod[0] = 0.8
        }

        if (skill.physical) {
            damage = Math.floor(Math.pow(attacker.baseStats.strength, 2) / (defender.baseStats.vitality * 1.5) * (1 + (skill.power / 100)) * resMod[0] * ((Math.random() / 3) + 1) + (1 * resMod[0]))
        }
        else {
            damage = Math.floor(Math.pow(attacker.baseStats.magic, 2) / (defender.baseStats.vitality * 1.5) * (1 + (skill.power / 100)) * resMod[0] * ((Math.random() / 3) + 1) + (1 * resMod[0]))
        }

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

        if (affected.battleStats.hp <= 0) {
            affected.battleStats.hp = 0
            console.log(`${affected.name} DEFEATED!`)
        }

        switch (resMod[1]) {
            case " WEAK!":
                return 1
            case " NULL!":
                return 2
            case " REFLECT!":
                return 3
            case " DRAIN!":
                return 3
            default:
                return 0
        }
    }

    calculateHeal(skill, active, recipient) {
        let recovery

        if (skill.recoverAmnt) {
            recovery = skill.recoverAmnt
        }
        else {
            console.log("ok!")
            recovery = Math.floor((skill.recoverPrct / 100) * recipient.baseStats.hp)
        }

        if (recipient.battleStats.hp + recovery > recipient.baseStats.hp) {
            recovery = recipient.baseStats.hp - recipient.battleStats.hp
        }

        recipient.battleStats.hp += recovery
        console.log(`${active.name} USES ${skill.name} ON ${recipient.name} FOR ${recovery}HP!`)
    }

    calculateResistance(skill, defender) {
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
                return 1
        }

        // CHECKS IF SKILL PIERCES RESISTANCES
        if (skill.pierce == true)
        {
            switch(defender.resistances[skillType]) {
                case 0:
                    return 2
                default:
                    return 1
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
                return [-0.5, " REFLECT"]
            case 5:
                return [-1, " DRAIN"]
        }
    }

    progressBattle(results) {
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
            if (party.actors[i].battleStats.hp > 0) {
                ++activeDemons
            }
        }
        return activeDemons
    }

    maxActive(party) {
        return (party.actors.length > 3 ? 3 : party.actors.length)
    }
 }
 