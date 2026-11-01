using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RecipeScrollFix
{
    /// <summary>
    /// Read by Configuration Manager via reflection (field names must match) to order / hide entries.
    /// </summary>
    internal sealed class ConfigurationManagerAttributes
    {
        public int? Order;
        public bool? IsAdvanced;
        public string DispName;
    }

    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class RecipeScrollFixPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "r4b2.RecipeScrollFix";
        public const string PluginName = "Recipe Scroll Fix";
        public const string PluginVersion = "1.0.0";

        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<float> RowsPerNotch;
        internal static ConfigEntry<bool> DebugLog;
        internal static BepInEx.Logging.ManualLogSource Log;

        // The ScrollRect that contains the crafting/upgrade recipe list (resolved in InventoryGui.Awake).
        internal static ScrollRect RecipeScroll;
        internal static InventoryGui Gui;

        private void Awake()
        {
            Log = Logger;

            Enabled = Config.Bind("1 - General", "Enabled", true,
                new ConfigDescription(
                    "Use a fixed scroll step for the crafting / upgrade recipe list. Turn off to get the vanilla behaviour back.",
                    null,
                    new ConfigurationManagerAttributes { Order = 3, DispName = "Enabled" }));

            RowsPerNotch = Config.Bind("1 - General", "Rows per mouse wheel notch", 2f,
                new ConfigDescription(
                    "How many recipe rows the list moves per mouse wheel notch. Changes apply immediately.",
                    new AcceptableValueRange<float>(0.25f, 10f),
                    new ConfigurationManagerAttributes { Order = 2, DispName = "Rows per mouse wheel notch" }));

            DebugLog = Config.Bind("2 - Debug", "Log scroll values", false,
                new ConfigDescription(
                    "Log the raw scroll values the game sends to the recipe list (useful for tuning / bug reports).",
                    null,
                    new ConfigurationManagerAttributes { Order = 1, IsAdvanced = true, DispName = "Log scroll values" }));

            new Harmony(PluginGuid).PatchAll();
        }
    }

    [HarmonyPatch(typeof(InventoryGui), "Awake")]
    internal static class InventoryGui_Awake_Patch
    {
        private static void Postfix(InventoryGui __instance)
        {
            RecipeScrollFixPlugin.Gui = __instance;
            RecipeScrollFixPlugin.RecipeScroll = __instance.m_recipeListRoot != null
                ? __instance.m_recipeListRoot.GetComponentInParent<ScrollRect>()
                : null;

            if (RecipeScrollFixPlugin.RecipeScroll == null)
                RecipeScrollFixPlugin.Log.LogWarning("Could not find the recipe list ScrollRect - nothing will be changed.");
            else
                RecipeScrollFixPlugin.Log.LogInfo("Recipe list ScrollRect found: sensitivity=" +
                    RecipeScrollFixPlugin.RecipeScroll.scrollSensitivity +
                    ", rowSpace=" + __instance.m_recipeListSpace);
        }
    }

    [HarmonyPatch(typeof(ScrollRect), nameof(ScrollRect.OnScroll))]
    internal static class ScrollRect_OnScroll_Patch
    {
        private static void Prefix(ScrollRect __instance, PointerEventData data)
        {
            if (!RecipeScrollFixPlugin.Enabled.Value || data == null ||
                __instance != RecipeScrollFixPlugin.RecipeScroll || RecipeScrollFixPlugin.Gui == null)
                return;

            Vector2 raw = data.scrollDelta;
            float dir = Mathf.Abs(raw.y) >= Mathf.Abs(raw.x) ? raw.y : raw.x;
            if (dir == 0f)
                return;

            // Replace whatever magnitude the input system sent (it can be +-120 per notch)
            // with a fixed number of rows per notch. OnScroll moves content by delta * scrollSensitivity pixels.
            float rowPixels = Mathf.Max(RecipeScrollFixPlugin.Gui.m_recipeListSpace, 1f);
            float wantedPixels = rowPixels * RecipeScrollFixPlugin.RowsPerNotch.Value;
            float sensitivity = Mathf.Max(__instance.scrollSensitivity, 0.0001f);

            if (RecipeScrollFixPlugin.DebugLog.Value)
                RecipeScrollFixPlugin.Log.LogInfo("raw scrollDelta=" + raw + " -> " +
                    (Mathf.Sign(dir) * wantedPixels) + "px (sens " + sensitivity + ")");

            data.scrollDelta = new Vector2(0f, Mathf.Sign(dir) * wantedPixels / sensitivity);
        }
    }
}
