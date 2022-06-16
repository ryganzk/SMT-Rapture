/*********************************************************************************
 * itemCompendium.js
 * 
 * A resource that holds information on every item in the game, allowing the user
 * to access its contents when building their party
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

 import {itemLinks} from "./itemLinks.js"

 
 export class ItemCompendium {
     
     /*****************************************************************************
      * ItemCompendium constructor
      * 
      * @method
      *****************************************************************************/
     constructor() {
         this.items = itemLinks
     }
 
     /*****************************************************************************
      * Returns the item with the corresponding ID
      * 
      * @method
      * @param {int} itemID
      * @returns {Item} 
      *****************************************************************************/
      getItem(itemID) {
        return this.items[itemID]
    }
 }