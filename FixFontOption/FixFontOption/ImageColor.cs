using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI;
using HarmonyLib;

namespace FixFontOption
{
    internal class ImageColor
    {
        private static IModHelper? Helper;
        private static IMonitor? Monitor;
        private static bool Debug = false;
        private static bool ColorChange = false;
        private static List<FixColor>? Colors;
        public static Dictionary<int, string> DebugBuffer { get; private set; } = new Dictionary<int, string>();
        public ImageColor(IModHelper? helper = null, IMonitor? monitor = null)
        {
            Helper = helper;
            Monitor = monitor;
        }
        public void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(typeof(ImageColor), nameof(ImageColor.SpriteBatch_Draw_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(typeof(ImageColor), nameof(ImageColor.SpriteBatch_Draw_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(typeof(ImageColor), nameof(ImageColor.SpriteBatch_Draw_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color) }),
                prefix: new HarmonyMethod(typeof(ImageColor), nameof(ImageColor.SpriteBatch_Draw_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color) }),
                prefix: new HarmonyMethod(typeof(ImageColor), nameof(ImageColor.SpriteBatch_Draw_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Color) }),
                prefix: new HarmonyMethod(typeof(ImageColor), nameof(ImageColor.SpriteBatch_Draw_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Color) }),
                prefix: new HarmonyMethod(typeof(ImageColor), nameof(ImageColor.SpriteBatch_Draw_Prefix))
            );
        }
        public void SetColors(bool? enabled, List<FixColor>? colors)
        {
            if (enabled.HasValue)
            {
                ColorChange = enabled.Value;
            }
            if (colors != null)
            {
                Colors = colors;
            }
        }
        public void SetDebug(bool enabled)
        {
            Debug = enabled;
        }
        private static void Log(string message, LogLevel level = LogLevel.Trace)
        {
            Monitor?.Log(message, level);
        }
        private static Color? ChangeColorWithMask(Color color, List<byte?> target, List<byte?> change, List<byte?> mask)
        {
            if (target.Count != 4 || change.Count != 4)
            {
                return null;
            }
            Color targetColor = new(target.ElementAtOrDefault(0) ?? byte.MinValue, target.ElementAtOrDefault(1) ?? byte.MinValue, target.ElementAtOrDefault(2) ?? byte.MinValue, target.ElementAtOrDefault(3) ?? byte.MinValue);
            Color changeColor = new(change.ElementAtOrDefault(0) ?? byte.MinValue, change.ElementAtOrDefault(1) ?? byte.MinValue, change.ElementAtOrDefault(2) ?? byte.MinValue, change.ElementAtOrDefault(3) ?? byte.MinValue);
            Color maskColor = new(mask.ElementAtOrDefault(0) ?? byte.MaxValue, mask.ElementAtOrDefault(1) ?? byte.MaxValue, mask.ElementAtOrDefault(2) ?? byte.MaxValue, mask.ElementAtOrDefault(3) ?? byte.MaxValue);
            if ((color.R & maskColor.R) == (targetColor.R & maskColor.R) && (color.G & maskColor.G) == (targetColor.G & maskColor.G) && (color.B & maskColor.B) == (targetColor.B & maskColor.B) && (color.A & maskColor.A) == (targetColor.A & maskColor.A))
            {
                return new(
                    (byte)((color.R & ~maskColor.R) | (changeColor.R & maskColor.R)),
                    (byte)((color.R & ~maskColor.G) | (changeColor.G & maskColor.G)),
                    (byte)((color.R & ~maskColor.B) | (changeColor.B & maskColor.B)),
                    (byte)((color.R & ~maskColor.A) | (changeColor.A & maskColor.A))
                );
            }
            else
            {
                return null;
            }
        }
        private static bool SpriteBatch_Draw_Prefix(Game1 __instance, ref Color color)
        {
            try
            {
                if (Debug)
                {
                    int key = HashCode.Combine(nameof(SpriteBatch_Draw_Prefix), color);
                    string value = $"{nameof(SpriteBatch_Draw_Prefix)}:{color}";
                    if (DebugBuffer != null && !DebugBuffer.ContainsKey(key))
                    {
                        DebugBuffer.Add(key, value);
                    }
                }
                if (ColorChange)
                {
                    foreach (FixColor fixColor in Colors ?? new List<FixColor>())
                    {
                        List<byte?> target = fixColor.Target;
                        List<byte?> change = fixColor.Change;
                        List<byte?> mask = fixColor.Mask;

                        Color? newColor = ChangeColorWithMask(color, target, change, mask);
                        if (newColor != null)
                        {
                            color = (Color)newColor;
                            break;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log($"Failed in {nameof(SpriteBatch_Draw_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
