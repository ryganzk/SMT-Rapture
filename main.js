import {Battler} from "./battler.js"
import {Compendium} from "./data/compendium.js"
import {Party} from "./data/party.js"
import {Player} from "./data/player.js"

const compendium = new Compendium()

const player1 = new Player("Toyota Carolla", 0, [5, 0, 0, 0, 0, 0, 0, 0], [109])
const player2 = new Player("Honda Civic", 0, [5, 0, 0, 0, 0, 0, 0, 0], [0])

const party1 = new Party(player1)
const party2 = new Party(player2)

party1.pushNewActor(compendium.getDemon(0), [5, 0, 15, 0, 0, 0, 0, 0], [50])
party1.pushNewActor(compendium.getDemon(1), [5, 15, 0, 0, 0, 0, 0, 0], [])
//party1.pushNewActor(compendium.getDemon(2), [5, 0, 0, 0, 0, 0, 0, 0], [])
party1.addItem(compendium.getItem(0), 5)

party2.pushNewActor(compendium.getDemon(0), [5, 0, 15, 0, 0, 0, 0, 0], [50])
party2.pushNewActor(compendium.getDemon(1), [5, 0, 0, 0, 0, 0, 0, 0], [])
party2.pushNewActor(compendium.getDemon(3), [5, 0, 0, 0, 0, 0, 0, 0], [])
party2.addItem(compendium.getItem(0), 5)


party1.displayDemon(1)
party2.displayDemon(1)

const battler = new Battler(party1, party2)
battler.begin()