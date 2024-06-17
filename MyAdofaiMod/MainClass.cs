using System.Reflection;
using System.Threading;
using HarmonyLib;
using UnityModManagerNet;
using System;
using System.Reflection;
// TODO: Rename this namespace to your mod's name.
namespace MyAdofaiMod
{
    /// <summary>
    /// The main class for the mod. Call other parts of your code from this
    /// class.
    /// </summary>
    public static class MainClass
    {
        /// <summary>
        /// Whether the mod is enabled. This is useful to have as a global
        /// property in case other parts of your mod's code needs to see if the
        /// mod is enabled.
        /// </summary>
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// UMM's logger instance. Use this to write logs to the UMM settings
        /// window under the "Logs" tab.
        /// </summary>
        public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        private static Harmony harmony;


        
        private static MethodInfo GetMethodInfo(Delegate d)
        {
            return d.Method;
        }

        /// <summary>
        /// Perform any initial setup with the mod here.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for the mod.</param>
        internal static void Setup(UnityModManager.ModEntry modEntry) {
            Logger = modEntry.Logger;
            harmony = new Harmony(modEntry.Info.Id);
            HookLoadAsm();
            // Add hooks to UMM event methods
            modEntry.OnToggle = OnToggle;
        }

        /// <summary>
        /// Handler for toggling the mod on/off.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for the mod.</param>
        /// <param name="value">
        /// <c>true</c> if the mod is being toggled on, <c>false</c> if the mod
        /// is being toggled off.
        /// </param>
        /// <returns><c>true</c></returns>
        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            IsEnabled = value;
            if (value) {
                StartMod(modEntry);
            } else {
                StopMod(modEntry);
            }
            return true;
        }
        private static void HookLoadAsm()
        {
            MethodInfo loadAsmHook = typeof(MainClass).GetMethod(nameof(LoadAssemblyHook));
            MethodInfo[] infoList = typeof(Assembly).GetMethods();
            MethodInfo loadAsmOrig = typeof(Assembly).GetMethod("LoadFile", new Type[] { typeof(string) });

            Logger.Log(loadAsmOrig.ToString());
            //harmony.Patch(GetMethodInfo((Action)Assembly.Load), postfix: new HarmonyMethod(LoadAsmHook));
            //Assembly.LoadFile
            harmony.Patch(loadAsmOrig, postfix: new HarmonyMethod(loadAsmHook));
            Logger.Log(loadAsmOrig.ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());

        }
        /// <summary>
        /// Start the mod up. You can create Unity GameObjects, patch methods,
        /// etc.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for the mod.</param>

        private static void StartMod(UnityModManager.ModEntry modEntry) {
            // Patch everything in this assembly
           
        }

        public static void LoadAssemblyHook(string path)
        {
            Logger.Log("Loaded" + path + ", attempting patch...");
            ApplyManualModPatches(harmony);
            
        }

        private static void ApplyManualModPatches(Harmony harmony)
        {
            // Add other patches here if u want
            //while (!OverlayerUpdatePatch.AttemptInit()) { };
            //definitely needs refactoring if u want this to actually work with other shit
            if (!OverlayerUpdatePatch.AttemptInit())
                return;
            harmony.Patch(OverlayerUpdatePatch.origMethod, prefix: new HarmonyMethod(OverlayerUpdatePatch.patchMethod));
            Logger.Log("Patched all mods.");

        }

        /// <summary>
        /// Stop the mod by cleaning up anything that you created in
        /// <see cref="StartMod(UnityModManager.ModEntry)"/>.
        /// </summary>
        /// <param name="modEntry">UMM's mod entry for the mod.</param>
        private static void StopMod(UnityModManager.ModEntry modEntry) {
            // Unpatch everything
            harmony.UnpatchAll(modEntry.Info.Id);
        }
    }
}
