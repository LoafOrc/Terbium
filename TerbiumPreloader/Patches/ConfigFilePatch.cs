using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Terbium.Util;

namespace Terbium.Patches;

[HarmonyPatch(typeof(ConfigFile))]
static class ConfigFilePatch {
	static List<ConfigFile> Configs = [];
	internal static bool SavingAllowed = false;
	internal static int SavesBlocked = 0;
	
	[HarmonyPrefix, HarmonyPatch(MethodType.Constructor, [typeof(string), typeof(bool), typeof(BepInPlugin)])]
	static void DisableSaveOnConfigSet(ConfigFile __instance) {
		Configs.Add(__instance);
	}

	[HarmonyPrefix, HarmonyPatch(nameof(ConfigFile.Save))]
	static bool LogSaving(ConfigFile __instance) {
		if (!SavingAllowed) {
			SavesBlocked++;
		}

		return SavingAllowed;
	}

	internal static void SaveAll() {
		TerbiumPreloader.Logger.LogWarning("saving ALL configs, lag spike probably!");
		Stopwatch timer = Stopwatch.StartNew();
		if (!SavingAllowed) {
			TerbiumPreloader.Logger.LogError("WOAH, Saving was disabled but .SaveAll() was called! Forcing it to TRUE.");
			SavingAllowed = true;
		}
		foreach (ConfigFile config in Configs) {
			TerbiumPreloader.Logger.LogInfo($"Saving config: {config.GetName()}");
			config.Save();
		}
		timer.Stop();
		TerbiumPreloader.Logger.LogInfo($"===============");
		TerbiumPreloader.Logger.LogInfo($"Took {timer.ElapsedMilliseconds}ms to save {Configs.Count} config(s).");
		TerbiumPreloader.Logger.LogInfo($"Blocked {SavesBlocked} un-necessary saves.");
	}
}