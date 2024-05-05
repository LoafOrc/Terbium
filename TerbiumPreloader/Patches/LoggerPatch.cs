using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

namespace Terbium.Patches;

[HarmonyPatch(typeof(BepInEx.Logging.Logger))]
static class LoggerPatch {
	[HarmonyPrefix, HarmonyPatch("LogMessage")]
	static void ReenableAndSaveConfigs(object data) {
		if (data is not "Chainloader startup complete") return; // this is icky, but patching Chainloader.Start just borks it lmao.
		TerbiumPreloader.Logger.LogInfo("Chainloader is done! Saving all configs and re-enabling saving!");
		ConfigFilePatch.SavingAllowed = true;
		ConfigFilePatch.SaveAll();
		
		TerbiumPreloader.ChainloaderTimer.Stop();
		TerbiumPreloader.Logger.LogInfo($"Chainloader took {TerbiumPreloader.ChainloaderTimer.ElapsedMilliseconds}ms to load.");
	}
}