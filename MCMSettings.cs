using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace DK_Collection
{
    public class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        public override string Id => "DK_Collection_v1";
        public override string DisplayName => "DK Collection";
        public override string FolderName => "DK_Collection";

        [SettingPropertyBool("Enable Bleed", Order = 0, RequireRestart = false, HintText = "When enabled, strikes will have a chance to cause bleed over time. Blades cause more bleed. Hits to the head/torso cause more bleed. High damage hits cause more bleed. Healthy and armored units are less likely to bleed.")]
        public bool EnableBleed { get; set; } = true;


        [SettingPropertyBool("Enable Partial Tourney Reward", Order = 0, RequireRestart = false, HintText = "When enabled, losing a tourney will still reward some gold based on round reached.")]
        public bool EnableTourney { get; set; } = true;


        [SettingPropertyBool("Enable Damage mods", Order = 0, RequireRestart = false, HintText = "When enabled, blunt damage is less affected by armor. Cut damage is more affected by armor. Axes do more damage to shields.")]
        public bool EnableDamage { get; set; } = true;


        [SettingPropertyBool("Enable Preserve Companions", Order = 0, RequireRestart = false, HintText = "When enabled, there will be a prompt on companion death to preserve them as a dead member of your clan.")]
        public bool EnableCompanions { get; set; } = true;


        [SettingPropertyBool("Enable More Deaths", Order = 1, RequireRestart = false, HintText = "When enabled the player, lords and companions are more likely to die on the battlefield.")]
        [SettingPropertyGroup("Death Settings")]
        public bool EnableDeath { get; set; } = true;

        [SettingPropertyInteger("Death chance multiplier", 0, 100, Order = 1, RequireRestart = false, HintText = "0 = no deaths, 1 = no change, 10 = ten times more likely to die. Applies to Player, Companions & Lords.")]
        [SettingPropertyGroup("Death Settings")]
        public float DeathFactor { get; set; } = 5;

    }
}
