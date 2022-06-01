/*********************************************************************************
 * player.js
 * 
 * Creates an object containing information on the player
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

import fsPromises from 'fs/promises'
import {SkillCompendium} from "./skills/skillCompendium.js"
const skillCompendium = new SkillCompendium()

let player = JSON.parse(await fsPromises.readFile("./data/demons/player/player.json"))
 
export class Player {
    constructor(name, statMods, skills) {
        try {
            if (statMods[1] + statMods[2] > (statMods[0] - player.level) * 6
            || statMods[3] + statMods[4] + statMods[5] + statMods[6] + statMods[7]
            > (statMods[0] - player.level) * 4)
                throw `Too many stats have been pumped into ${player.name}!`

            player.level = statMods[0]
            player.stats.hp += statMods[1]
            player.stats.mp += statMods[2]
            player.stats.strength += statMods[3]
            player.stats.vitality += statMods[4]
            player.stats.magic += statMods[5]
            player.stats.agility += statMods[6]
            player.stats.luck += statMods[7]

            if(skills.length != 0) 
                player.skills = skills
            
            for(let i = 0; i < player.skills.length; i++) {
                player.skills[i] = skillCompendium.getSkill(player.skills[i])
            }
            
            player.boosts = [0, 0, 0, 0, 0, 0, 0, 0, 0]
            player.blocks = [0, 0, 0, 0, 0, 0, 0]
            player.taunt = 0
        }
        catch (err) {
            console.log(err)
        }

        player.name = name
        this.player = player;
    }
}