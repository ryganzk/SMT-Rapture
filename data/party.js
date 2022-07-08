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
      * @param {Ruleset} rules
      *****************************************************************************/
    constructor(player, rules) {
        this.player = Object.values(player)[0]
        this.rules = rules
        this.actors = []
        this.active = []
        this.items = []

        // Makes sure the player's level isn't above the ruleset's limit
        if (this.player.level > this.rules.levelLimit) {
            throw `${this.player.name}'s level is higher than the limit of ${this.rules.levelLimit}`
        }

        if(this.player.skills.length > rules.moveLimit) {
            throw `${this.player.name} cannot know more than ${rules.moveLimit} moves`
        }

        for (let i = 0; i < this.player.skills.length; ++i) {
            if (this.player.skills[i].unique && this.player.skills[i].unique != 999 && this.rules.standardRules) {
                throw `${this.player.name} cannot use the unique move ${this.player.skills[i].name}`
            }

            if (this.player.skills[i].level > this.rules.highestRank) {
                throw `${this.player.name} cannot learn ${this.player.skills[i].name}, as it's too powerful for the current ruleset`
            }
        }
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
        // Makes sure the demon's level isn't lower than the minimum possible level for that demon
        if (statMods[0] < demon.level) {
            throw `${demon.name}'s level is too low! Must be ${demon.level} or higher!`
        }

        // Makes sure the player's level isn't above the ruleset's limit
        if (statMods[0] > this.rules.levelLimit) {
            throw `${demon.name}'s level is higher than the limit of ${this.rules.levelLimit}!`
        }

        if(skills.length > this.rules.moveLimit) {
            throw `${demon.name} cannot know more than ${this.rules.moveLimit} moves`
        }

        if (statMods[1] + statMods[2] > (statMods[0] - demon.level) * 5
        || statMods[3] + statMods[4] + statMods[5] + statMods[6] + statMods[7]
        > (statMods[0] - demon.level) * 3) {
            throw `Too many stats have been pumped into ${demon.name}!`
        }

        demon.level = statMods[0]
        demon.baseStats.hp += statMods[1]
        demon.baseStats.mp += statMods[2]
        demon.baseStats.strength += statMods[3]
        demon.baseStats.vitality += statMods[4]
        demon.baseStats.magic += statMods[5]
        demon.baseStats.agility += statMods[6]
        demon.baseStats.luck += statMods[7]

        let base = false

        // If the demon doesn't include any moves, assume base skills
        if(skills.length == 0) {
            base = true
            skills = demon.baseSkills
            
            // Depending on the demon's level, give appropriate level-up skills
            for (let i = 0; i < demon.levelSkills.length; ++i) {
                if (statMods[0] >= demon.levelSkills[i][1]) {
                    skills.push(demon.levelSkills[i][0])
                }
                else {
                    break
                }
            }
        }
        
        for(let i = 0; i < skills.length; i++) {
            skills[i] = skillCompendium.getSkill(skills[i])

            if (skills[i].unique && skills[i].unique != demon.demID) {
                throw `${demon.name} cannot use the unique move ${skills[i].name}`
            }

            if (skills[i].level > this.rules.highestRank && !base) {
                throw `${demon.name} cannot learn ${skills[i].name}, as it's too powerful for the current ruleset`
            }

            determinePotentials(demon.potentials, skills[i])
        }

        // Makes sure the demon doesn't include duplicate skills
        let dupCheck = new Set(skills)
            if (skills.length !== dupCheck.size) {
                for (let i = 0; i < skills.length; ++i) {
                    if (!dupCheck.has(skills[i])) {
                        throw `${demon.name} knows ${skills[i].name} more than once!`
                    }
                    else {
                        dupCheck.delete(skills[i])
                    }
                }
            }

        let addendum = {
            battleStats: {
                hp: demon.baseStats.hp,
                mp: demon.baseStats.mp
            },
            skills: skills
        }

        Object.assign(demon, addendum)
        
        /*************************************************************************
          * 0: AILMENT
          * 1: CHARGE
          * 2: PROTECTIVE
          * 3: DAMAGE DOWN
          * 4: TAUNT
          * 5: ATTACK
          * 6: DEFENSE
          * 7: ACCURACY/EVASION
          * 
          * ARRAYS WITHIN THE ARRAY REPRESENTS: [BOOST, TURNS LEFT]
          *************************************************************************/

        demon.boosts = [[0, 0, 0], 0, 0, 0, [0, 0], [0, 0], [0, 0], [0, 0]]

        /*************************************************************************
          * 0: HP AID
          * 1: MP AID
          * 2: PHYS DAMAGE
          * 3: FIRE DAMAGE
          * 4: ICE DAMAGE
          * 5: ELEC DAMAGE
          * 6: FORCE DAMAGE
          * 7: LIGHT DAMAGE
          * 8: DARK DAMAGE
          * 10: ALMIGHTY DAMAGE
          * 11: MASK
          * 12: RESTORE
          * 13: CURSE SIPHON
          * 14: COUNTER
          * 15: EYE
          * 16: GLEE
          * 17: POISON
          * 18: BOON BOOST
          * 19: ENDURE
          * 20: SAFEGUARD
          * 21: CRITICAL ZEALOT
          * 22: HEAVENLY COUNTER
          *************************************************************************/

        demon.passives = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        demon.guard = 0

        this.actors.push(demon)

        if (this.active.length < 3) {
            this.active.push(demon)
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
        item.skill = skillCompendium.getSkill(item.skill)
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