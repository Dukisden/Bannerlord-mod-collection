using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using System;

namespace DukisCollection
{
    public class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        public override string Id => "DukisCollection_v1";
        public override string DisplayName => "Duki's Collection";
        public override string FolderName => "DukisCollection";
        public override string FormatType => "json2";

        // Bleed
        [SettingPropertyBool("Enable Bleed", Order = 0, IsToggle = true, RequireRestart = false, HintText = "When enabled, strikes will have a chance to cause bleed over time. Blades cause more bleed. Hits to the head/torso cause more bleed. High damage hits cause more bleed. Healthy and armored units are less likely to bleed.")]
        [SettingPropertyGroup("Bleed")]
        public bool EnableBleed { get; set; } = false;

        [SettingPropertyBool("Proc info debug", Order = 0, RequireRestart = false, HintText = "Enable to display bleed proc chance detail on hit")]
        [SettingPropertyGroup("Bleed")]
        public bool BleedDebug { get; set; } = false;

        // Tourney
        [SettingPropertyBool("Enable Partial Tourney Reward", Order = 0, IsToggle = true, RequireRestart = false, HintText = "When enabled, losing a tourney will still reward some gold based on round reached.")]
        [SettingPropertyGroup("Partial Tourney Reward")]
        public bool EnableTourney { get; set; } = false;

        // Damage
            // Amplify armor effect
            [SettingPropertyBool("Enable Amplify Armor Effect", Order = 0, IsToggle = true, RequireRestart = false, HintText = "When enabled, blunt damage is less affected by armor. Cut damage is more affected by armor. Axes do more damage to shields.")]
            [SettingPropertyGroup("Damage Modifiers/Amplify armor effect")]
            public bool EnableArmorAmplify { get; set; } = false;

                [SettingPropertyInteger("Extra armor efficacy vs Cut", 0, 200, "0'%'", Order = 1, RequireRestart = false, HintText = "0% = no change, 100% = armor absorbs twice as much")]
                [SettingPropertyGroup("Damage Modifiers/Amplify armor effect")]
                public int CutMult { get; set; } = 20;

                [SettingPropertyInteger("Reduced armor efficacy vs Blunt", 0, 100, "0'%'", Order = 2, RequireRestart = false, HintText = "0% = no change, 100% = armor has no effect vs blunt damage")]
                [SettingPropertyGroup("Damage Modifiers/Amplify armor effect")]
                public int BluntMult { get; set; } = 20;

                [SettingPropertyInteger("Extra Axe damage vs Shields", 0, 200, "0'%'", Order = 3, RequireRestart = false, HintText = "0% = no change, 100% = Shields take 2 times more damage from axes")]
                [SettingPropertyGroup("Damage Modifiers/Amplify armor effect")]
                public int ShieldMult { get; set; } = 75;

            // Damage multipliers
            [SettingPropertyBool("Enable Damage Multipliers", Order = 0, IsToggle = true, RequireRestart = false)]
            [SettingPropertyGroup("Damage Modifiers/Damage Received Multipliers")]
            public bool EnableDamageMults { get; set; } = false;

                [SettingPropertyInteger("Player", 0, 200, "0'%'", Order = 10, RequireRestart = false, HintText = "Adjusts the damage received by the player. 100% = no change")]
                [SettingPropertyGroup("Damage Modifiers/Damage Received Multipliers")]
                public int DamageMultiplierPlayer { get; set; } = 100;

                [SettingPropertyInteger("Player Family", 0, 200, "0'%'", Order = 20, RequireRestart = false, HintText = "Adjusts the damage received by the player’s clan members. 100% = no change")]
                [SettingPropertyGroup("Damage Modifiers/Damage Received Multipliers")]
                public int DamageMultiplierFamily { get; set; } = 100;

                [SettingPropertyInteger("Player Clan", 0, 200, "0'%'", Order = 30, RequireRestart = false, HintText = "Adjusts the damage received by the player’s clan members. 100% = no change")]
                [SettingPropertyGroup("Damage Modifiers/Damage Received Multipliers")]
                public int DamageMultiplierClan { get; set; } = 100;

                [SettingPropertyInteger("Player Troops", 0, 200, "0'%'", Order = 40, RequireRestart = false, HintText = "Adjusts the damage received by the player’s regular troops. 100% = no change")]
                [SettingPropertyGroup("Damage Modifiers/Damage Received Multipliers")]
                public int DamageMultiplierPlayerTroops { get; set; } = 100;

                [SettingPropertyInteger("AI Lords", 0, 200, "0'%'", Order = 50, RequireRestart = false, HintText = "Adjusts the damage received by AI lords. 100% = no change")]
                [SettingPropertyGroup("Damage Modifiers/Damage Received Multipliers")]
                public int DamageMultiplierAILords { get; set; } = 100;

                [SettingPropertyInteger("AI Troops", 0, 200, "0'%'", Order = 60, RequireRestart = false, HintText = "Adjusts the damage received by AI troops. 100% = no change")]
                [SettingPropertyGroup("Damage Modifiers/Damage Received Multipliers")]
                public int DamageMultiplierAITroops { get; set; } = 100;

                [SettingPropertyInteger("Bodyguards", 0, 100, "0'%'", Order = 70, RequireRestart = false, HintText = "Damage received by bodyguards (for Ai lords & player with bodyguard mod.) Applies on top of other applicable multipliers.")]
                [SettingPropertyGroup("Damage Modifiers/Damage Received Multipliers")]
                public int DamageMultiplierBodyguard { get; set; } = 50;

                [SettingPropertyInteger("Player Formation", 0, 100, "0'%'", Order = 80, RequireRestart = false, HintText = "Damage received by your heroes (not player) when their formation has over 15 units left. Applies on top of other applicable multipliers.")]
                [SettingPropertyGroup("Damage Modifiers/Damage Received Multipliers")]
                public int DamageMultiplierFormation { get; set; } = 50;

                [SettingPropertyInteger("Ai Formation", 0, 100, "0'%'", Order = 90, RequireRestart = false, HintText = "Damage received by Ai heroes when their formation has over 15 units left. Applies on top of other applicable multipliers.")]
                [SettingPropertyGroup("Damage Modifiers/Damage Received Multipliers")]
                public int DamageMultiplierAiFormation { get; set; } = 100;

        // Death
        [SettingPropertyBool("Enable More Deaths", Order = 0, IsToggle = true, RequireRestart = false, HintText = "When enabled the player, lords and companions are more likely to die on the battlefield.")]
        [SettingPropertyGroup("More Hero Deaths")]
        public bool EnableDeath { get; set; } = false;

            [SettingPropertyFloatingInteger("Player death chance", 0f, 100f, "0.0x", Order = 10, RequireRestart = false, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die.")]
            [SettingPropertyGroup("More Hero Deaths", GroupOrder = 1)]
            public float DeathFactorPlayer { get; set; } = 5f;

            [SettingPropertyFloatingInteger("Player Family death chance", 0f, 100f, "0.0x", Order = 20, RequireRestart = false, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die.")]
            [SettingPropertyGroup("More Hero Deaths", GroupOrder = 1)]
            public float DeathFactorFamily { get; set; } = 3f;

            [SettingPropertyFloatingInteger("Player Clan death chance", 0f, 100f, "0.0x", Order = 30, RequireRestart = false, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die.")]
            [SettingPropertyGroup("More Hero Deaths", GroupOrder = 1)]
            public float DeathFactorClan { get; set; } = 5f;

            [SettingPropertyFloatingInteger("Player Kingdom death chance", 0f, 100f, "0.0x", Order = 40, RequireRestart = false, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die.")]
            [SettingPropertyGroup("More Hero Deaths", GroupOrder = 1)]
            public float DeathFactorKingdom { get; set; } = 5f;

            [SettingPropertyFloatingInteger("Ai death chance", 0f, 100f, "0.0x", Order = 50, RequireRestart = false, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die.")]
            [SettingPropertyGroup("More Hero Deaths", GroupOrder = 1)]
            public float DeathFactorAi { get; set; } = 5f;

            [SettingPropertyFloatingInteger("Player Formation death chance", 0f, 1f, "0.0x", Order = 60, RequireRestart = false, HintText = "Extra death chance multiplier if the hero's formation has more than 15 alive units (does not apply to player.) 0 = never die, 1 = no change")]
            [SettingPropertyGroup("More Hero Deaths", GroupOrder = 1)]
            public float DeathFactorFormation { get; set; } = 0.5f;

            [SettingPropertyBool("Death chance info debug", Order = 99, RequireRestart = false, HintText = "Enable to display death chance info on hero wound")]
            [SettingPropertyGroup("More Hero Deaths", GroupOrder = 1)]
            public bool DeathDebug { get; set; } = false;

        // Companions
        [SettingPropertyBool("Enable Preserve Companions", Order = 1, IsToggle = true, RequireRestart = false, HintText = "When enabled, there will be a prompt on companion death to preserve them as a dead member of your clan.")]
        [SettingPropertyGroup("Preserve Companions")]
        public bool EnableCompanions { get; set; } = true;

        // Toolbox
        [SettingPropertyGroup("Toolbox", GroupOrder = 99)]
        [SettingPropertyButton("Force assault on player siege", Order = 0, RequireRestart = false, HintText = "When the player is defending or attacking a settlement, force the besieging AI to launch the assault.", Content = "Force Assault")]
        public Action ForceAssault { get; set; } = delegate ()
        {
            MCMSettings.Instance.ForceAssaultToggle = true;
        };

        public bool ForceAssaultToggle { get; set; } = false;

    }
}
