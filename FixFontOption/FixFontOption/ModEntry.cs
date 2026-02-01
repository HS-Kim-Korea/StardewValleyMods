using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;

namespace FixFontOption
{
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
                /// <summary>The mod configuration from the player.</summary>
        private ModConfig? Config;
        private BMFontOption? BMFontOption;
        private FontColor? FontColor;
        private ImageColor? ImageColor;
        private SpriteFontOption? SpriteFontOption;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Harmony harmony = new(ModManifest.UniqueID);
            Config = helper.ReadConfig<ModConfig>();

            BMFontOption = new BMFontOption(helper, Monitor);
            SpriteFontOption = new SpriteFontOption(helper, Monitor);
            FontColor = new FontColor(helper, Monitor);
            ImageColor = new ImageColor(helper, Monitor);
            // Bitmap font options
            BMFontOption.SetDebug(Config.Debug);
            BMFontOption.SetFontPixelZoom(Config.EnableFixFontPixelZoom, Config.FontPixelZoom);
            BMFontOption.SetFontLineSpace(Config.EnableBMFontLineSpace, Config.BMFontLineSpace);
            // sprite font options
            SpriteFontOption.SetDebug(Config.Debug);
            SpriteFontOption.SetHideShadow(Config.HideShadow);
            SpriteFontOption.SetDialogueLineSpace(Config.EnableFixDialogueFontLineSpace, Config.DialogueFontLineSpace);
            SpriteFontOption.SetSmallLineSpace(Config.EnableFixSmallFontLineSpace, Config.SmallFontLineSpace);
            // font color
            FontColor.SetDebug(Config.Debug);
            FontColor.SetColors(Config.EnableFixFontColor, Config.FontColor);
            // image color
            ImageColor.SetDebug(Config.Debug);
            ImageColor.SetColors(Config.EnableFixImgColor, Config.ImgColor);
            // harmony patch
            BMFontOption.Patch(harmony);
            SpriteFontOption.Patch(harmony);
            FontColor.Patch(harmony);
            ImageColor.Patch(harmony);
            helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
        }

        private void GameLoop_OneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if(Config != null && Config.Debug)
            {
                foreach (KeyValuePair<int, string> buffer in FontColor.DebugBuffer)
                {
                    Log($"{buffer.Value}", LogLevel.Debug);
                }
                foreach (KeyValuePair<int, string> buffer in ImageColor.DebugBuffer)
                {
                    Log($"{buffer.Value}", LogLevel.Debug);
                }
                FontColor.DebugBuffer.Clear();
                ImageColor.DebugBuffer.Clear();
            }
        }
        private void Log(string message, LogLevel level = LogLevel.Trace)
        {
            Monitor?.Log(message, level);
        }
    }
}
