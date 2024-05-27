using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Terbium.Util;

namespace Terbium.Patches;

[HarmonyPatch(typeof(ConfigFile))]
static class ConfigFilePatch {
	static readonly List<ConfigFile> Configs = [];
	internal static bool SavingAllowed = false;
	static int SavesBlocked = 0;
	
	[HarmonyPrefix, HarmonyPatch(MethodType.Constructor, [typeof(string), typeof(bool), typeof(BepInPlugin)])]
	static void RegisterConfig(ConfigFile __instance) {
		Configs.Add(__instance);
	}

	internal static void ApplyRemoveSaveOnBindPatch(Harmony harmony) {
		TerbiumPreloader.Logger.LogInfo("doing silly stuff to patch ConfigFile.Bind :3");
		MethodInfo patchMethod = typeof(ConfigFilePatch).GetMethod(nameof(RemoveSaveOnBind));
		foreach (MethodInfo method in typeof(ConfigFile).GetMethods()) {
			if(method.Name != nameof(ConfigFile.Bind) || !method.IsGenericMethod) continue;

			harmony.Patch(method.MakeGenericMethod(typeof(float)), transpiler: new HarmonyMethod(patchMethod));
		}
	}
	
	public static IEnumerable<CodeInstruction> RemoveSaveOnBind(IEnumerable<CodeInstruction> instructions) {
		TerbiumPreloader.Logger.LogDebug("Transpiler was fired!");
		CodeMatcher matcher = new CodeMatcher(instructions)
			.MatchForward(false,
				new CodeMatch(OpCodes.Ldarg_0),
				new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ConfigFile), nameof(ConfigFile.Save)))
			);
		
		TerbiumPreloader.Logger.LogDebug("IsValid? " + matcher.IsValid);

		if (matcher.IsValid) {
			TerbiumPreloader.Logger.LogInfo("Patching the correct ConfigFile.Bind!");
			matcher = matcher
				.Advance(1)
				.Set(OpCodes.Call, AccessTools.Method(typeof(ConfigFilePatch), nameof(Save)));
		}
			
		return matcher
			.InstructionEnumeration();
	}

	internal static void Save(ConfigFile instance) {
		if(!instance.SaveOnConfigSet) return; // modder has manually disabled saving, we don't want to do anything with that
		if (SavingAllowed) {
			instance.Save();
		} else {
			SavesBlocked++;
		}
	}
	
	internal static void SaveAll() {
		TerbiumPreloader.Logger.LogWarning("saving ALL configs, lag spike probably!");
		Stopwatch timer = Stopwatch.StartNew();
		if (!SavingAllowed) {
			TerbiumPreloader.Logger.LogError("WOAH, Saving was disabled but .SaveAll() was called! Forcing it to TRUE.");
			SavingAllowed = true;
		}
		foreach (ConfigFile config in Configs) {
			if(config.Count == 0) continue;
			TerbiumPreloader.Logger.LogInfo($"Saving config: {config.GetName()}");
			config.Save();
		}
		timer.Stop();
		TerbiumPreloader.Logger.LogInfo($"===============");
		TerbiumPreloader.Logger.LogInfo($"Took {timer.ElapsedMilliseconds}ms to save {Configs.Count} config(s).");
		TerbiumPreloader.Logger.LogInfo($"Blocked {SavesBlocked} un-necessary saves.");
	}
}