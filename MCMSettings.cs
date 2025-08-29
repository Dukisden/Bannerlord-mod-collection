using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace DukisCollection
{
    public class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        public override string Id => "DukisCollection_v1";
        public override string DisplayName => "Duki's Collection";
        public override string FolderName => "DukisCollection";
        public override string FormatType => "json2";

        [SettingPropertyBool("Enable Bleed", Order = 0, IsToggle = true, RequireRestart = false, HintText = "When enabled, strikes will have a chance to cause bleed over time. Blades cause more bleed. Hits to the head/torso cause more bleed. High damage hits cause more bleed. Healthy and armored units are less likely to bleed.")]
        [SettingPropertyGroup("Bleed")]
        public bool EnableBleed { get; set; } = true;

        [SettingPropertyBool("Proc info debug", Order = 0, RequireRestart = false, HintText = "Enable to display bleed proc chance detail on hit")]
        [SettingPropertyGroup("Bleed")]
        public bool BleedDebug { get; set; } = false;


        [SettingPropertyBool("Enable Partial Tourney Reward", Order = 0, IsToggle = true, RequireRestart = false, HintText = "When enabled, losing a tourney will still reward some gold based on round reached.")]
        [SettingPropertyGroup("Partial Tourney Reward")]
        public bool EnableTourney { get; set; } = true;


        [SettingPropertyBool("Enable Damage mods", Order = 0, IsToggle = true, RequireRestart = false, HintText = "When enabled, blunt damage is less affected by armor. Cut damage is more affected by armor. Axes do more damage to shields.")]
        [SettingPropertyGroup("Type Damage Modifiers")]
        public bool EnableDamage { get; set; } = true;


        [SettingPropertyBool("Enable Preserve Companions", Order = 0, IsToggle = true, RequireRestart = false, HintText = "When enabled, there will be a prompt on companion death to preserve them as a dead member of your clan.")]
        [SettingPropertyGroup("Preserve Companions")]
        public bool EnableCompanions { get; set; } = true;


        [SettingPropertyBool("Enable More Deaths", Order = 1, IsToggle = true, RequireRestart = false, HintText = "When enabled the player, lords and companions are more likely to die on the battlefield.")]
        [SettingPropertyGroup("More Hero Deaths")]
        public bool EnableDeath { get; set; } = true;

        [SettingPropertyFloatingInteger("Death chance multiplier", 0, 100, Order = 1, RequireRestart = false, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die. Applies to Player, Companions & Lords.")]
        [SettingPropertyGroup("More Hero Deaths", GroupOrder = 1)]
        public float DeathFactor { get; set; } = 5;

        [SettingPropertyFloatingInteger("Death chance info debug", 0, 100, Order = 9, RequireRestart = false, HintText = "Enable to display death chance info on hero death")]
        [SettingPropertyGroup("More Hero Deaths", GroupOrder = 1)]
        public bool DeathDebug { get; set; } = false;

    }
}
