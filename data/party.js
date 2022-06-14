/*********************************************************************************
 * party.js
 * 
 * Creates an object containing information on useable demons and items within a
 * battle
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

import {SkillCompendium} from "./skills/skillCompendium.js"
import {determinePotentials} from "./skills/potentialCalculator.js"
const skillCompendium = new SkillCompendium()

export class Party {

    /*****************************************************************************
      * Compendium constructor
      * 
      * @method
      * @param {Player} player
      *****************************************************************************/
    constructor(player) {
        this.player = Object.values(player)[0]
        this.actors = []
        this.items = []
    }

    /*****************************************************************************
      * Takes in a demon element, modifies baseStats based on the demon's level, gives
      * the wanted skills to the demon, and adds it as an actor to the party
      * 
      * @method
      * @param {Demon} demon
      * @param {int[8]} statMods
      * @param {Skill[8]} skills
      * @throws If the player wants to add more baseStats than the demon's level allows
      *****************************************************************************/
    pushNewActor(demon, statMods, skills) {
        try {
            if (statMods[1] + statMods[2] > (statMods[0] - demon.level) * 5
            || statMods[3] + statMods[4] + statMods[5] + statMods[6] + statMods[7]
            > (statMods[0] - demon.level) * 3)
                throw `Too many stats have been pumped into ${demon.name}!`

            demon.level = statMods[0]
            demon.baseStats.hp += statMods[1]
            demon.baseStats.mp += statMods[2]
            demon.baseStats.strength += statMods[3]
            demon.baseStats.vitality += statMods[4]
            demon.baseStats.magic += statMods[5]
            demon.baseStats.agility += statMods[6]
            demon.baseStats.luck += statMods[7]

            let battleStats = {
                battleStats: {
                    hp: demon.baseStats.hp,
                    mp: demon.baseStats.mp
                }
            }

            Object.assign(demon, battleStats)

            if(skills.length != 0) 
                demon.skills = skills
            
            for(let i = 0; i < demon.skills.length; i++) {
                demon.skills[i] = skillCompendium.getSkill(demon.skills[i])
                determinePotentials(demon.potentials, demon.skills[i])
            }
            
            demon.boosts = [0, 0, 0, 0]
            demon.guard = 0

            this.actors.push(demon)
        }
        catch (err) {
            console.log(err)
        }
    }

    /*****************************************************************************
      * Returns the actor at the specified poisition
      * 
      * @method 
      * @param {int} actorNum
      * @returns {Demon}
      *****************************************************************************/
    getActor(actorNum) {
        return this.actors[actorNum]
    }

    /*****************************************************************************
      * Adds the specified item to the party, as well as the intended amount
      * 
      * @method
      * @param {Item} item
      * @param {int} amount
      *****************************************************************************/
    addItem(item, amount) {
        item.amount = amount
        this.items.push(item)
    }

    /*****************************************************************************
      * Logs the actors and items in the party to console
      * 
      * @method
      *****************************************************************************/
    displayParty() {
        console.log(`PLAYER\n`)
        console.log(this.player)
        console.log(`\nACTORS\n`)
        console.log(this.actors)
        console.log(`\nITEMS\n`)
        console.log(this.items)
    }

    displayDemon(id) {
        console.log(this.actors[id])
    }
}