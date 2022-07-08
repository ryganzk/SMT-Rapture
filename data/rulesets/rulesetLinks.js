/*********************************************************************************
 * rulesetLinks.js
 * 
 * Creates an array to hold ruleset information from the corresponsding JSON files
 * 
 * @author Ryan Ganzke (anOrgandroiD)
 * @version 1.0
 *********************************************************************************/

import fsPromises from 'fs/promises'

export const rulesetLinks = [
    JSON.parse(await fsPromises.readFile("./data/rulesets/levelCap/under20.json")),
    JSON.parse(await fsPromises.readFile("./data/rulesets/levelCap/under50.json")),
    JSON.parse(await fsPromises.readFile("./data/rulesets/demonCrack.json")),
]