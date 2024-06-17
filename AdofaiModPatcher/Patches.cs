using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using UnityEngine;

// TODO: Rename this namespace to your mod's name.
namespace AdofaiModPatcher
{
    /// <summary>
    /// Add all of your <see cref="HarmonyPatch"/> classes here. If you find
    /// this file getting too large, you may want to consider separating the
    /// patches into several different classes.
    /// </summary>
    internal static class Patches
    {
        /// <summary>
        /// Example patch that logs anytime the user presses a key.
        /// </summary>
    }

    //ill make a base class if i feel like it
    public static class OverlayerUpdatePatch
    {
        static string className = "Overlayer.Main";
        static string methodName = "EnsureOverlayerVersion";
        public static MethodInfo origMethod;
        public static MethodInfo patchMethod;
        public static bool Prefix()
        {
            MainClass.Logger.Log("haiiii ;3");
            return false;
        }

        public static bool AttemptInit() {
            try
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                //Provide the current application domain evidence for the assembly.
                Evidence asEvidence = currentDomain.Evidence;
                //Load the assembly from the application directory using a simple name.


                //Make an array for the list of assemblies.
                Assembly[] assems = currentDomain.GetAssemblies();

                Type overlayerMain = null;
                foreach (Assembly assem in assems)
                {
                    if (assem.GetName().Name.Equals("Overlayer"))
                    { 
                        overlayerMain = assem.GetType(className);
                        break;
                    }
                     
                        
                }

                origMethod = overlayerMain.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                patchMethod = typeof(OverlayerUpdatePatch).GetMethod(nameof(Prefix));
                return true;
            }
            catch (Exception e) {
                MainClass.Logger.LogException(e);
                return false;
            };
        }
        
        
    }
}
