using AoTGiants.Patches;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
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

        private const string assetBundleName = "aotgiantsassets";
        internal const float DefaultSFXVolume = 0.3f;

        internal static ConfigEntry<string> configVolume;

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

            //Config initialization
            configVolume = Config.Bind("General","Volume", "25", "Volume percentage of the music");

            // Plugin startup logic
            Logger.LogInfo($"Plugin {pluginGUID} is loaded!");

            //Get absolute path of assetbundle and try to load it
            string dllName = "AoTGiants.dll";
            string location = this.Info.Location.TrimEnd(dllName.ToCharArray());
            SoundFX = new List<AudioClip>();
            Bundle = AssetBundle.LoadFromFile(location + assetBundleName);

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

        //Not tested xd
        internal static float getVolume(string volume)
        {
            try
            {
                return (Int32.Parse(volume) / 100);
            } catch (Exception ex)
            {
                return DefaultSFXVolume;
            }
        }
    }
}
