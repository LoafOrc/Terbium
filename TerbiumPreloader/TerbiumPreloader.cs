using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil;

namespace Terbium;
public static class TerbiumPreloader {
	// List of assemblies to patch
	public static IEnumerable<string> TargetDLLs { get; } = new[] {"Assembly-CSharp.dll"};

	internal static Stopwatch ChainloaderTimer;
	
	internal static ManualLogSource Logger { get; } = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
	
	// Patches the assemblies
	public static void Patch(AssemblyDefinition assembly)
	{
		// Patcher code here
	}
	
	public static void Finish() {
		Logger.LogInfo("hewwo from the preloader! doing patches :3");
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);

		ChainloaderTimer = Stopwatch.StartNew();
	}
}