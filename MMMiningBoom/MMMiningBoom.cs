using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MofoMojo
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "MofoMojo.MMMiningBoom";
        public const string PluginName = "MMMiningBoom";
        public const string PluginVersion = "0.0.1";
        Harmony _Harmony;
        public static Plugin Instance;
        public const string ModName = "MMMiningBoom";

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake()
        {
            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it
            Jotunn.Logger.LogInfo("MMMiningBoom has landed");

            // To learn more about Jotunn's features, go to
            // https://valheim-modding.github.io/Jotunn/tutorials/overview.html
            Instance = this;
            Settings.Init();
            PluginLoggingLevel = Settings.PluginLoggingLevel.Value;

            if(Settings.MMMiningBoomModEnabled.Value) _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

               
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose,
            Debug
        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchSelf();
        }

        public static void Log(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogError(message);
        }

        public static void LogVerbose(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.Verbose) Debug.Log(message);
        }

        public static void LogDebug(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel == LoggingLevel.Debug) Debug.Log(message);
        }

    }

    internal static class Settings
    {
        public static ConfigEntry<int> nexusId;
        public static ConfigEntry<bool> MMMiningBoomEnabled;
        public static ConfigEntry<bool> MMMiningBoomModEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
           public static ConfigEntry<KeyCode> DropModifierKey;


        public static void Init()
        {
            MMMiningBoomModEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMMiningBoomModEnabled", "MMMiningBoomModEnabled", true, "Enables MMMiningBoomEnabled mod");
            MMMiningBoomEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMMiningBoomEnabled", "MMMiningBoomEnabled", false, "Enables MMMiningBoomEnabled damage during play");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
        }

    }

    // Token: 0x02000004 RID: 4
    [HarmonyPatch]
    public static class Patches
    {
        // Token: 0x06000005 RID: 5 RVA: 0x00002090 File Offset: 0x00000290
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MineRock5), "Damage")]
        public static void MineRock5_Damage(ZSyncAnimation __instance, ZNetView ___m_nview, List<HitArea> ___m_hitAreas, EffectList ___m_destroyedEffect, HitData hit)
        {
            if(MofoMojo.Settings.MMMiningBoomEnabled.Value && hit.GetAttacker().IsPlayer())
            {
                if (Player.m_localPlayer != null)
                {
                    for (int i = 0; i < ___m_hitAreas.Count; i++)
                    {
                        hit.m_damage.m_damage = hit.m_damage.m_damage * 10f;
                        ___m_nview.InvokeRPC("Damage", new object[]
                        {
                        hit,
                        i
                        });
                    }
                }
            }

        }
    }
}



