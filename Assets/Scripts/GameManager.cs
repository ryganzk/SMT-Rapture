using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const int ATTACK_ID = 182;
    private const int AILMENT_ID = 200;
    private const int RECOVERY_ID = 220;
    private const int SUPPORT_ID = 258;
    private int turn = 1;

    public GameObject active;
    public Text actionCommand;
    public GameObject playerTeam, opponentTeam;
    public Animator cameraAnimator;
    public GameObject screen;
    public Image pressTurnIcon;
    public Sprite glowingPressTurnIcon;

    [SerializeReference]
    public List<Skill> skillCompendium;

    [System.Serializable]
    public class Skill
    {
        public string name;
        public string desc;
        public int skillID;
        public int level;
        public int unique;
    }

    [System.Serializable]
    public class NonPassiveSkill : Skill
    {
        public int cost;
        public int targets;
    }

    [System.Serializable]
    public class AttackSkill : NonPassiveSkill
    {
        public int power;
        public int type;
        public int accuracy;
        public int[] hits;
        public bool physical;
        public bool pierce;
    }

    [System.Serializable]
    public class AilmentSkill : NonPassiveSkill
    {
        public int[] ailments;
        public int accuracy;
    }

    [System.Serializable]
    public class RecoverySkill : NonPassiveSkill
    {
        public int recoverAmnt;
        public bool cure;
        public bool revive;
        public bool overheal;
    }

    [System.Serializable]
    public class SupportSkill : NonPassiveSkill
    {
        public int[] chargeID;
        public bool selfOnly;
    }

    [System.Serializable]
    public class PassiveSkill : Skill
    {
        public int[] passive;
        public int strength;
    }

    void ReadCompendiums()
    {
        skillCompendium = new List<Skill> {
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/lunge") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/bestialBite") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/hellishSlash") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/gramSlice") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/dreamNeedle") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/toxicSting") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/criticalSlash") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/madnessNeedle") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/aramasa") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/bouncingClaw") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/scratchDance") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/beserkerGod") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/needleSpray") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/fangBreaker") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/pierceArmor") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/blindingStrike") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/heavyBlow") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/heatWave") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/puncturePunch") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/venomChaser") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/beatdown") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/blight") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/carnageFang") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/crusherOnslaught") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/hystericalSlap") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/dreamFist") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/eatWhole") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/axelClaw") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/damascusClaw") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/criticalWave") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/darkSword") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/acrobatKick") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/fatalSword") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/severingBite") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/purpleSmoke") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/steelNeedle") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/frenziedChomp") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/mightyCleave") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/mortalJihad") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/deathbound") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/rampage") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/mistRush") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/wrathTempest") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/madnessNails") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/nihilClaw") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/powerPunch") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/hellThrust") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/braveBlade") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/akashicArts") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/macAnLuin") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/pandemonicCrush") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/megatonPress") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/yabusameShot") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/figmentSlash") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/myriadSlashes") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/hadesBlast") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/titanomachia") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/hassouTobi") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/catastrophe") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/karnak") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/gungnir") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/pantaSpane") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/dancingStrike") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/astralSaintstrike") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/headcrush") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/somersault") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/andalucia") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/hellSpin") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/terrorblade") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/pestilence") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/javelinRain") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/gaeaRage") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/physical/deadlyFury") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/agi") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/agilao") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/agidyne") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/agibarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/maragi") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/maragion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/maragidyne") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/maragibarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/fireBreath") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/fireDracostrike") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/ragnarok") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/trisagion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/ghastfireRain") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/ragingHellfire") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/mirageShot") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/hellishBrand") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/megidoFlame") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/fire/hellBurner") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/bufu") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/bufula") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/bufudyne") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/bufubarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/mabufu") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/mabufula") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/mabufudyne") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/mabufubarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/iceBreath") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/iceDracostrike") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/glacialBlast") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/iceAge") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/stormcallerSong") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/jackBufula") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/hellishSpurt") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/eternalBlizzard") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/kingBufula") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/ragingBlizzard") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/ice/thalassicCalamity") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/zio") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/zionga") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/ziodyne") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/ziobarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/mazio") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/mazionga") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/maziodyne") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/maziobarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/shockbound") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/stormDracostrike") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/thunderReign") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/narukami") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/ragingLightning") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/souffleDeclair") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/ruinousThunder") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/electric/keraunos") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/zan") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/zanma") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/zandyne") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/zanbarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/mazan") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/mazanma") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/mazandyne") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/mazanbarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/windBreath") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/windDracostrike") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/floralGust") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/killingWind") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/ragingTempest") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/sacrificeOfClay") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/force/hellExhaust") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/light/hama") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/light/hamaon") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/light/hamabarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/light/mahama") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/light/mahamaon") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/light/mahamabarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/light/whiteDracostrike") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/light/lightsDescent") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/light/godsBow") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/mudo") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/mudoon") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/mudobarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/mamudo") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/mamudoon") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/mamudobarion") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/blackDracostrike") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/dieForMe") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/toxicBreath") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/profanedLand") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/walpurgisnacht") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/deathFlies") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/dark/fallenDestroyer") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/megido") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/megidola") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/megidolaon") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/freikugel") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/lifeDrain") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/spiritDrain") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/energyDrain") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/sanguineDrain") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/violentRage") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/siltOfRuin") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/fireOfSinai") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/sakanagi") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/murakumo") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/divineArrowfall") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/deathLust") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/babylonGoblet") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/meditation") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/madnessGlint") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/tandava") as TextAsset).text),
            JsonUtility.FromJson<AttackSkill>((Resources.Load("Skills/almighty/soulDivide") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/dormina") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/dustoma") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/marinKarin") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/poisma") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/pulinpa") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/makajama") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/lullaby") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/fogna") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/sexyDance") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/toxicCloud") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/tentarafoo") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/makajamaon") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/toxicSpray") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/slumberVortex") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/frolic") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/stagnantAir") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/preach") as TextAsset).text),
            JsonUtility.FromJson<AilmentSkill>((Resources.Load("Skills/ailment/evilMelody") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/dia") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/diarama") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/diarahan") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/media") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/mediarama") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/mediarahan") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/patra") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/mePatra") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/recarm") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/samarecarm") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/diamrita") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/sunsRadiance") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/humbleBlessing") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/harvestDance") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/eleusinianHarvest") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/matriarchsLove") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/goldenApple") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/lightOfOrder") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/miracleWater") as TextAsset).text),
            JsonUtility.FromJson<RecoverySkill>((Resources.Load("Skills/recovery/revivalChant") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/tarukaja") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/rakukaja") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/sukukaja") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/matarukaja") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/marakukaja") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/masukukaja") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/lusterCandy") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/tarunda") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/rakunda") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/sukunda") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/matarunda") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/marakunda") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/masukunda") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/debilitate") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/cautiousCheer") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/redCapote") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/dekaja") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/dekunda") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/charge") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/concentrate") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/donumGladi") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/donumMagici") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/criticalAura") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/impalersAnimus") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/bowlOfHygieia") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/tetrakarn") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/makarakarn") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/physBlock") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/fireBlock") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/iceBlock") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/elecBlock") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/forceBlock") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/lightBlock") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/darkBlock") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/taunt") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/fierceRoar") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/witnessMe") as TextAsset).text),
            JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/kannabiVeil") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/lifeSpring") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/manaSpring") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/greatLifeSpring") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/greatManaSpring") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/lightLifeAid") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/lightManaAid") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/lifeAid") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/manaAid") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/resistPhys") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/resistFire") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/resistIce") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/resistElec") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/resistForce") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/resistLight") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/resistDark") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/nullPhys") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/nullFire") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/nullIce") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/nullElec") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/nullForce") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/nullLight") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/nullDark") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/repelPhys") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/repelFire") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/repelIce") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/repelElec") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/repelForce") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/repelLight") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/repelDark") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/drainPhys") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/drainFire") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/drainIce") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/drainElec") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/drainForce") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/drainLight") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/drainDark") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/hellishMask") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/abyssalMask") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/restore") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/highRestore") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/curseSiphon") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/greatCurseSiphon") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/counter") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/retaliate") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/heavenlyCounter") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/safeguard") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/beastEye") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/dragonEye") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/bloodyGlee") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/murderousGlee") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/criticalZealot") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/physPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/firePleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/icePleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/elecPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/forcePleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/lightPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/darkPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/almightyPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/healPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/highPhysPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/highFirePleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/highIcePleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/highElecPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/highForcePleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/highLightPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/highDarkPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/highAlmightyPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/highHealPleroma") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/poisonAdept") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/poisonMaster") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/boonBoost") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/boonBoostEX") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/endure") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/enduringSoul") as TextAsset).text),
            JsonUtility.FromJson<PassiveSkill>((Resources.Load("Skills/passive/inspiringLeader") as TextAsset).text)
        };
    }

    public void BackToSideView()
    {
        cameraAnimator.Play("BackToSideView");
    }

    public int DetermineSkillType(Skill skill)
    {
        switch (skill.skillID)
        {
            case int id when (id <= ATTACK_ID):
                return 0;
            case int id when (id <= AILMENT_ID):
                return 1;
            case int id when (id <= RECOVERY_ID):
                return 2;
            default:
                return 3;
        }
    }

    public void Guard()
    {
        active.GetComponent<Animator>().SetTrigger("guardStart");
        active.GetComponent<ActorStats>().guard = true;
        NextUp(1);
    }

    public void GuardEnd()
    {
        active.GetComponent<Animator>().SetTrigger("guardEnd");
        active.GetComponent<ActorStats>().guard = false;
    }

    public void NextUp(int val)
    {
        var team = playerTeam.GetComponent<Team>();
        if (active == team.player)
        {
            active = team.activeDemons[0];
        }
        else
        {
            active = team.player;
        }

        UpdatePress(val);
        UpdateName();

        if (active.GetComponent<ActorStats>().guard)
        {
            GuardEnd();
        }
    }

    public void ChangeDemons(GameObject demon)
    {
        var temp = active;
        demon.SetActive(true);
        active.SetActive(false);

        var team = GetComponent<GameManager>().playerTeam.GetComponent<Team>().activeDemons;
        team[team.IndexOf(active)] = demon;

        demon.transform.position = active.transform.position;
        demon.transform.rotation = active.transform.rotation;
        active = demon;

        NextUp(0);
    }

    public void SwitchTeams()
    {
        var temp = playerTeam;
        playerTeam = opponentTeam;
        opponentTeam = temp;

        if (playerTeam.GetComponent<Team>().homeTeam)
        {
            screen.transform.Find("Turn").GetComponent<Text>().text = "Ally Turn";
            ++turn;
            screen.transform.Find("TurnNumber").GetComponent<Text>().text = turn.ToString();
        }
        else
            screen.transform.Find("Turn").GetComponent<Text>().text = "Enemy Turn";
    }

    public void CreatePartyTurns()
    {
        Transform pressTurnPane = screen.transform.Find("PressTurns");

        Color glow;
        if (playerTeam.GetComponent<Team>().homeTeam)
            glow = Color.cyan;
        else
            glow = Color.red;

        for (int i = 0; i < playerTeam.GetComponent<Team>().activeDemons.Count + 1; ++i)
        {

            CreatePressTurn(i, pressTurnPane, glow);
        }
    }

    void CreatePressTurn(int offset, Transform pressTurnPane, Color glow)
    {
        var pressTurn = Instantiate(pressTurnIcon, Vector3.zero, Quaternion.identity) as Image;
        pressTurn.transform.SetParent(pressTurnPane.transform);
        pressTurn.color = glow;
        var rectTransform = pressTurn.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(1, 0);
        rectTransform.position = new Vector3(1830 - (100 * offset), 990, 0);
    }

    public void UpdatePress(int val)
    {
        Transform pressTurnPane = screen.transform.Find("PressTurns");
        switch (val)
        {
            //WEAK, PASS, CHANGE
            case 0:
                ExtraTurn(pressTurnPane);
                break;
            //NORMAL, RESIST, GUARD
            case 1:
                DeleteTurns(1, pressTurnPane);
                break;
            //MISS, NULL
            case 2:
                DeleteTurns(2, pressTurnPane);
                break;
            //REPEL, DRAIN
            case 3:
                DeleteTurns(4, pressTurnPane);
                break;
        }
    }

    private void DeleteTurns(int val, Transform pressTurnPane)
    {
        for (int i = 0; i < val; ++i)
        {
            Destroy(pressTurnPane.transform.GetChild(pressTurnPane.transform.childCount - 1).gameObject);
        }
    
        if (pressTurnPane.transform.childCount == 1)
        {
            SwitchTeams();
            CreatePartyTurns();
            active = playerTeam.GetComponent<Team>().player;
        }
    }

    private void ExtraTurn(Transform pressTurnPane)
    {
        for (int i = (pressTurnPane.transform.childCount - 1); i >= 0; --i)
        {
            if (pressTurnPane.transform.GetChild(i).GetComponent<Image>().sprite == pressTurnIcon.sprite)
            {
                pressTurnPane.transform.GetChild(i).GetComponent<Image>().sprite = glowingPressTurnIcon;
                return;
            }
        }

        DeleteTurns(1, pressTurnPane);
    }

    public void UpdateName()
    {
        actionCommand.text = "WHAT WILL " + active.GetComponent<ActorStats>().stats.name + " DO?";
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {
        ReadCompendiums();

        foreach (Transform child in playerTeam.transform)
        {
            child.GetComponent<ActorStats>().LoadCharacter();
            var childStats = child.GetComponent<ActorStats>().stats;

            foreach (int skillID in childStats.baseSkills)
            {
                childStats.skills.Add(skillCompendium[skillID]);
            }
            
            if (child.GetSiblingIndex() >= 1 && child.GetSiblingIndex() < 2)
                playerTeam.GetComponent<Team>().activeDemons.Add(child.gameObject);

            if (child.name == "nahobino")
                childStats.skills.Add(JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/tarunda") as TextAsset).text));
        }

        foreach (Transform child in opponentTeam.transform)
        {
            child.GetComponent<ActorStats>().LoadCharacter();
            var childStats = child.GetComponent<ActorStats>().stats;

            foreach (int skillID in childStats.baseSkills)
            {
                childStats.skills.Add(skillCompendium[skillID]);
            }

            if (child.GetSiblingIndex() >= 1 && child.GetSiblingIndex() < 2)
                opponentTeam.GetComponent<Team>().activeDemons.Add(child.gameObject);

            if (child.name == "nahobino")
                childStats.skills.Add(JsonUtility.FromJson<SupportSkill>((Resources.Load("Skills/support/lusterCandy") as TextAsset).text));
        }


        UpdateName();
        CreatePartyTurns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
