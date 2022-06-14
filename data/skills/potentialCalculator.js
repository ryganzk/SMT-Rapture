export function determinePotentials(potentials, skill) {
    switch (true) {
        // PHYS SKILLS
        case skill.skillID < 73:
            skill.power = Math.round(skill.power * valueDamageMultiplier(potentials.physical)),
            skill.cost = Math.round(skill.cost * damageAilmentSkillCost(potentials.physical))
            break
        // FIRE SKILLS
        case skill.skillID < 90:
            skill.power = Math.round(skill.power * valueDamageMultiplier(potentials.fire))
            skill.cost = Math.round(skill.cost * damageAilmentSkillCost(potentials.fire))
            break
        // ICE SKILLS
        case skill.skillID < 109:
            skill.power = Math.round(skill.power * valueDamageMultiplier(potentials.ice))
            skill.cost = Math.round(skill.cost * damageAilmentSkillCost(potentials.ice))
            break
        // ELEC SKILLS
        case skill.skillID < 125:
            skill.power = Math.round(skill.power * valueDamageMultiplier(potentials.electric))
            skill.cost = Math.round(skill.cost * damageAilmentSkillCost(potentials.electric))
            break
        // FORCE SKILLS
        case skill.skillID < 140:
            skill.power = Math.round(skill.power * valueDamageMultiplier(potentials.force))
            skill.cost = Math.round(skill.cost * damageAilmentSkillCost(potentials.force))
            break
        // LIGHT SKILLS
        case skill.skillID < 149:
            skill.power = Math.round(skill.power * valueDamageMultiplier(potentials.light))
            skill.cost = Math.round(skill.cost * damageAilmentSkillCost(potentials.light))
            break
        // DARK SKILLS
        case skill.skillID < 162:
            skill.power = Math.round(skill.power * valueDamageMultiplier(potentials.dark))
            skill.cost = Math.round(skill.cost * damageAilmentSkillCost(potentials.dark))
            break
        // ALMIGHTY SKILLS
        case skill.skillID < 181:
            skill.power = Math.round(skill.power * valueDamageMultiplier(potentials.almighty))
            skill.cost = Math.round(skill.cost * damageAilmentSkillCost(potentials.almighty))
            break
        // AILMENT SKILLS
        case skill.skillID < 196:
            skill.accuracy = Math.round(skill.accuracy * valueAilmentAffliction(potentials.ailment))
            skill.cost = Math.round(skill.cost * damageAilmentSkillCost(potentials.ailment))
            break
        // HEAL SKILLS
        case skill.skillID < 216:
            skill.recoverAmnt = Math.round(skill.recoverAmnt * valueRecovery(potentials.heal))
            skill.cost = Math.round(skill.cost * recoverySupportCost(potentials.heal))
            break
        // SUPPORT SKILLS
        case skill.skillID < 253:
            skill.cost = Math.round(skill.cost * recoverySupportCost(potentials.support))
            break
    }
}

function valueDamageMultiplier(value) {
    switch (value) {
        case -7:
            return 0.57
        case -6:
            return 0.61
        case -5:
            return 0.65
        case -4:
            return 0.75
        case -3:
            return 0.8
        case -2:
            return 0.85
        case -1:
            return 0.9
        case 0:
            return 1
        case 1:
            return 1.1
        case 2:
            return 1.15
        case 3:
            return 1.2
        case 4:
            return 1.25
        case 5:
            return 1.35
        case 6:
            return 1.39
        case 7:
            return 1.43
        case 8:
            return 1.47
        case 9:
            return 1.55
    }
}

function valueAilmentAffliction(value) {
    switch (value) {
        case -7:
            return 0.55
        case -6:
            return 0.6
        case -5:
            return 0.65
        case -4:
            return 0.75
        case -3:
            return 0.8
        case -2:
            return 0.85
        case -1:
            return 0.9
        case 0:
            return 1
        case 1:
            return 1.1
        case 2:
            return 1.15
        case 3:
            return 1.2
        case 4:
            return 1.25
        case 5:
            return 1.35
        case 6:
            return 1.4
        case 7:
            return 1.45
        case 8:
            return 1.5
        case 9:
            return 1.6
    }
}

function valueRecovery(value) {
    switch (value) {
        case -5:
            return 0.6
        case -4:
            return 0.75
        case -3:
            return 0.8
        case -2:
            return 0.85
        case -1:
            return 0.9
        case 0:
            return 1
        case 1:
            return 1.1
        case 2:
            return 1.15
        case 3:
            return 1.2
        case 4:
            return 1.25
        case 5:
            return 1.40
    }
}

function damageAilmentSkillCost(value) {
    switch (value) {
        case -7:
            return 1.46
        case -6:
            return 1.4
        case -5:
            return 1.34
        case -4:
            return 1.25
        case -3:
            return 1.16
        case -2:
            return 1.13
        case -1:
            return 1.1
        case 0:
            return 1
        case 1:
            return 0.9
        case 2:
            return 0.87
        case 3:
            return 0.84
        case 4:
            return 0.81
        case 5:
            return 0.75
        case 6:
            return 0.72
        case 7:
            return 0.69
        case 8:
            return 0.66
        case 9:
            return 0.6
    }
}

function recoverySupportCost(value) {
    switch (value) {
        case -5:
            return 1.55
        case -4:
            return 1.5
        case -3:
            return 1.4
        case -2:
            return 1.3
        case -1:
            return 1.15
        case 0:
            return 1
        case 1:
            return 0.85
        case 2:
            return 0.8
        case 3:
            return 0.75
        case 4:
            return 0.7
        case 5:
            return 0.6
    }
}