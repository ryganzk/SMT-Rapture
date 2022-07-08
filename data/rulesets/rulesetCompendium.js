/*********************************************************************************
 * rulesetCompendium.js
 * 
 * A resource that holds information on every ruleset in the game, allowing the user
 * to access its contents when building their party
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

 import {rulesetLinks} from "./rulesetLinks.js"

export class RulesetCompendium {
    
    /*****************************************************************************
     * RulesetCompendium constructor
     * 
     * @method
     *****************************************************************************/
    constructor() {
        this.rulesets = rulesetLinks
    }

    /*****************************************************************************
      * Returns the ruleset with the corresponding ID
      * 
      * @method
      * @param {int} rulesetID
      * @returns {Ruleset} 
      *****************************************************************************/
     getRuleset(rulesetID) {
        return this.rulesets[rulesetID]
    }
}