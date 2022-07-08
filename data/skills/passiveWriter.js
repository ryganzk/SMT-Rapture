/*********************************************************************************
 * passiveWrite.js
 * 
 * Updates demons based on their passives
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

export class PassiveWriter {
    constructor () {

    }

    updateWithPassive(demon, skill) {
        console.log(skill)
        switch (skill.passive[0]) {
            case 0:
                demon.baseStats.hp = Math.floor(demon.baseStats.hp * (skill.strength == 1 ? 1.3 : 1.15))
                break
            case 1:
                demon.baseStats.mp = Math.floor(demon.baseStats.mp * (skill.strength == 1 ? 1.3 : 1.15))
                break
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
                if (skill.strength >= 2 && skill.strength > demon.resistances[this.findResistance(skill.passive[0] - 4)]) {
                    demon.resistances[this.findResistance(skill.passive[0] - 4)] += skill.strength
                    break
                }
            default:
                let strength = 1
                if (skill.strength) {
                    strength = skill.strength + 1
                }
                demon.passives[skill.passive[0] - 2] += strength
                break
        }
    }

    findResistance(passiveType) {
        switch(passiveType) {
            case 0:
                return "physical"
            case 1:
                return "fire"
            case 2:
                return "ice"
            case 3:
                return "electric"
            case 4:
                return "force"
            case 5:
                return "light"
            case 6:
                return "dark"
            case 7:
                return "almighty"
        }
    }
}