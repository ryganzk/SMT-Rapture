/*********************************************************************************
 * skillCompendium.js
 * 
 * A resource that holds information on every skill in the game, allowing the user
 * to access its contents when building their party
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

import {skillLinks} from "./skillLinks.js"

/*********************************************************************************
 *                       SKILL LEVEL IDS
 *   ----------------------------------------------------
 *   0: LOW (LEVELS 1-19)
 *   1: MEDIUM (LEVELS 20-49)
 *   2: HIGH (LEVELS 50-69)
 *   3: ELITE (LEVELS 70-100)
 * 
 *********************************************************************************/

/*********************************************************************************
 *                         TARGET IDS
 *   ----------------------------------------------------
 *   0: ONE
 *   1: ALL
 *   2: RANDOM
 *
 *********************************************************************************/
 
/*********************************************************************************
 *                      ATTACK TYPE IDS
 *   ----------------------------------------------------
 *   0: PHYSICAL
 *   1: FIRE
 *   2: ICE
 *   3: ELECTRIC
 *   4: FORCE
 *   5: LIGHT
 *   6: DARK
 *   7: ALMIGHTY
 *
 *********************************************************************************/

/*********************************************************************************
 * 
 *                     AILMENT TYPE IDS
 *   ----------------------------------------------------
 *   0: POISON
 *   1: CONFUSION
 *   2: CHARM
 *   3: SEAL
 *   4: SLEEP
 *   5: MIRAGE
 *
 *********************************************************************************/

/*********************************************************************************
 *                   RECOVERY AMOUNT IDS
 *   ----------------------------------------------------
 *   0: SLIGHT
 *   1: MODERATE
 *   2: FULL
 * 
 *********************************************************************************/

/*********************************************************************************
 *                 SUPPORT SKILL TYPE IDS
 *   ----------------------------------------------------
 *   0: SKILLS THAT RAISE ATTACK
 *   1: SKILLS THAT RAISE DEFENSE
 *   2: SKILLS THAT RAISE ACCURACY/EVASION
 *   3: SKILLS REGARDING PHYSICAL SKILLS
 *   4: SKILLS REGARDING FIRE SKILLS
 *   5: SKILLS REGARDING ICE SKILLS
 *   6: SKILLS REGARDING ELECTRIC SKILLS
 *   7: SKILLS REGARDING FORCE SKILLS
 *   8: SKILLS REGARDING LIGHT SKILLS
 *   9: SKILLS REGARDING DARK SKILLS
 *   10: SKILLS REGARDING ALMIGHTY SKILLS
 *   11: SKILLS REGARDING RECOVERY SKILLS
 *   12: SKILLS THAT RAISE MAX HP
 *   13: SKILLS THAT RAISE MAX MP
 *   14: SKILLS THAT RAISE ACCURACY
 *   15: SKILLS THAT RAISE POISON CHANCE
 *   16: SKILLS THAT INCREASE CRIT RATE
 *   17: SKILLS THAT INCREASE CRIT DAMAGE
 *   18: SKILLS THAT PROTECT FROM FATAL ATTACKS
 *   19: SKILLS THAT PROTECT FROM AILMENTS
 *   20: SKILLS THAT RESTORE HP AFTER ATTACKS
 *   21: SKILLS THAT RESTORE MP AFTER ATTACKS
 *   22: SKILLS THAT RESTORE MP AFTER CAUSING AILMENTS
 *   23: SKILLS THAT CAN COUNTER PHYSICALLY AFTER ATTACKS
 *   24: SAFEGUARD
 *   25: CRITICAL ZEALOT
 *   26: BOON BOOST
 *   27: HEAVENLY COUNTER
 *
 *********************************************************************************/

/*********************************************************************************
 *                  PASSIVE STRENGTH IDS
 *   ----------------------------------------------------
 *   0: WEAK
 *   1: GREAT
 *   2: RESIST (DEFENSIVE PASSIVE)
 *   3: NULL (DEFENSIVE PASSIVE)
 *   4: REFLECT (DEFENSIVE PASSIVE)
 *   5: DRAIN (DEFENSIVE PASSIVE)
 * 
 *********************************************************************************/

export class SkillCompendium {
    
    /*****************************************************************************
     * SkillCompendium constructor
     * 
     * @method
     *****************************************************************************/
    constructor() {
        this.skills = skillLinks
    }

    /*****************************************************************************
      * Returns the skill with the corresponding ID
      * 
      * @method
      * @param {int} skillID
      * @returns {Skill} 
      *****************************************************************************/
     getSkill(skillID) {
        return this.skills[skillID]
    }
}