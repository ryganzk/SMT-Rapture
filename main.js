import {Battler} from "./battler.js"
import {Compendium} from "./data/compendium.js"
import {Party} from "./data/party.js"
import {Player} from "./data/player.js"

const compendium = new Compendium()

const player1 = new Player("Toyota Carolla", 0, [50, 0, 120, 0, 0, 0, 50, 0], [186, 258, 110, 196, 299])
const player2 = new Player("Honda Civic", 0, [50, 0, 0, 0, 0, 10, 30, 0], [0, 114, 239, 243])

const rules = compendium.getRuleset(2)

const party1 = new Party(player1, rules)
const party2 = new Party(player2, rules)

party1.pushNewActor(compendium.getDemon(0), [50, 0, 15, 0, 0, 0, 10, 0], [])
party1.pushNewActor(compendium.getDemon(1), [15, 15, 20, 0, 0, 0, 0, 0], [204, 245])
party1.pushNewActor(compendium.getDemon(34), [20, 0, 0, 0, 0, 0, 0, 0], [3, 110, 297])
party1.pushNewActor(compendium.getDemon(16), [15, 0, 0, 0, 0, 0, 0, 0], [3, 110])
party1.pushNewActor(compendium.getDemon(9), [15, 0, 0, 0, 0, 0, 0, 0], [])
party1.addItem(compendium.getItem(0), 5)

party2.pushNewActor(compendium.getDemon(0), [50, 200, 0, 0, 0, 0, 0, 15], [])
party2.pushNewActor(compendium.getDemon(1), [5, 0, 0, 0, 0, 0, 0, 0], [210, 333])
party2.pushNewActor(compendium.getDemon(53), [49, 0, 0, 0, 60, 0, 0, 0], [])
party2.addItem(compendium.getItem(0), 5)

const battler = new Battler(party1, party2)
battler.begin()