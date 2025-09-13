using DukisCollection.dk_Death;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace DukisCollection
{
    public class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        public MCMSettings() {
            ToggleSettingsBleed = false;
            ToggleSettingsArmor = false;
            ToggleSettingsDamage = false;
            ToggleSettingsDeath = false;
        }

        public override string Id => "DukisCollection_v1";
        public override string DisplayName => "Duki's Collection";
        public override string FolderName => "DukisCollection";
        public override string FormatType => "json2";

        private string _settingsVersion = "0";
        [SettingPropertyText("Config version", RequireRestart = false)]
        [SettingPropertyGroup("Debug", GroupOrder = 999)]
        public string SettingsVersion 
        { 
            get => _settingsVersion; 
            set => _settingsVersion = SubModule.SettingsVersion;
        }

        public void CheckVersion()
        {
            string settingsVersion = SubModule.SettingsVersion;
            string savedSettingsVersion = SettingsVersion;

            if (savedSettingsVersion != settingsVersion)
            {
                if (savedSettingsVersion == "0")
                {
                    if (EnableDeath)
                    {
                        Utils.Log("Duki's Collection death settings have changed since last version, verify your config.", true);
                    }
                }

                SettingsVersion = settingsVersion;
                OnPropertyChanged(nameof(SettingsVersion));
            }
        }

        // Bleed
        [SettingPropertyBool("Enable Bleed", Order = 0, IsToggle = true, RequireRestart = false)]
        [SettingPropertyGroup("Bleed")]
        public bool EnableBleed { get; set; } = false;

        [SettingPropertyBool("Show settings", Order = 0, IsToggle = true, RequireRestart = false)]
        [SettingPropertyGroup("Bleed/Show Settings")]
        public bool ToggleSettingsBleed { get; set; } = false;

            [SettingPropertyBool("Info Logging", Order = 99, HintText = "Log bleed chance calculations.", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Main", GroupOrder = -10)]
            public bool BleedDebug { get; set; } = false;

            [SettingPropertyInteger("Max Bleed Duration", 5, 60, "#s", Order = 0, HintText = "Maximum duration that a bleed effect can last. If bleed damage is lower: duration will be shorter. If bleed damage is higher: bleed will deal more damage per tick. Default: 15", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Main", GroupOrder = -10)]
            public int MaxBleedDuration { get; set; } = 15;

            [SettingPropertyInteger("Relative speed of bleeding units", 0, 100, "0'%'",Order = 1, HintText = "Units affected by bleeding will move at this % of their normal speed. Default: 70", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Main", GroupOrder = -10)]
            public int BleedSlowAmount { get; set; } = 70;

            [SettingPropertyInteger("Relative speed of low hp units", 0, 100, "0'%'", Order = 2, HintText = "Units with low health will slow down proportionally to their missing HP, down to this % of their normal speed. Default: 70", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Main", GroupOrder = -10)]
            public int SlowAmount { get; set; } = 70;

            [SettingPropertyBool("Heroes can cause bleeding", Order = 3, RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Main", GroupOrder = -10)]
            public bool canHeroesBleed { get; set; } = true;

            [SettingPropertyBool("Troops can cause bleeding", Order = 4, RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Main", GroupOrder = -10)]
            public bool canTroopBleed { get; set; } = true;

            // Base chances
            [SettingPropertyInteger("Base Chance - Player", 0, 200, "0'%'", Order = 0, HintText = "Base bleed chance against the player. Default: 20", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Base Chances")]
            public float BC_Player { get; set; } = 20f;

            [SettingPropertyInteger("Base Chance - Hero", 0, 200, "0'%'", Order = 1, HintText = "Base bleed chance against heroes. Default: 60", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Base Chances")]
            public float BC_Hero { get; set; } = 60f;

            [SettingPropertyInteger("Base Chance - Troop", 0, 200, "0'%'", Order = 2, HintText = "Base bleed chance against regular troops. Default: 100", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Base Chances")]
            public float BC_Troop { get; set; } = 100f;

            [SettingPropertyInteger("Troops proc penalty", 0, 100, "-0'%'", Order = 2, HintText = "Bleed chance penalty applied to troops. Default: 50", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Base Chances")]
            public float BC_TroopPenalty { get; set; } = 50f;

            // Core Factors
            [SettingPropertyInteger("Damage Factor", 0, 200, "0", Order = 0, HintText = "Increases bleed chance the more damage the hit does compared to victim’s health. Higher = big hits bleed more often, lower = damage dealt has less importance. Default: 50", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Scaling")]
            public float BF_Damage { get; set; } = 50f;

            [SettingPropertyInteger("Armor Factor", 0, 200, "0", Order = 1, HintText = "Reduces bleed chance when armor blocks damage. Higher = armor protects better, lower = armor matters less. Default: 20", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Scaling")]
            public float BF_Armor { get; set; } = 20f;

            [SettingPropertyInteger("Health Factor", -200, 200, "0", Order = 2, HintText = "Reduces bleed chance if the victim has high hp. Positive = healthy units less likely to bleed, negative = healthy units more likely to bleed. Default: 30", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Scaling")]
            public float BF_Health { get; set; } = 30f;

            // Body Part Multipliers
            [SettingPropertyFloatingInteger("Head Multiplier", 0f, 10f, "0.0x", Order = 0, HintText = "Final bleed multiplier for head hits. Default: 1.5", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Body Part Multipliers")]
            public float BC_Head { get; set; } = 1.5f;

            [SettingPropertyFloatingInteger("Neck Multiplier", 0f, 10f, "0.0x", Order = 1, HintText = "Final bleed multiplier for neck hits. Default: 3", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Body Part Multipliers")]
            public float BC_Neck { get; set; } = 3.0f;

            [SettingPropertyFloatingInteger("Chest Multiplier", 0f, 10f, "0.0x", Order = 2, HintText = "Final bleed multiplier for chest/abdomen hits. Default: 1", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Body Part Multipliers")]
            public float BC_Chest { get; set; } = 1.0f;

            [SettingPropertyFloatingInteger("Arms Multiplier", 0f, 10f, "0.0x", Order = 4, HintText = "Final bleed multiplier for shoulder/arm hits. Default: 0.1", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Body Part Multipliers")]
            public float BC_Arms { get; set; } = 0.1f;

            [SettingPropertyFloatingInteger("Legs Multiplier", 0f, 10f, "0.0x", Order = 5, HintText = "Final bleed multiplier for leg hits. Default: 0.3", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Body Part Multipliers")]
            public float BC_Legs { get; set; } = 0.3f;

            // Minimum chances
            [SettingPropertyInteger("Minimum Damage to trigger bleed", 0, 100, "0'dmg'", Order = 0, HintText = "Hits bellow this value will never cause bleeding. Default: 10", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Minimum Chances")]
            public float MinDamageForBleed { get; set; } = 10;

            [SettingPropertyInteger("Minimum Bleed Chance", 0, 100, "0'%'", Order = 1, HintText = "Minimun chance to bleed (before bodypart multiplier). Default: 20", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Minimum Chances")]
            public float BleedMinChance { get; set; } = 20f;

            [SettingPropertyInteger("Minimum Bleed Chance (Daggers)", 0, 100, "0'%'", Order = 2, HintText = "The absolute lowest bleed chance with a dagger (after bodypart multiplier). Default: 50", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Proc/Minimum Chances")]
            public float BleedMinChanceDagger { get; set; } = 50f;

            // Damage
            [SettingPropertyInteger("Bleed % of damage dealt", 0, 200, "0'%'", Order = 0, HintText = "Units will bleed for this % of the damage that caused bleeding. Default: 66", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Damage/Base")]
            public float BD_Base { get; set; } = 66f;

            [SettingPropertyInteger("Cut damage armor reduction", 0, 200, "0'%'", Order = 1, HintText = "Reduce bleed damage by this % of the hit's damage absorbded by armor. Default: 50", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Damage/Base")]
            public float BD_ArmorCut { get; set; } = 50f;

            [SettingPropertyInteger("Cut damage max armor reduction", 0, 100, "#", Order = 2, HintText = "Maximum bleed reduction from armor when hit with cut damage. Default: 30", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Damage/Base")]
            public float BD_ArmorFlat { get; set; } = 30f;

            [SettingPropertyInteger("Missile multiplier", 0, 200, "0'%'", Order = 3, HintText = "Bleed damage modifier for missiles. Default: 50", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Damage/Base")]
            public float BD_Missile { get; set; } = 50f;

            [SettingPropertyInteger("Pierce multiplier", 0, 200, "0'%'", Order = 4, HintText = "Bleed damage modifier for piercing damage. Default: 66", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Damage/Base")]
            public float BD_Pierce { get; set; } = 66f;

            [SettingPropertyInteger("Blunt multiplier", 0, 200, "0'%'", Order = 5, HintText = "Bleed damage modifier for blunt damage. Default: 33", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Damage/Base")]
            public float BD_Blunt { get; set; } = 33f;

            // Stack
            [SettingPropertyInteger("Flat Tick Bonus per Stack", 0, 10, "#", Order = 0, HintText = "Flat amount of extra bleed damage per tick added for each additional bleed stack. This escalates fast. Default: 1", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Damage/Stacks")]
            public int BleedStackFlat { get; set; } = 1;

            [SettingPropertyInteger("Percent of new hit damage added per Stack (%)", 0, 100, "#'%'", Order = 1, HintText = "Percentage of the new hit damage added to current bleed for each additional stack. Default: 15", RequireRestart = false)]
            [SettingPropertyGroup("Bleed/Show Settings/Damage/Stacks")]
            public int BleedStackPercent { get; set; } = 15;

        // Tourney
        [SettingPropertyBool("Enable Partial Tourney Reward", Order = 0, IsToggle = true, HintText = "When enabled, losing a tourney will still reward some gold based on the round reached.", RequireRestart = false)]
        [SettingPropertyGroup("Partial Tourney Reward")]
        public bool EnableTourney { get; set; } = false;

            [SettingPropertyInteger("Entry Fee", 0, 1000, "g", Order = 10, HintText = "Base cost for participating in a tournament. Default: 100", RequireRestart = false)]
            [SettingPropertyGroup("Partial Tourney Reward", GroupOrder = 1)]
            public int EntryFee { get; set; } = 100;

            [SettingPropertyInteger("Round reward base", 0, 500, Order = 20, HintText = "Base reward for winning rounds. Is then multiplied by round² (round1: x1, round2: x4, round3: x9). Default: 50", RequireRestart = false)]
            [SettingPropertyGroup("Partial Tourney Reward", GroupOrder = 1)]
            public int RoundRewardBase { get; set; } = 50;

        // Amplify armor effect
        [SettingPropertyBool("Amplify Armor Effect", Order = 0, IsToggle = true, HintText = "When enabled, blunt damage is less affected by armor. Cut damage is more affected by armor. Axes do more damage to shields.", RequireRestart = false)]
        [SettingPropertyGroup("Amplify armor effect")]
        public bool EnableArmorAmplify { get; set; } = false;

        [SettingPropertyBool("Show settings", Order = 0, IsToggle = true, RequireRestart = false)]
        [SettingPropertyGroup("Amplify armor effect/Show Settings")]
        public bool ToggleSettingsArmor { get; set; } = false;

            [SettingPropertyInteger("Extra armor efficacy vs Cut", 0, 200, "0'%'", Order = 1, HintText = "0% = no change, 100% = armor absorbs twice as much. Default: 20", RequireRestart = false)]
            [SettingPropertyGroup("Amplify armor effect/Show Settings")]
            public int CutMult { get; set; } = 20;

            [SettingPropertyInteger("Reduced armor efficacy vs Blunt", 0, 100, "0'%'", Order = 2, HintText = "0% = no change, 100% = armor has no effect vs blunt damage. Default: 20", RequireRestart = false)]
            [SettingPropertyGroup("Amplify armor effect/Show Settings")]
            public int BluntMult { get; set; } = 20;

            [SettingPropertyInteger("Extra Axe damage vs Shields", 0, 200, "0'%'", Order = 3, HintText = "0% = no change, 100% = Shields take 2 times more damage from axes. Default: 75", RequireRestart = false)]
            [SettingPropertyGroup("Amplify armor effect/Show Settings")]
            public int ShieldMult { get; set; } = 75;

        // Damage multipliers
        [SettingPropertyBool("Damage Received Multipliers", Order = 0, IsToggle = true, RequireRestart = false)]
        [SettingPropertyGroup("Damage Received Multipliers")]
        public bool EnableDamageMults { get; set; } = false;

        [SettingPropertyBool("Show settings", Order = 0, IsToggle = true, RequireRestart = false)]
        [SettingPropertyGroup("Damage Received Multipliers/Show Settings")]
        public bool ToggleSettingsDamage { get; set; } = false;

            [SettingPropertyInteger("Player", 0, 200, "0'%'", Order = 10, HintText = "Adjusts the damage received by the player. 100% = no change", RequireRestart = false)]
            [SettingPropertyGroup("Damage Received Multipliers/Show Settings")]
            public int DamageMultiplierPlayer { get; set; } = 100;

            [SettingPropertyInteger("Player Family", 0, 200, "0'%'", Order = 20, HintText = "Adjusts the damage received by the player’s clan members. 100% = no change", RequireRestart = false)]
            [SettingPropertyGroup("Damage Received Multipliers/Show Settings")]
            public int DamageMultiplierFamily { get; set; } = 100;

            [SettingPropertyInteger("Player Clan", 0, 200, "0'%'", Order = 30, HintText = "Adjusts the damage received by the player’s clan members. 100% = no change", RequireRestart = false)]
            [SettingPropertyGroup("Damage Received Multipliers/Show Settings")]
            public int DamageMultiplierClan { get; set; } = 100;

            [SettingPropertyInteger("Player Troops", 0, 200, "0'%'", Order = 40, HintText = "Adjusts the damage received by the player’s regular troops. 100% = no change", RequireRestart = false)]
            [SettingPropertyGroup("Damage Received Multipliers/Show Settings")]
            public int DamageMultiplierPlayerTroops { get; set; } = 100;

            [SettingPropertyInteger("AI Lords", 0, 200, "0'%'", Order = 50, HintText = "Adjusts the damage received by AI lords. 100% = no change", RequireRestart = false)]
            [SettingPropertyGroup("Damage Received Multipliers/Show Settings")]
            public int DamageMultiplierAILords { get; set; } = 100;

            [SettingPropertyInteger("AI Troops", 0, 200, "0'%'", Order = 60, HintText = "Adjusts the damage received by AI troops. 100% = no change", RequireRestart = false)]
            [SettingPropertyGroup("Damage Received Multipliers/Show Settings")]
            public int DamageMultiplierAITroops { get; set; } = 100;

            [SettingPropertyInteger("Bodyguards", 0, 100, "0'%'", Order = 70, HintText = "Damage received by bodyguards (for Ai lords & player with bodyguard mod.) Applies on top of other applicable multipliers.", RequireRestart = false)]
            [SettingPropertyGroup("Damage Received Multipliers/Show Settings")]
            public int DamageMultiplierBodyguard { get; set; } = 50;

            [SettingPropertyInteger("Player Formation", 0, 100, "0'%'", Order = 80, HintText = "Damage received by your heroes (not player) when their formation has over 15 units left. Applies on top of other applicable multipliers.", RequireRestart = false)]
            [SettingPropertyGroup("Damage Received Multipliers/Show Settings")]
            public int DamageMultiplierFormation { get; set; } = 50;

            [SettingPropertyInteger("Ai Formation", 0, 100, "0'%'", Order = 90, HintText = "Damage received by Ai heroes when their formation has over 15 units left. Applies on top of other applicable multipliers.", RequireRestart = false)]
            [SettingPropertyGroup("Damage Received Multipliers/Show Settings")]
            public int DamageMultiplierAiFormation { get; set; } = 100;

        // Death
        [SettingPropertyBool("Enable More Deaths", Order = 0, IsToggle = true, HintText = "When enabled the player, lords and companions are more likely to die on the battlefield.", RequireRestart = false)]
        [SettingPropertyGroup("More Hero Deaths")]
        public bool EnableDeath { get; set; } = false;

        [SettingPropertyBool("Show settings", Order = 0, IsToggle = true, RequireRestart = false)]
        [SettingPropertyGroup("More Hero Deaths/Show Settings")]
        public bool ToggleSettingsDeath { get; set; } = false;

            private float _deathBias = 50;
            [SettingPropertyInteger("Hero survival bias", 0, 50, "0.#", Order = 0, HintText = "The game applies a huge survivibility bias for heros, making them very unlikely to die. You can reduce it to globally raise the death chance. Native: 50", RequireRestart = false)]
            [SettingPropertyGroup("More Hero Deaths/Show Settings", GroupOrder = 1)]
            public float DeathBias
            {
                get => _deathBias;
                set
                {
                    _deathBias = value;
                    DeathEstimate = "update";
                    OnPropertyChanged(nameof(DeathEstimate));
                }
            }

            private string _deathEstimate = "0.85%";
            [SettingPropertyText("Aproximate average death chance", Order = 1, HintText = "This is to give you an idea of the approximate average death chance for a lord, before the multipliers bellow. The real number will be higher or lower depending on the specific hero's armor, age etc.", RequireRestart = false)]
            [SettingPropertyGroup("More Hero Deaths/Show Settings", GroupOrder = 1)]
            public string DeathEstimate
            {
                get => _deathEstimate;
                set => _deathEstimate = DeathPatcher.CalculateEstimate(_deathBias);
            }

            [SettingPropertyFloatingInteger("Player death chance", 0f, 10f, "0.0x", Order = 10, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die.", RequireRestart = false)]
            [SettingPropertyGroup("More Hero Deaths/Show Settings", GroupOrder = 1)]
            public float DeathFactorPlayer { get; set; } = 5f;

            [SettingPropertyFloatingInteger("Player Family death chance", 0f, 10f, "0.0x", Order = 20, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die.", RequireRestart = false)]
            [SettingPropertyGroup("More Hero Deaths/Show Settings", GroupOrder = 1)]
            public float DeathFactorFamily { get; set; } = 3f;

            [SettingPropertyFloatingInteger("Player Clan death chance", 0f, 10f, "0.0x", Order = 30, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die.", RequireRestart = false)]
            [SettingPropertyGroup("More Hero Deaths/Show Settings", GroupOrder = 1)]
            public float DeathFactorClan { get; set; } = 5f;

            [SettingPropertyFloatingInteger("Player Kingdom death chance", 0f, 10f, "0.0x", Order = 40, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die.", RequireRestart = false)]
            [SettingPropertyGroup("More Hero Deaths/Show Settings", GroupOrder = 1)]
            public float DeathFactorKingdom { get; set; } = 5f;

            [SettingPropertyFloatingInteger("Ai death chance", 0f, 10f, "0.0x", Order = 50, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die.", RequireRestart = false)]
            [SettingPropertyGroup("More Hero Deaths/Show Settings", GroupOrder = 1)]
            public float DeathFactorAi { get; set; } = 5f;

            [SettingPropertyFloatingInteger("Player Formation death chance", 0f, 1f, "0.0x", Order = 60, HintText = "Extra death chance multiplier if the hero's formation has more than 15 alive units (does not apply to player.) 0 = never die, 1 = no change", RequireRestart = false)]
            [SettingPropertyGroup("More Hero Deaths/Show Settings", GroupOrder = 1)]
            public float DeathFactorFormation { get; set; } = 0.5f;

            [SettingPropertyBool("Death chance info debug", Order = 99, HintText = "Enable to display death chance info on hero wound", RequireRestart = false)]
            [SettingPropertyGroup("More Hero Deaths/Show Settings", GroupOrder = 1)]
            public bool DeathDebug { get; set; } = false;

        // Companions
        [SettingPropertyBool("Enable Preserve Companions", Order = 1, IsToggle = true, HintText = "When enabled, there will be a prompt on companion death to preserve them as a dead member of your clan.", RequireRestart = false)]
        [SettingPropertyGroup("Preserve Companions")]
        public bool EnableCompanions { get; set; } = true;

        // Toolbox
        [SettingPropertyGroup("Toolbox", GroupOrder = 99)]
        [SettingPropertyBool("Command all units of army (once)", Order = 10, HintText = "The next time the player is in an army and participates in battle, the player will control all units. Resets after trigerring.", RequireRestart = false)]
        public bool LeadAttackToggle { get; set; } = false;

        [SettingPropertyGroup("Toolbox", GroupOrder = 99)]
        [SettingPropertyBool("Force assault on player siege (once)", Order = 0, HintText = "When the player is defending or attacking a settlement, force the besieging AI to launch the assault. Resets after trigerring.", RequireRestart = false)]
        public bool ForceAssaultToggle { get; set; } = false;
    }
}
