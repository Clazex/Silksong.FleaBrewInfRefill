using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

namespace FleaBrewInfRefill;

[BepInAutoPlugin(id: "dev.clazex.fleabrewinfrefill")]
public sealed partial class FleaBrewInfRefillPlugin : BaseUnityPlugin {
	public static FleaBrewInfRefillPlugin Instance { get; private set; } = null!;
	internal static new ManualLogSource Logger { get; private set; } = null!;

	private Harmony Harmony { get; } = new(Id);

	private void Awake() {
		Instance = this;
		Logger = base.Logger;
		Harmony.PatchAll(typeof(FleaBrewInfRefillPlugin));
		Logger.LogInfo($"Plugin {Name} ({Id}) v{Version} has loaded!");
	}

	private void OnDestroy() {
#if DEBUG
		Logger.LogWarning("Unloading in release build");
#endif
		Harmony.UnpatchSelf();
	}

	[HarmonyPatch(typeof(ToolItemManager), nameof(ToolItemManager.Awake))]
	[HarmonyWrapSafe]
	[HarmonyPostfix]
	private static void AllowInfRefill(ToolItemManager __instance) {
		ToolItemStatesLiquid toolFleaBrew = (ToolItemStatesLiquid) __instance.toolItems.GetByName("Flea Brew");
		toolFleaBrew.infiniteRefillsBool = nameof(PlayerData.FleaGamesStarted);
		Logger.LogDebug("Infinite flea brew refill enabled");
	}
}
