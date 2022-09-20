/*********************************************************************************
 * playerCompendium.js
 * 
 * A resource that holds information on every player in the game, allowing the user
 * to access its contents when building their party
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

 import {playerLinks} from "./demonLinks.js"

    /*********************************************************************************
     *                   PLAYER RACE IDS
     *   ----------------------------------------------------
     *   0: NAHOBINO
     *   1: DEMI-FIEND
     * 
     *********************************************************************************/
     
     export class PlayerCompendium {
         
         /*****************************************************************************
          * PlayerCompendium constructor
          * 
          * @method
          *****************************************************************************/
         constructor() {
            this.players = playerLinks
         }
    
         /*****************************************************************************
          * Returns the player with the corresponding ID
          * 
          * @method
          * @param {int} playerID
          * @returns {Player} 
          *****************************************************************************/
          getPlayer(playerID) {
            return this.players[playerID]
        }
    }