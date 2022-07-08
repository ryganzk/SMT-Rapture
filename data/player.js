/*********************************************************************************
 * player.js
 * 
 * Creates an object containing information on the player
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

import {PlayerCompendium} from "./demons/playerCompendium.js"
import {SkillCompendium} from "./skills/skillCompendium.js"
import {determinePotentials} from "./skills/potentialCalculator.js"
const skillCompendium = new SkillCompendium()

let playerCompendium = new PlayerCompendium()
 
export class Player {
    constructor(name, id, statMods, skills) {
        try {
            let player = JSON.parse(JSON.stringify(playerCompendium.getPlayer(id)));

            // Makes sure the player's level isn't lower than the minimum possible level for that demon
            if (statMods[0] < player.level) {
                throw `${name}'s level is too low! Must be ${player.level} or higher!`
            }

            if (statMods[1] + statMods[2] > (statMods[0] - player.level) * 6
            || statMods[3] + statMods[4] + statMods[5] + statMods[6] + statMods[7]
            > (statMods[0] - player.level) * 4)
                throw `Too many stats have been pumped into ${name}!`

            player.level = statMods[0]
            player.baseStats.hp += statMods[1]
            player.baseStats.mp += statMods[2]
            player.baseStats.strength += statMods[3]
            player.baseStats.vitality += statMods[4]
            player.baseStats.magic += statMods[5]
            player.baseStats.agility += statMods[6]
            player.baseStats.luck += statMods[7]

            let battleStats = {
                battleStats: {
                    hp: player.baseStats.hp,
                    mp: player.baseStats.mp
                }
            }

            Object.assign(player, battleStats)

            if(skills.length != 0) 
                player.skills = skills
            
            for(let i = 0; i < player.skills.length; i++) {
                player.skills[i] = skillCompendium.getSkill(player.skills[i])
                determinePotentials(player.potentials, player.skills[i])
            }

            let dupCheck = new Set(player.skills)
            if (player.skills.length !== dupCheck.size) {
                for (let i = 0; i < player.skills.length; ++i) {
                    if (!dupCheck.has(player.skills[i])) {
                        throw `${player.name} knows ${player.skills[i].name} more than once!`
                    }
                    else {
                        dupCheck.delete(player.skills[i])
                    }
                }
            }
            
            player.boosts = [[0, 0, 0], 0, 0, 0, [0, 0], [0, 0], [0, 0], [0, 0]]
            player.passives = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
            player.guard = 0

            player.name = name
            this.player = player
        }
        catch (err) {
            console.log(err)
        }
    }
}