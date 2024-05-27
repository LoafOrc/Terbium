using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Terbium;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Terbium : BaseUnityPlugin {
	public static Terbium Instance { get; private set; } = null!;
	internal new static ManualLogSource Logger { get; private set; } = null!;
	internal static Harmony Harmony { get; set; }

	private void Awake() {
		Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
		Instance = this;

		Logger.LogInfo("Running Harmony patches.");
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
		
		Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID}:{MyPluginInfo.PLUGIN_VERSION} by loaforc has loaded :3");
	}
}