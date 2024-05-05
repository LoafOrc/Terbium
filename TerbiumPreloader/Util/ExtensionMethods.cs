using System.IO;
using BepInEx.Configuration;

namespace Terbium.Util;

public static class ExtensionMethods {
	public static string GetName(this ConfigFile file) {
		return Path.GetFileNameWithoutExtension(file.ConfigFilePath);
	}
}