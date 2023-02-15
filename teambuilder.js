import {Battler} from "./battler.js"
import {Compendium} from "./data/compendium.js"
import {Party} from "./data/party.js"
import {Player} from "./data/player.js"
import fs from 'fs'

const compendium = new Compendium()

// MEDIUM MAGIC
const player1 = new Player("Toyota Carolla", 0, [50, 100, 135, 0, 20, 70, 31, 20], [77, 92, 111, 127, 142, 151, 240, 222])
// FULL DRACOSTRIKE
const player2 = new Player("Honda Civic", 0, [50, 150, 85, 70, 31, 0, 20, 20], [20, 82, 100, 119, 135, 147, 156, 239])

const rules = compendium.getRuleset(2)

const party1 = new Party(player1, rules)
const party2 = new Party(player2, rules)

// AILMENT SLIME
party1.pushNewActor(compendium.getDemon(0), [50, 180, 65, 17, 50, 0, 30, 50], [0, 5, 183, 184, 185, 186, 187, 188])
// HEALING/PARTY SUPPORT PIXIE
party1.pushNewActor(compendium.getDemon(1), [50, 90, 150, 0, 14, 50, 50, 30], [127, 202, 211, 207, 221, 222, 223, 245])
// FIRE MAG/ENEMY SUPPORT PYRO JACK
party1.pushNewActor(compendium.getDemon(37), [50, 50, 100, 0, 20, 50, 20, 0], [74, 75, 228, 229, 230, 240, 247, 250])

// party1.pushNewActor(compendium.getDemon(16), [15, 0, 0, 0, 0, 0, 0, 0], [3, 110])
// party1.pushNewActor(compendium.getDemon(9), [15, 0, 0, 0, 0, 0, 0, 0], [])
// party1.addItem(compendium.getItem(0), 5)

// AILMENT SLIME
party2.pushNewActor(compendium.getDemon(0), [50, 180, 65, 17, 50, 0, 30, 50], [0, 5, 183, 184, 185, 186, 187, 188])
// HEALING/PARTY SUPPORT PIXIE
party2.pushNewActor(compendium.getDemon(1), [50, 90, 150, 0, 14, 50, 50, 30], [127, 202, 211, 207, 221, 222, 223, 245])
// ICE MAG/ENEMY SUPPORT JACK FROST
party2.pushNewActor(compendium.getDemon(51), [50, 25, 100, 0, 15, 50, 10, 0], [92, 93, 228, 229, 230, 240, 247, 249])
// party2.addItem(compendium.getItem(0), 5)

const battler = new Battler(party1, party2)

var jsonObj = JSON.parse(JSON.stringify(party1));
//console.log(jsonObj);
 
// stringify JSON Object
var jsonContent = JSON.stringify(jsonObj);
//console.log(jsonContent);
 
fs.writeFile("output.json", jsonContent, 'utf8', function (err) {
    if (err) {
        console.log("An error occured while writing JSON Object to File.");
        return console.log(err);
    }
 
    console.log("JSON file has been saved.");
});

//battler.begin()