/*********************************************************************************
 * demonCompendium.js
 * 
 * A resource that holds information on every demon in the game, allowing the user
 * to access its contents when building their party
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

 import {demonLinks} from "./demonLinks.js"

/*********************************************************************************
 *                   DEMON RACE IDS
 *   ----------------------------------------------------
 *   0: AVATAR
 *   1: AVIAN
 *   2: BEAST
 *   3: BRUTE
 *   4: DEITY
 *   5: DIVINE
 *   6: DRAGON
 *   7: DRAKE
 *   8: ELEMENT
 *   9: FAIRY
 *   10: FALLEN
 *   11: FEMME
 *   12: FIEND
 *   13: FOUL
 *   14: FURY
 *   15: GENMA
 *   16: HAUNT
 *   17: HERALD
 *   18: HOLY
 *   19: JAKI
 *   20: JIRAE
 *   21: KISHIN
 *   22: KUNITSU
 *   23: LADY
 *   24: MEGAMI
 *   25: MITAMA
 *   26: NIGHT
 *   27: RAPTOR
 *   28: SNAKE
 *   29: TYRANT
 *   30: VILE
 *   31: WARGOD
 *   32: WILDER
 *   33: YOMA
 *   34: NAHOBINO
 *   35: PANAGIA
 *   36: DEMI-FIEND
 * 
 *********************************************************************************/

/*********************************************************************************
 *                   DEMON REGION IDS
 *   ----------------------------------------------------
 *   0: NONE
 *   1: AMERICAN
 *   2: ENGLISH
 *   3: HINDU
 *   4: BUDDHIST
 *   5: JAPANESE
 *   6: CHINESE
 *   7: EUROPEAN
 *   8: GRECO-ROMAN
 *   9: ABRAHAMIC
 *   10: IRISH
 *   11: SCOTTISH
 *   12: OCCULT
 *   13: TIBETAN
 *   14: AUSTRALIAN
 *   15: LITHUANIAN
 *   16: CELTIC
 *   17: GOETIA
 *   18: KOREAN
 *   19: FILIPINO
 *   20: ZOROASTRIAN
 *   21: PERSIAN
 *   22: HAITIAN
 *   23: MESOPOTAMIAN
 *   24: WICCAN
 *   25: EGYPTIAN
 *   26: AZTEC
 *   27: NORSE
 *   28: LOVECRAFTIAN
 *   29: SLAVIC
 *   30: BALINESE
 *   31: SEMITIC
 *   32: CANAANITE
 *   33: VEDIC
 *   34: SRI LANKAN
 *   35: ARABIAN
 *   36: SUMERIAN
 * 
 *********************************************************************************/
 
 export class DemonCompendium {
     
     /*****************************************************************************
      * DemonCompendium constructor
      * 
      * @method
      *****************************************************************************/
     constructor() {
         this.demons = demonLinks
     }

     /*****************************************************************************
      * Returns the demon with the corresponding ID
      * 
      * @method
      * @param {int} demonID
      * @returns {Demon} 
      *****************************************************************************/
      getDemon(demonID) {
        let demon = this.demons[demonID]
        return JSON.parse(JSON.stringify(demon))
    }
}