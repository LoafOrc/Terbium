using System;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Terbium.Patches;

[HarmonyPatch(typeof(ManualLogSource))]
static class ManualLogSourcePatch {
	[HarmonyPrefix, HarmonyWrapSafe, HarmonyPatch(nameof(ManualLogSource.Log))]
	static void HidePersonalInfoInPaths(ref object data) {
		data = data.ToString()
				   .Replace(Path.GetTempPath(), "%TEMP%" + Path.DirectorySeparatorChar)
				   .Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
					   "%USERPROFILE%" + Path.DirectorySeparatorChar)
				   .Replace(Environment.UserName, "<user>");
	}
}