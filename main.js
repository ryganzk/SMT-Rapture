import {Compendium} from "./data/compendium.js"
import {Party} from "./data/party.js"
import {Player} from "./data/player.js"

let player = new Player("Toyota Carolla", [5, 0, 12, 0, 0, 0, 0, 0], [109])
let compendium = new Compendium()
let party = new Party(player)
party.pushNewActor(compendium.getDemon(0), [5, 0, 15, 0, 0, 0, 0, 0], [0, 184])
party.pushNewActor(compendium.getDemon(1), [5, 0, 15, 0, 0, 0, 0, 0], [125, 196])
party.addItem(compendium.getItem(0), 5)
party.displayParty()