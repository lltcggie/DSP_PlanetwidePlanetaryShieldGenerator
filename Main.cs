using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace PlanetaryShieldGenerator
{
    [BepInPlugin("lltcggie.DSP.plugin.PlanetwidePlanetaryShieldGenerator", "PlanetwidePlanetaryShieldGenerator", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        public void Awake()
        {
            LogManager.Logger = Logger;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    public class LogManager
    {
        public static ManualLogSource Logger;
    }
}
