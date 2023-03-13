using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static AI;

public class GameManager : MonoBehaviour
{
    public const int ATTACK_ID = 182;
    public const int AILMENT_ID = 200;
    public const int RECOVERY_ID = 220;
    public const int SUPPORT_ID = 258;
    public const int SIDE_EFFECT_RATE = 40;
    private int turn = 1;
    private int activeIndex = 3;

    public Camera cam;
    public GameObject active;
    public Text actionCommand;
    public GameObject playerTeam, opponentTeam;
    public Animator cameraAnimator;
    public Image pressTurnIcon;
    public Canvas screen;
    public Canvas mainScreen;
    public Canvas gameOverScreen;
    public Sprite glowingPressTurnIcon;
    public GameObject aiType;
    public GameObject flavorText, switchText;
    public GameObject demonDex;
    public GameObject battlePositions;

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
    public class PseudoSupportSkill : NonPassiveSkill
    {
        public List<int> support;
        public int suppAmnt;
        public bool buff;
    }

    [System.Serializable]
    public class AilmentSkill : PseudoSupportSkill
    {
        public List<int> ailments;
        public int accuracy;
    }

    [System.Serializable]
    public class AttackSkill : AilmentSkill
    {
        public int power;
        public int type;
        public List<int> hits;
        public bool physical;
        public bool pierce;
        public int ohko;
        public int critBonus;
    }

    [System.Serializable]
    public class RecoverySkill : NonPassiveSkill
    {
        public int recoverAmnt;
        public int recoverPrct;
        public bool cure;
        public bool revive;
        public bool overheal;
    }

    [System.Serializable]
    public class SupportSkill : PseudoSupportSkill
    {
        public bool selfOnly;
        public List<int> charge;
        public List<int> block;
        public bool veil;
        public bool taunt;
    }

    [System.Serializable]
    public class PassiveSkill : Skill
    {
        public List<int> passive;
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

    public List<GameObject> AliveDemons(List<GameObject> objs)
    {
        List<GameObject> aliveList = new List<GameObject>();

        // Adds a demon to target if they exit in the list
        foreach(GameObject obj in objs)
        {
            if (obj != null && obj.GetComponent<ActorStats>().stats.battleStats.hp > 0)
                aliveList.Add(obj);
        }

        // Allows player targeting if not all demons are active OR self-inflicted boost/heal
        if (aliveList.Count != 3 || objs[0].transform.parent.gameObject == playerTeam)
            aliveList.Add(opponentTeam.GetComponent<Team>().player);
        
        return aliveList;
    }

    // ONLY FOR USE WHEN SWITCHING TURNS
    public List<GameObject> AliveDemons(List<GameObject> objs, bool includePlayer)
    {
        List<GameObject> aliveList = new List<GameObject>();

        // Adds a demon to target if they exit in the list
        foreach (GameObject obj in objs)
        {
            if (obj != null && obj.GetComponent<ActorStats>().stats.battleStats.hp > 0)
                aliveList.Add(obj);
        }

        // Allows player targeting if not all demons are active OR self-inflicted boost/heal
        if (includePlayer)
            aliveList.Add(playerTeam.GetComponent<Team>().player);

        return aliveList;
    }

    public void ExecuteMove(GameObject obj, NonPassiveSkill skill, List<GameObject> team)
    {
        int result = 0;
        if (skill.name == "Attack")
            flavorText.GetComponent<Text>().text = active.GetComponent<ActorStats>().stats.name + " Attacked!";
        else
            flavorText.GetComponent<Text>().text = active.GetComponent<ActorStats>().stats.name + " Used " + skill.name + "!";
        switch (DetermineSkillType(skill))
        {
            case 0:
                if (((AttackSkill) skill).physical)
                    active.GetComponent<Animator>().SetTrigger("skillAtk");
                else
                    active.GetComponent<Animator>().SetTrigger("skillRcv");

                switch (skill.targets)
                {
                    case 1:
                        result = PartyDamage((AttackSkill) skill, team);
                        break;
                    case 2:
                        result = RandDamage((AttackSkill) skill, team);
                        break;
                    default:
                        result = Damage((AttackSkill) skill, obj);
                        break;
                }   

                RemoveCharge((AttackSkill) skill, active.GetComponent<ActorStats>());
                break;
            case 1:
                active.GetComponent<Animator>().SetTrigger("skillRcv");
                switch (skill.targets)
                {
                    case 1:
                        result = PartyAilment((AilmentSkill) skill, obj.transform.GetComponentInParent<Team>().activeDemons);
                        break;
                    default:
                        result = Ailment((AilmentSkill) skill, obj);
                        break;
                }
                break;
            case 2:
                active.GetComponent<Animator>().SetTrigger("skillRcv");
                switch (skill.targets)
                {
                    case 1:
                        PartyHeal((RecoverySkill) skill, obj.transform.GetComponentInParent<Team>().activeDemons);
                        break;
                    default:
                        Heal((RecoverySkill) skill, obj);
                        break;
                }

                RemoveCharge((RecoverySkill) skill, active.GetComponent<ActorStats>());
                break;
            case 3:
                active.GetComponent<Animator>().SetTrigger("skillRcv");
                switch (skill.targets)
                {
                    case 1:
                        PartySupport((SupportSkill) skill, obj.transform.GetComponentInParent<Team>().activeDemons);
                        break;
                    default:
                        Support((SupportSkill) skill, obj);
                        break;
                }
                break;

        }

        // Swap results of 1 and 0
        if (result == 1)
            result = 0;
        else if (result == 0)
            result = 1;
        
        active.GetComponent<ActorStats>().stats.battleStats.mp -= skill.cost;
        StartCoroutine(Delay(result));
    }

    public int PartyDamage(AttackSkill skill, List<GameObject> objs)
    {
        int result = 0;
        objs = AliveDemons(objs);

        foreach (GameObject obj in objs)
        {
            int tempResult = Damage (skill, obj);
            if (tempResult > result)
                result = tempResult;
        }
        return result;
    }

    public int RandDamage(AttackSkill skill, List<GameObject> objs)
    {
        int result = 0;
        objs = AliveDemons(objs);

        int maxHits;
        if (skill.hits.Count == 1)
            maxHits = skill.hits[0];
        else
            maxHits = skill.hits[1];

        //Debug.Log("HITS: " + (int) Math.Floor((double) UnityEngine.Random.Range(skill.hits[0], maxHits)));

        for (int i = 0; i < (int) Math.Floor((double) UnityEngine.Random.Range(skill.hits[0], maxHits)); ++i) {
            int tempResult = Damage(skill, objs[(int) Math.Floor((double) UnityEngine.Random.Range(0, objs.Count))]);

            if (tempResult > result)
                result = tempResult;
        }
        return result;
    }

    public int Damage(AttackSkill skill, GameObject obj)
    {
        var attacker = active.GetComponent<ActorStats>();
        var defender = obj.GetComponent<ActorStats>();

        // TAUNT
        foreach (GameObject dem in opponentTeam.GetComponent<Team>().activeDemons)
        {
            var demon = dem.GetComponent<ActorStats>();
            if (demon.taunt[0] != 0)
            {
                var tauntChance = 30 * (((demon.stats.baseStats.luck / 2) + (demon.stats.baseStats.agility / 4)) 
                    / ((attacker.stats.baseStats.luck / 2) + (attacker.stats.baseStats.agility / 4)));
                
                if ((UnityEngine.Random.Range(0f, 1f) * 100) < tauntChance)
                {
                    defender = demon;
                    break;
                }
            }
        }

        float resMod = CalculateResistance(skill, attacker, defender);
        float chargeVal = 1;
        var affected = defender;
        string result = "";

        // PROTECTIVE BARRIER
        if (resMod == 0 || defender.protective == skill.type + 3)
        {
            resMod = 0;
            result = "NULL";
        }

        // Moves affected by guard/miss/critical
        if (resMod > 0)
        { 
            // MISS
            if (!CalculateHit(skill, attacker, defender, 0))
            {
                result = "MISS";
                resMod = 0;
            }

            // GUARDING
            else if  (defender.guard && resMod >= 1)
            {
                resMod = 0.8f;
                result = "GUARD";
            }

            // CRITICAL
            else
            {
                if (CalculateCritRate(skill, attacker, defender))
                {
                    resMod *= (1.5f * (1 + (attacker.passives[21] * 0.3f)));
                    result = "CRITICAL";
                }
                else
                    resMod *= (1 - (attacker.passives[21] * 0.1f));
            }
        }

        // REFLECT
        if (resMod == -0.5f || (defender.protective == 1 && skill.type == 0) || (defender.protective == 2 && skill.type > 0 && skill.type < 7))
        {
            resMod = 1;
            result = "REPEL";
            affected = attacker;
        }

        else if (resMod == 2)
        {
            result = "WEAK";

            // Calculated OHKO if weak
            if (skill.ohko > 0 && CalculateInstaKill(skill, attacker, defender))
            {
                affected.stats.battleStats.hp = 0;
            }
        }

        if (resMod == -1)
            result = "DRAIN";
            

        if (attacker.charge == 3 || (skill.physical && attacker.charge == 1) || (!skill.physical && attacker.charge == 2))
        {
            chargeVal = 1.8f;
        }

        // Determine if physical or magical attack
        var demPower = 0;
        if (skill.physical)
            demPower = attacker.stats.baseStats.strength;
        else
            demPower = attacker.stats.baseStats.magic;

        var damage = Math.Pow(demPower, 2)
                / (defender.stats.baseStats.vitality * 1.5f)
                * (1 + (skill.power / 100f))
                * resMod
                * ((UnityEngine.Random.Range(0f, 1f) / 3) + 1)
                * CalculateBattleBuff(attacker, 0)
                * (2 - CalculateBattleBuff(defender, 1))
                * (1 - (defender.damageDown * 0.3f))
                * chargeVal
                * PassiveIncreaser(attacker.passives[skill.type + 2])
                + (1 * resMod);

        if (result != "")
            Debug.Log(affected.name + " TOOK " + damage + " DAMAGE! " + result + "!");
        else
            Debug.Log(affected.name + " TOOK " + damage + " DAMAGE!");

        affected.stats.battleStats.hp -= (int) damage;

        // If drain overheals
        if (affected.stats.battleStats.hp > affected.stats.baseStats.hp)
            affected.stats.battleStats.hp = affected.stats.baseStats.hp;

        int pressResult = 0;
        if (result == "WEAK" || result == "CRITICAL")
        {
            if (attacker.passives[12] != 0)
            {
                int healAmnt = (
                    RestoreAmnt(attacker.passives[12])
                    > attacker.stats.baseStats.hp - attacker.stats.battleStats.hp
                    ? attacker.stats.baseStats.hp - attacker.stats.battleStats.hp
                    : RestoreAmnt(attacker.passives[12])
                );
                attacker.stats.battleStats.hp += healAmnt;
            }
            pressResult = 1;
        }
        else if (result == "NULL" || result == "MISS")
            pressResult = 2;
        else if (result == "REPEL" || result == "DRAIN")
            pressResult = 3;

        // Update known data 
        if (result != "MISS" && result != "GUARD" && skill.type != 7)
            playerTeam.GetComponent<Team>().UpdateOpposingData(playerTeam.GetComponent<Team>().FindOpposingData(obj), (NonPassiveSkill) skill, defender);

        // If the attack defeats the defender
        if (affected.stats.battleStats.hp <= 0)
        {
            affected.stats.battleStats.hp = 0;

            // If the attacker kills itself (REPEL)
            if (active.GetComponent<ActorStats>() == affected)
            {
                // If the attacker IS the opposing player
                if(active == playerTeam.GetComponent<Team>().player)
                {
                    StartCoroutine(GameOver());
                    active.SetActive(false);
                    return 4;
                }

                active.SetActive(false);
                return pressResult;
            }

            // If the attacker kills the opposing player
            if (obj == opponentTeam.GetComponent<Team>().player)
            {
                StartCoroutine(GameOver());
                obj.SetActive(false);
                return 4;
            }

            obj.SetActive(false);
        }

        // Apply support effects if attacked demon is still alive
        if (pressResult < 2 && skill.support.Count > 0 && affected.stats.battleStats.hp > 0)
        {
            PseudoSupport((PseudoSupportSkill) skill, obj);
        }

        return pressResult;
    }

    public int GetResistance(AttackSkill skill, ActorStats defender)
    {
        switch (skill.type)
        {
            case 0:
                return defender.stats.resistances.physical;
            case 1:
                return defender.stats.resistances.fire;
            case 2:
                return defender.stats.resistances.ice;
            case 3:
                return defender.stats.resistances.electric;
            case 4:
                return defender.stats.resistances.force;
            case 5:
                return defender.stats.resistances.light;
            case 6:
                return defender.stats.resistances.dark;
            default:
                return 1;
        }
    }

    private float CalculateResistance(AttackSkill skill, ActorStats attacker, ActorStats defender)
    {
        int skillRes = GetResistance(skill, defender);

        // Proceed if skill pierces or under effects of impaler animus
        if (skill.pierce || attacker.charge == 3)
        {
            if (skillRes == 0)
                return 2;
            else
                return 1;
        }
        
        switch (skillRes)
        {
            case 0:
                return 2;
            case 1:
                return 1;
            case 2:
                return 0.5f;
            case 3:
                return 0;
            case 4:
                return -0.5f;
            case 5:
                return -1;
            default:
                return 1;
        }
    }

    private bool CalculateHit(AilmentSkill skill, ActorStats attacker, ActorStats defender, int ailmentType)
    {
        if (defender.ailment[0] == 5 || attacker.charge == 1)
            return true;

        float accuracy = skill.accuracy * (CalculateDemonHitAvoid(attacker) / CalculateDemonHitAvoid(defender)) * PassiveIncreaser(attacker.passives[15]);

        // If pure ailment skill, take the defender's resistances to determine accuracy
        if (skill.skillID >= ATTACK_ID)
            accuracy *= (1.0f / AilmentLookup(defender, ailmentType)) * PassiveDecreaser(defender.passives[11]);

        // If attacker is inflicted with mirage, lower accuracy
        if (attacker.ailment[0] == 6)
            accuracy /= 2;

        if (UnityEngine.Random.Range(0, 100) < accuracy)
            return true;
        else
            return false;
    }

    private float CalculateDemonHitAvoid(ActorStats demon)
    {
        return (demon.stats.baseStats.agility / 2.0f) + (demon.stats.baseStats.luck / 4.0f) * CalculateBattleBuff(demon, 2);
    }

    private int AilmentLookup(ActorStats demon, int type)
    {
        switch (type)
        {
            case 0:
                return demon.stats.ailmentResistances.poison;
            case 1:
                return demon.stats.ailmentResistances.confusion;
            case 2:
                return demon.stats.ailmentResistances.charm;
            case 3:
                return demon.stats.ailmentResistances.seal;
            case 4:
                return demon.stats.ailmentResistances.sleep;
            case 5:
                return demon.stats.ailmentResistances.mirage;
            default:
                return 1;
        }
    }

    private bool CalculateCritRate(AttackSkill skill, ActorStats attacker, ActorStats defender)
    {
        // Skills with 200 crit bonus are guaranteed crits
        if (skill.critBonus == 200)
            return true;

        // CRITICAL AURA
        if (skill.type == 0 && attacker.charge == 4)
            return true;

        float crit = skill.critBonus + (CalculateDemonCritAvoid(attacker) / CalculateDemonCritAvoid(defender)) * PassiveIncreaser(attacker.passives[16]) + 6.25f;

        if (UnityEngine.Random.Range(0, 100) < crit)
            return true;
        else
            return false;
    }

    private float CalculateDemonCritAvoid(ActorStats demon)
    {
        return (demon.stats.baseStats.luck / 2.0f) + (demon.stats.baseStats.agility / 4.0f);
    }

    private float CalculateBattleBuff(ActorStats demon, int type)
    {
        switch (type)
        {
            case 0:
                return (1 + demon.attack[0] * 0.2f);
            case 1:
                return (1 + demon.defense[0] * 0.2f);
            case 2:
                return (1 + demon.accEvas[0] * 0.2f);
            default:
                return 1;
        }
    }

    private bool CalculateInstaKill(AttackSkill skill, ActorStats attacker, ActorStats defender)
    {
        float killChance = skill.ohko * CalculateDemonInstaKillChance(attacker) / CalculateDemonInstaKillChance(defender) * PassiveDecreaser(defender.passives[11]);

        if (UnityEngine.Random.Range(0, 100) < killChance)
            return true;
        else
            return false;
    }

    private float CalculateDemonInstaKillChance(ActorStats demon)
    {
        return (demon.stats.baseStats.luck / 2.0f) + (demon.stats.baseStats.agility / 4.0f);
    }

    private float PassiveIncreaser(int amnt)
    {
        switch (amnt)
        {
            case 1:
                return 1.2f;
            case 2:
                return 1.35f;
            case 3:
                return 1.55f;
            default:
                return 1;
        }
    }

    private float PassiveDecreaser(int amnt)
    {
        switch (amnt)
        {
            case 1:
                return 0.8f;
            case 2:
                return 0.65f;
            case 3:
                return 0.45f;
            default:
                return 1;
        }
    }

    private int RestoreAmnt(int amnt)
    {
        switch (amnt)
        {
            case 1:
                return 10;
            case 2:
                return 20;
            case 3:
                return 30;
            default:
                return 0;
        }
    }

    public int PartyAilment(AilmentSkill skill, List<GameObject> objs)
    {
        int result = 0;
        objs = AliveDemons(objs);

        foreach (GameObject obj in objs)
        {
            int tempResult = Ailment (skill, obj);
            if (tempResult > result)
                result = tempResult;
        }
        return result;
    }

    public int Ailment(AilmentSkill skill, GameObject obj)
    {
        var attacker = active.GetComponent<ActorStats>();
        var defender = obj.GetComponent<ActorStats>();
        int result = 3;

        foreach (int i in skill.ailments)
        {
            var resType = AilmentLookup(defender, i);

            // Nulls ailment (remember to check other possible ailments)
            if (resType == 3)
            {
                Debug.Log(defender.name + " nulls the ailment...");
                result = 2;
            }
            else if (resType == 0)
            {
                InflictAilment(skill, obj, attacker, defender, i);
                return 1;
            }
            else 
            {
                if (CalculateHit(skill, attacker, defender, resType))
                {
                    InflictAilment(skill, obj, attacker, defender, i);
                    return 0;
                }
                else
                    Debug.Log(defender.name + " avoided the ailment...");
            }
        }

        // If no null, treat as normal
        if (result == 3)
            return 0;

        return result;
    }

    void InflictAilment(AilmentSkill skill, GameObject obj, ActorStats attacker, ActorStats defender, int ailmentIndex)
    {
        Debug.Log(defender.name + " was inflicted...");
        PseudoSupport(skill, obj);
        CurseSiphoon(attacker);
        defender.ailment = new List<int>{ ailmentIndex + 1, 0, attacker.passives[17] };
    }

    private void CurseSiphoon(ActorStats demon)
    {
        if (demon.passives[13] != 0)
        {
            int healAmnt = RestoreAmnt(demon.passives[13])
                > demon.stats.baseStats.mp - demon.stats.battleStats.mp
                ? demon.stats.baseStats.mp - demon.stats.battleStats.mp
                : RestoreAmnt(demon.passives[13]);
            demon.stats.battleStats.mp += healAmnt;
        }
    }

    float PoisonRate(int amnt) {
        switch(amnt) {
            case 0:
                return 16f;
            case 1:
                return 12f;
            case 2:
                return 8f;
            case 3:
                return 4f;
            default:
                return 16f;
        }
    }

    public void PartyHeal(RecoverySkill skill, List<GameObject> objs)
    {
        objs = AliveDemons(objs);

        foreach (GameObject obj in objs)
        {
            Heal (skill, obj);
        }
    }

    public void Heal(RecoverySkill skill, GameObject obj)
    {
        var healer = active.GetComponent<ActorStats>();
        var recipient = obj.GetComponent<ActorStats>();

        var recovery = 1.0;
        int overheal = 0;

        // BOWL OF HYGIEIA
        if (healer.charge == 5)
        {
            recovery = 1.5f;
            overheal = 1;
        }

        if (skill.recoverAmnt != 0)
            recovery *= skill.recoverAmnt * ((Math.Pow(healer.stats.baseStats.magic, 2) / 225.0f) + 1);
        else
            recovery *= (skill.recoverPrct / 100.0f) * recipient.stats.baseStats.hp;

        // Overheal formula
        if (recipient.stats.battleStats.hp + recovery > recipient.stats.battleStats.hp * OverhealExpr(overheal))
            recovery = (recipient.stats.baseStats.hp * OverhealExpr(overheal)) - recipient.stats.battleStats.hp;

        recipient.stats.battleStats.hp += (int) recovery;
    }

    private float OverhealExpr(int overheal)
    {
        return (1 + (overheal * 0.3f));
    }

    public void PartySupport(SupportSkill skill, List<GameObject> objs)
    {
        objs = AliveDemons(objs);

        foreach (GameObject obj in objs)
            Support(skill, obj);
    }

    public void PseudoSupport(PseudoSupportSkill skill, GameObject obj)
    {
        var caster = active.GetComponent<ActorStats>();
        var recipient = obj.GetComponent<ActorStats>();

        if (skill.support.Count > 0)
            CalculatePseudoBuff(skill, caster, recipient);
    }

    public void Support(SupportSkill skill, GameObject obj)
    {
        var caster = active.GetComponent<ActorStats>();
        var recipient = obj.GetComponent<ActorStats>();

        if (skill.support.Count > 0)
            CalculateBuff(skill, caster, recipient);
        if (skill.charge.Count > 0)
            CalculateCharge(skill, recipient);
        if (skill.block.Count > 0)
            CalculateProtection(skill, recipient);
        if (skill.veil)
            CalculateDamageDown(recipient);
        if (skill.taunt)
            CalculateTaunt(recipient);
    }

    private void CalculatePseudoBuff(PseudoSupportSkill skill, ActorStats caster, ActorStats recipient)
    {
        foreach (int suppId in skill.support)
        {
            switch (suppId)
            {
                // ATTACK
                case 0:
                    PseudoBuffHelper(recipient.attack, skill, caster, recipient);
                    break;
                case 1:
                    PseudoBuffHelper(recipient.attack, skill, caster, recipient);
                    break;
                case 2:
                    PseudoBuffHelper(recipient.accEvas, skill, caster, recipient);
                    break;
                case 3:
                    recipient.attack = new List<int>{ 0, 0 };
                    recipient.defense = new List<int>{ 0, 0 };
                    recipient.accEvas = new List<int>{ 0, 0 };
                    return;
            }
        }
    }

    private void CalculateBuff(SupportSkill skill, ActorStats caster, ActorStats recipient)
    {
        foreach (int suppId in skill.support)
        {
            switch (suppId)
            {
                // ATTACK
                case 0:
                    BuffHelper(recipient.attack, skill, caster, recipient);
                    break;
                case 1:
                    BuffHelper(recipient.attack, skill, caster, recipient);
                    break;
                case 2:
                    BuffHelper(recipient.accEvas, skill, caster, recipient);
                    break;
                case 3:
                    recipient.attack = new List<int>{ 0, 0 };
                    recipient.defense = new List<int>{ 0, 0 };
                    recipient.accEvas = new List<int>{ 0, 0 };
                    return;
            }
        }
    }

    private void PseudoBuffHelper(List<int> effect, PseudoSupportSkill skill, ActorStats caster, ActorStats recipient)
    {
        if (skill.buff)
        {
            effect[0] += skill.suppAmnt;
            if (effect[0] > 2)
                effect[0] = 2;
        }

        else if (!skill.buff)
        {
            effect[0] -= skill.suppAmnt;
            if (effect[0] < -2)
                effect[0] = -2;
        }

        effect[1] = 3 + caster.passives[18];
    }

    private void BuffHelper(List<int> effect, SupportSkill skill, ActorStats caster, ActorStats recipient)
    {
        if (skill.buff)
        {
            effect[0] += skill.suppAmnt;
            if (effect[0] > 2)
                effect[0] = 2;
        }

        else if (!skill.buff)
        {
            effect[0] -= skill.suppAmnt;
            if (effect[0] < -2)
                effect[0] = -2;
        }

        effect[1] = 3 + caster.passives[18];
    }

    private void CalculateCharge(SupportSkill skill, ActorStats recipient)
    {

        recipient.charge = skill.charge[0] + 1;
    }

    private void RemoveCharge(AttackSkill skill, ActorStats recipient)
    {
        if ((skill.physical && recipient.charge == 1) || (!skill.physical && recipient.charge == 2) || recipient.charge == 3 || recipient.charge == 4)
            recipient.charge = 0;
    }

    private void RemoveCharge(RecoverySkill skill, ActorStats recipient)
    {
        if (skill.skillID >= AILMENT_ID && skill.skillID < RECOVERY_ID && recipient.charge == 5)
            recipient.charge = 0;
    }

    private void CalculateProtection(SupportSkill skill, ActorStats recipient)
    {
        recipient.protective = skill.block[0] + 1;
    }

    private void ExpireProtection(Team team)
    {
        var player = team.player.GetComponent<ActorStats>();
        player.protective = 0;
        player.damageDown = 0;

        foreach (GameObject obj in team.activeDemons)
        {
            var demon = obj.GetComponent<ActorStats>();
            demon.protective = 0;
            demon.damageDown = 0;
        }
    }

    private void BreakProtection(AttackSkill skill, ActorStats demon)
    {
        if ((demon.protective == skill.type + 3) || (demon.protective == 1 && skill.type == 0) || (demon.protective == 2 && skill.type > 0 && skill.type < 7))
            demon.protective = 0;

        else if (demon.ailment[0] == 5)
            demon.ailment = new List<int>{ 0, 0, 0 };
    }

    private void CalculateDamageDown(ActorStats demon)
    {
        demon.damageDown = 1;
    }

    private void CalculateTaunt(ActorStats demon)
    {
        demon.taunt = new List<int>{ 1, 3 };
    }

    public void FocusOnActive()
    {
        //var team = playerTeam.GetComponent<Team>();
        //var opposing = 0;
        //if (!team.homeTeam)
        //    opposing = 1;

        //cam.transform.position = new Vector3(
        //    active.transform.position.x,
        //    active.transform.position.y + (active.GetComponent<MeshCollider>().bounds.size.y * 50),
        //    active.transform.position.z + (active.GetComponent<MeshCollider>().bounds.size.z * 50) + 3f - (6f * opposing));

        //if (team.activeDemons.Contains(active))
        //{
        //    cam.transform.eulerAngles = new Vector3(
        //        cam.transform.rotation.x,
        //        (220f - (40f * team.activeDemons.IndexOf(active))) + (180f * opposing),
        //        cam.transform.rotation.z);
        //}
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
        flavorText.GetComponent<Text>().text = active.GetComponent<ActorStats>().stats.name + " Is Guarding!";
        active.GetComponent<Animator>().SetTrigger("guardStart");
        active.GetComponent<ActorStats>().guard = true;
        StartCoroutine(Delay(1));
    }

    public void GuardEnd()
    {
        active.GetComponent<Animator>().SetTrigger("guardEnd");
        active.GetComponent<ActorStats>().guard = false;
    }

    IEnumerator SwitchDemon(int val)
    {   
        var team = playerTeam.GetComponent<Team>();

        // Don't perform on team switch (code 5)
        if (val != 5)
        {
            active = SetNextDemonActive(team);
            while(active == null || active.GetComponent<ActorStats>().stats.battleStats.hp <= 0)
                active = SetNextDemonActive(team);
        }

        var demon = active.GetComponent<ActorStats>();
        bool switchTeam = UpdatePress(val);

        if (switchTeam)
            yield break;

        // Execute if the current demon is afflicted with ailments
        if (demon.ailment[0] != 0) {
            if (UnityEngine.Random.Range(0f, 100f) <= demon.ailment[1] * 25) {
                flavorText.GetComponent<Text>().text = demon.stats.name + " Recoved From Its Status Condition!";
                demon.ailment = new List<int>{ 0, 0, 0 };

                yield return new WaitForSeconds(3.0f);
            }
        }

        NextUp(val);
    }

    public void NextUp(int val)
    {
        // On game over...
        if (val == 4)
            return;

        var demon = active.GetComponent<ActorStats>();

        if (demon.guard)
        {
            GuardEnd();
        }
        
        // If asleep...
        if (demon.ailment[0] == 5) {
            flavorText.GetComponent<Text>().text = demon.stats.name + " Is Sleeping...";
            StartCoroutine(Delay(1));

            ++demon.ailment[1];
            return;
        }

        // If confused or charmed...
        else if ((demon.ailment[0] == 2 || demon.ailment[0] == 3) && (UnityEngine.Random.Range(0f, 100f) < SIDE_EFFECT_RATE)) {
            string statusText;
            if (demon.ailment[0] == 3)
                statusText = "Charm";
            else
                statusText= "Confusion";
            flavorText.GetComponent<Text>().text = demon.stats.name + " Is Immobilized By " + statusText + "...";
            StartCoroutine(Delay(1));

            ++demon.ailment[1];
            return;
        }

        else  
            UpdateName();

        if (!playerTeam.GetComponent<Team>().ai)
            mainScreen.enabled = true;
        else
            AITurn();

        FocusOnActive();
    }

    public GameObject SetNextDemonActive(Team team)
    {
        activeIndex = (++activeIndex) % 4;
        switch (activeIndex)
        {
            case 0:
                return team.activeDemons[0];
            case 1:
                return team.activeDemons[1];
            case 2:
                return team.activeDemons[2];
            default:
                return team.player;
        }
    }

    public void ChangeDemons(GameObject oldDemon, GameObject newDemon)
    {
        if (oldDemon.GetComponent<ActorStats>().stats.battleStats.hp > 0)
            flavorText.GetComponent<Text>().text = oldDemon.GetComponent<ActorStats>().stats.name + " Swapped With " + newDemon.GetComponent<ActorStats>().stats.name + "!";
        else   
            flavorText.GetComponent<Text>().text = newDemon.GetComponent<ActorStats>().stats.name + " Was Summoned!";

        var team = GetComponent<GameManager>().playerTeam.GetComponent<Team>();
        var tempPos = oldDemon.transform.position;
        var tempRot = oldDemon.transform.rotation;
        var tempIndex = team.activeDemons.IndexOf(oldDemon);
        newDemon.SetActive(true);

        // If oldDemon is in active party, perform swap
        if (team.activeDemons.Contains(newDemon))
        {
            //Debug.Log("OLD DEMON: " + oldDemon.GetComponent<ActorStats>().stats.name.ToUpper() + " | OLD INDEX: " + team.activeDemons.IndexOf(oldDemon));
            //Debug.Log("NEW DEMON: " + newDemon.GetComponent<ActorStats>().stats.name.ToUpper() + " | OLD INDEX: " + team.activeDemons.IndexOf(newDemon));
            //Debug.Log("Before:" + team.activeDemons[team.activeDemons.IndexOf(newDemon)].GetComponent<ActorStats>().stats.name);
            team.activeDemons[team.activeDemons.IndexOf(newDemon)] = oldDemon;
            //Debug.Log("After:" + team.activeDemons[team.activeDemons.IndexOf(oldDemon)].GetComponent<ActorStats>().stats.name);
            oldDemon.transform.position = newDemon.transform.position;
            oldDemon.transform.rotation = newDemon.transform.rotation;
        }
        else
            oldDemon.SetActive(false);

        team.activeDemons[tempIndex] = newDemon;
        newDemon.transform.position = tempPos;
        newDemon.transform.rotation = tempRot;

        //Debug.Log("OLD DEMON: " + oldDemon.GetComponent<ActorStats>().stats.name.ToUpper() + " | NEW INDEX: " + team.activeDemons.IndexOf(oldDemon));
        //Debug.Log("NEW DEMON: " + newDemon.GetComponent<ActorStats>().stats.name.ToUpper() + " | NEW INDEX: " + team.activeDemons.IndexOf(newDemon));

        // Do not set new active if player is active
        if (active != team.player)
            active = newDemon;
        
        StartCoroutine(Delay(0));
    }

    public void SwitchTeams()
    {
        var temp = playerTeam;
        playerTeam = opponentTeam;
        opponentTeam = temp;
        activeIndex = 3;
        
        List<GameObject> party = AliveDemons(playerTeam.GetComponent<Team>().activeDemons, true);
        foreach (GameObject obj in party)
            ReduceTurns(obj);

        if (playerTeam.GetComponent<Team>().homeTeam)
        {
            screen.transform.Find("Turn").GetComponent<Text>().text = "Ally Turn";
            ++turn;
            screen.transform.Find("TurnNumber").GetComponent<Text>().text = turn.ToString();
        }
        else
            screen.transform.Find("Turn").GetComponent<Text>().text = "Enemy Turn";

        StartCoroutine(SwitchDelay(playerTeam.GetComponent<Team>().homeTeam));
    }

    public void AITurn()
    {
        mainScreen.enabled = false;
        playerTeam.GetComponent<AI>().AIMove();
    }

    public void ReduceTurns(GameObject obj)
    {
        ActorStats stats = obj.GetComponent<ActorStats>();
        ReduceStatTurns(stats.attack);
        ReduceStatTurns(stats.defense);
        ReduceStatTurns(stats.accEvas);
    }

    public void ReduceStatTurns(List<int> stat)
    {
        switch (stat[1])
        {
            case 0:
                break;
            case 1:
                stat[0] = 0;
                stat[1] = 0;
                break;
            default:
                --stat[1];
                break;
        }
    }

    public void CreatePartyTurns()
    {
        Transform pressTurnPane = screen.transform.Find("PressTurns");
        int turnCount = 0;

        Color glow;
        if (playerTeam.GetComponent<Team>().homeTeam)
            glow = Color.cyan;
        else
            glow = Color.red;

        // Turn always generates for player
        CreatePressTurn(0, pressTurnPane, glow);

        for (int i = 0; i < playerTeam.GetComponent<Team>().activeDemons.Count; ++i)
        {
            // Only generates turn if there's a demon at that position
            if (playerTeam.GetComponent<Team>().activeDemons[i] != null && playerTeam.GetComponent<Team>().activeDemons[i].GetComponent<ActorStats>().stats.battleStats.hp > 0)
            {
                CreatePressTurn(turnCount + 1, pressTurnPane, glow);
                ++turnCount;
            }
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

    public bool UpdatePress(int val)
    {
        Transform pressTurnPane = screen.transform.Find("PressTurns");
        switch (val)
        {
            //WEAK, PASS, CHANGE
            case 0:
                return ExtraTurn(pressTurnPane);
            //NORMAL, RESIST, GUARD
            case 1:
                return DeleteTurns(1, pressTurnPane);
            //MISS, NULL
            case 2:
                return DeleteTurns(2, pressTurnPane);
            //REPEL, DRAIN
            case 3:
                return DeleteTurns(4, pressTurnPane);
            default:
                return false;
        }
    }

    private bool DeleteTurns(int val, Transform pressTurnPane)
    {
        int paneIndex = pressTurnPane.transform.childCount - 1;

        for (int i = 0; i < val; ++i)
        {
            Destroy(pressTurnPane.transform.GetChild(paneIndex).gameObject);
            --paneIndex;
            if (paneIndex < 0)
                break;
        }
    
        if (paneIndex < 0)
        {
            flavorText.SetActive(false);
            SwitchTeams();
            CreatePartyTurns();
            active = playerTeam.GetComponent<Team>().player;
            return true;
        }

        return false;
    }

    private bool ExtraTurn(Transform pressTurnPane)
    {
        for (int i = (pressTurnPane.transform.childCount - 1); i >= 0; --i)
        {
            if (pressTurnPane.transform.GetChild(i).GetComponent<Image>().sprite == pressTurnIcon.sprite)
            {
                pressTurnPane.transform.GetChild(i).GetComponent<Image>().sprite = glowingPressTurnIcon;
                return false;
            }
        }

        return DeleteTurns(1, pressTurnPane);
    }

    public void Pass()
    {
        flavorText.GetComponent<Text>().text = active.GetComponent<ActorStats>().stats.name + " Passed Their Turn!";
        StartCoroutine(Delay(0));
    }

    void UpdateIdles()
    {

        DemonIdleUpdate(playerTeam.GetComponent<Team>().player);
        foreach(GameObject demon in playerTeam.GetComponent<Team>().activeDemons)
        {
            DemonIdleUpdate(demon);
        }

        DemonIdleUpdate(opponentTeam.GetComponent<Team>().player);
        foreach(GameObject demon in opponentTeam.GetComponent<Team>().activeDemons)
        {
            DemonIdleUpdate(demon);
        }
    }

    void DemonIdleUpdate(GameObject demon)
    {
        if (demon.GetComponent<ActorStats>().stats.battleStats.hp <= demon.GetComponent<ActorStats>().stats.baseStats.hp / 4)
        {
            demon.GetComponent<Animator>().SetBool("dying", true);
        }
        else
            demon.GetComponent<Animator>().SetBool("dying", false);
    }

    IEnumerator Delay(int val)
    {
        mainScreen.enabled = false;
        var demon = active.GetComponent<ActorStats>();
        yield return new WaitForSeconds(3.0f);

        UpdateIdles();

        if (active.GetComponent<ActorStats>().ailment[0] == 1)
        {
            double damage = Math.Floor(demon.stats.baseStats.hp / PoisonRate(demon.ailment[2]));

            flavorText.GetComponent<Text>().text = demon.stats.name + " Took Damage From The Poison...";
            demon.stats.battleStats.hp -= (int) damage;

            if (demon.stats.battleStats.hp <= 1) {
                demon.stats.battleStats.hp = 1;
                demon.ailment = new List<int>(){ 0, 0, 0 };
            }

            yield return new WaitForSeconds(3.0f);
            DemonIdleUpdate(active);
        }

        StartCoroutine(SwitchDemon(val));
    }

    IEnumerator SwitchDelay(bool playerTeam)
    {
        if (playerTeam)
        {
            switchText.GetComponent<Text>().text = "Player Turn";
            switchText.GetComponent<Text>().color = Color.cyan;
        }
        else
        {
            switchText.GetComponent<Text>().text = "Enemy Turn";
            switchText.GetComponent<Text>().color = Color.red;
        }
        switchText.SetActive(true);
        yield return new WaitForSeconds(3.0f);

        switchText.SetActive(false);
        flavorText.SetActive(true);
        StartCoroutine(SwitchDemon(5));
    }

    public void UpdateName()
    {
        actionCommand.text = "What Will " + active.GetComponent<ActorStats>().stats.name + " Do?";
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadTeam(GameObject team)
    {
        foreach (Transform child in team.transform)
        {
            Debug.Log(child.name);
            child.GetComponent<ActorStats>().LoadCharacter();
            var childStats = child.GetComponent<ActorStats>().stats;
            childStats.battleStats.hp = childStats.baseStats.hp;
            childStats.battleStats.mp = childStats.baseStats.mp;

            foreach (int skillID in childStats.baseSkills)
            {
                childStats.skills.Add(skillCompendium[skillID]);
            }

            if (child.GetSiblingIndex() >= 1 && child.GetSiblingIndex() < 4)
                team.GetComponent<Team>().activeDemons.Add(child.gameObject);
        }

        Team otherTeam;
        if (team == playerTeam)
            otherTeam = opponentTeam.GetComponent<Team>();
        else
            otherTeam = playerTeam.GetComponent<Team>();

        team.GetComponent<Team>().FillOpposingData(otherTeam);

        for (int i = team.GetComponent<Team>().activeDemons.Count; i < 3; ++i)
        {
            team.GetComponent<Team>().activeDemons.Add(null);
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(3.0f);
        mainScreen.enabled = false;
        screen.enabled = false;
        gameOverScreen.enabled = true;

        Text winText = gameOverScreen.transform.Find("WinText").GetComponent<Text>();

        Team homeTeam;
        if (playerTeam.GetComponent<Team>().homeTeam)
            homeTeam = playerTeam.GetComponent<Team>();
        else
            homeTeam = opponentTeam.GetComponent<Team>();

        if(homeTeam.player.GetComponent<ActorStats>().stats.battleStats.hp > 0)
        {
            winText.text = "Player Team Wins!";
            winText.color = Color.cyan;
        }
        else
        {
            winText.text = "Enemy Team Wins!";
            winText.color = Color.red;
        }
    }

    private void SetTeam()
    {
        DemonDex demonIndicies = demonDex.GetComponent<DemonDex>();
        Team allyTeam = playerTeam.GetComponent<Team>();
        Team enemyTeam = opponentTeam.GetComponent<Team>();

       CreateDemon(demonIndicies.MatchIndexWithDemon(PlayerPrefs.GetInt("allyPlayer")), battlePositions.transform.GetChild(0), allyTeam, true);
       for (int i = 0; i < 3; ++i)
       {
            CreateDemon(demonIndicies.MatchIndexWithDemon(PlayerPrefs.GetInt("allyTeammate" + i)), battlePositions.transform.GetChild(i + 1), allyTeam, false);
       }

       CreateDemon(demonIndicies.MatchIndexWithDemon(PlayerPrefs.GetInt("enemyPlayer")), battlePositions.transform.GetChild(4), enemyTeam, true);
       for (int i = 0; i < 3; ++i)
       {
            CreateDemon(demonIndicies.MatchIndexWithDemon(PlayerPrefs.GetInt("enemyTeammate" + i)), battlePositions.transform.GetChild(i + 5), enemyTeam, false);
       }
    }

    GameObject CreateDemon(GameObject demon, Transform battlePos, Team team, bool player)
    {
        GameObject readyDemon = Instantiate(demon, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        readyDemon.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX |RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        readyDemon.transform.SetParent(team.transform);
        readyDemon.transform.position = battlePos.position;
        readyDemon.transform.rotation = battlePos.rotation;

        if (player)
            team.player = readyDemon;
        else
            team.demons.Add(readyDemon);

        return demon;
    }

    // Start is called before the first frame update
    void Start()
    {
        ReadCompendiums();

        SetTeam();
        LoadTeam(playerTeam);
        LoadTeam(opponentTeam);

        active = playerTeam.GetComponent<Team>().player;

        UpdateName();
        CreatePartyTurns();
        FocusOnActive();
    }
}
