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
            while (!(this.attack.pressTurns[0] == 0 && this.attack.pressTurns[1] == 0)) {
                console.log(`TURNS LEFT: ${this.attack.pressTurns}`)
                let active = {}
                while(Object.keys(active).length == 0) {
                    if ((counter % (activeDemons + 1)) == 0) {
                        active = this.attack.player
                    }
                    else {
                        if (this.attack.actors[(counter % (activeDemons + 1)) - 1].battleStats.hp > 0) {
                            active = this.attack.actors[(counter % (activeDemons + 1)) - 1]
                        }
                    }
                    ++counter
                }

                active.guard = 0;

                console.log(`WHAT WILL ${active.name} DO?`)
                console.log(`0: SKILLS\n1: ITEMS\n2: GUARD\n3: TALK\n4: CHANGE\n5: ESCAPE\n6: PASS\n`)
                const choice = prompt()
                switch (choice) {
                    case '0':
                        this.skill(active)
                        break
                    case '2':
                        this.guard(active)
                        break
                    case '4':
                        this.change(active)
                        break
                    case '6':
                        this.extraTurn()
                        break
                    default:
                        console.log('NOT VALID ATM')
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
        for (let i = 0; i < active.skills.length; ++i)
        {
            console.log(`${i}: ${active.skills[i].name} - ${active.skills[i].cost}MP`)
        }
        let skillChoice = active.skills[parseInt(prompt())]
        let counter = 0

        // All damage/ailment skills affect opponents
        if (skillChoice.skillID < 196) {
            if (skillChoice.targets == 0) {
                if (this.maxActive(this.defense) < 3) {
                    console.log(`0: ${this.defense.player.name} - ${this.defense.player.battleStats.hp}HP`)
                    ++counter
                }

                for (let i = 0; i < this.maxActive(this.defense); ++i)
                {
                    if (this.defense.actors[i].battleStats.hp > 0) {
                        console.log(`${counter}: ${this.defense.actors[i].name} - ${this.defense.actors[i].battleStats.hp}HP`)
                        ++counter
                    }
                }

                let demonChoice = this.defense.actors[parseInt(prompt())]

                console.log('SKILL:')
                console.log(skillChoice)
                console.log('ATTACKER:')
                console.log(active)
                console.log('DEFENDER:')
                console.log(demonChoice)

                let damage = this.calculateDamage(skillChoice, active, demonChoice)

                console.log(`${active.name} USES ${skillChoice.name} ON ${demonChoice.name} FOR ${damage}HP!`)
            }
            else {
                console.log(`0: ALL ENEMIES`)
            }
        }

        // All recovery skills affect allies
        else if (skillChoice.skillID < 216) {
            if (skillChoice.targets == 0) {
                if(skillChoice.revive)
                {
                    for (let i = 0; i < this.attack.actors.length; ++i)
                    {
                        if (this.attack.actors[i].battleStats.hp <= 0) {
                            console.log(`${counter}: ${this.attack.actors[i].name}`)
                            ++counter
                        }
                    }
                }
                else {
                    if (this.attack.player.battleStats.hp != this.attack.player.baseStats.hp) {
                        console.log(`0: ${this.attack.player.name} - ${this.attack.player.battleStats.hp}HP`)
                        ++counter
                    }
                    for (let i = 0; i < this.maxActive(this.attack); ++i)
                    {
                        if (this.attack.actors[i].battleStats.hp > 0 && this.attack.actors[i].battleStats.hp != this.attack.actors[i].baseStats.hp) {
                            console.log(`${counter}: ${this.attack.actors[i].name} - ${this.attack.actors[i].battleStats.hp}HP`)
                            ++counter
                        }
                    }
                }
            }
            else {
                console.log(`0: ALL ALLIES`)
            }
        }
    }

    guard(active) {
        active.guard = 1;
        this.loseTurn()
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
        console.log(`ACTIVE ID: ${activeListID}`)
        console.log(`INDICIES: ${switcheableDemons}`)
        const changeChoice = parseInt(prompt())
        if (changeChoice <= switcheableDemons.length) {
            let temp = this.attack.actors[activeID]
            this.attack.actors[activeID] = this.attack.actors[switcheableDemons[changeChoice]]
            this.attack.actors[switcheableDemons[changeChoice]] = temp
        }

        this.extraTurn()
    }

    calculateDamage(skill, attacker, defender) {
        if (skill.physical) {
            return Math.floor(Math.pow(attacker.baseStats.strength, 2) / (defender.baseStats.vitality * 1.5) * (1 + (skill.power / 100)) + (Math.random() * 10))
        }
        else {
            return Math.floor(Math.pow(attacker.baseStats.magic, 2) / (defender.baseStats.vitality * 1.5) * (1 + (skill.power / 100)) + (Math.random() * 10))
        }
    }

    loseTurn() {
        if (this.attack.pressTurns[1] != 0) {
            --this.attack.pressTurns[1]
        }
        else {
            --this.attack.pressTurns[0]
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