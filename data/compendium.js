/*********************************************************************************
 * compendium.js
 * 
 * A resource that merges compendiums for demons, skills, and items into one giant
 * resource
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

import {DemonCompendium} from "./demons/demonCompendium.js"
import {SkillCompendium} from "./skills/skillCompendium.js"
import {ItemCompendium} from "./items/itemCompendium.js"

export class Compendium {

    /*****************************************************************************
      * Compendium constructor
      * 
      * @method
      *****************************************************************************/
    constructor() {
        this.demonComp = new DemonCompendium()
        this.skillComp = new SkillCompendium()
        this.itemComp = new ItemCompendium()
    }

    /*****************************************************************************
      * Returns the demon with the corresponding ID
      * 
      * @method
      * @param {int} demonID
      * @returns {Demon}
      *****************************************************************************/
     getDemon(demonID) {
        return this.demonComp.getDemon(demonID)
    }

    /*****************************************************************************
      * Returns the skill with the corresponding ID
      * 
      * @method
      * @param {int} skillID
      * @returns {Skill} 
      *****************************************************************************/
    getSkill(skillID) {
        return this.skillComp.getSkill(skillID)
    }

    /*****************************************************************************
      * Returns the item with the corresponding ID
      * 
      * @method
      * @param {int} itemID
      * @returns {Item} 
      *****************************************************************************/
     getItem(itemID) {
        return this.itemComp.getItem(itemID)
    }
}