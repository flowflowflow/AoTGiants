using AoTGiants.Patches;
using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AoTGiants
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class AotGiantsBase : BaseUnityPlugin
    {
        private const string pluginGUID = "Flowprojects.AoTGiants";
        private const string pluginName = "AoT Giants";
        private const string pluginVersion = "1.0.0";
        private readonly Harmony harmony = new Harmony(pluginGUID);
        private static AotGiantsBase Instance;
        internal static List<AudioClip> SoundFX;
        internal static AssetBundle Bundle;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            // Plugin startup logic
            Logger.LogInfo($"Plugin {pluginGUID} is loaded!");

            //Get absolute path of assetbundle and try to load it
            string dllName = "AoTGiants.dll";
            string location = this.Info.Location.TrimEnd(dllName.ToCharArray());
            SoundFX = new List<AudioClip>();
            Bundle = AssetBundle.LoadFromFile(location + "aotgiantsassets");

            if (Bundle != null)
            {
                SoundFX = Bundle.LoadAllAssets<AudioClip>().ToList();
                Logger.LogInfo("Successfully loaded asset bundle!");
            }
            else
            {
                Logger.LogError("Failed to load assetbundle!");
            }

            //Patch method
            harmony.PatchAll(typeof(ForestGiantAIPatch));
        }
    }
}
