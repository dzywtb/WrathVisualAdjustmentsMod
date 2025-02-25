﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TutorialCanvas.Utilities
{
    public static class BundleManger
    {
        private static Dictionary<string, GameObject> _objects = new Dictionary<string, GameObject>();
        private static Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

        public static void RemoveBundle(string loadAss, bool unloadAll = false)
        {
            AssetBundle bundle;
            if (bundle = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.name == loadAss))
                bundle.Unload(unloadAll);
            if (unloadAll)
            {
                _objects.Clear();
                _sprites.Clear();
            }
        }

        public static void AddBundle(string loadAss)
        {
            try
            {
                AssetBundle bundle;
                GameObject prefab;
                Sprite sprite;

                RemoveBundle(loadAss, true);

                bundle = AssetBundle.LoadFromFile(VisualAdjustments.Main.ModEntry.Path + loadAss);
                if (!bundle) throw new Exception($"Failed to load AssetBundle! {VisualAdjustments.Main.ModEntry.Path + loadAss}");
                foreach (string b in bundle.GetAllAssetNames())
                {
                    if (b.EndsWith(".prefab"))
                    {
                        VisualAdjustments.Main.logger.Log($"Loading prefab: {b}");
                        if (!_objects.ContainsKey(Path.GetFileNameWithoutExtension(b)))
                        {
                            if ((prefab = bundle.LoadAsset<GameObject>(b)) != null)
                            {
                                prefab.SetActive(false);
                                _objects.Add(prefab.name, prefab);
                            }
                            else
                                VisualAdjustments.Main.logger.Error($"Failed to load the prefab: {b}");
                        }
                        else
                            VisualAdjustments.Main.logger.Error($"Asset {b} already loaded.");
                    }
                    if (b.EndsWith(".png"))
                    {
                        VisualAdjustments.Main.logger.Log($"Loading sprite: {b}");
                        if (!_sprites.ContainsKey(Path.GetFileNameWithoutExtension(b)))
                        {
                            if ((sprite = bundle.LoadAsset<Sprite>(b)) != null)
                            {
                                _sprites.Add(sprite.name, sprite);
                            }
                            else
                                VisualAdjustments.Main.logger.Log($"Failed to load the prefab: {b}");
                        }
                        else
                            VisualAdjustments.Main.logger.Log($"Asset {b} already loaded.");
                    }
                }
                RemoveBundle(loadAss);
            }
            catch (Exception ex)
            {
                VisualAdjustments.Main.logger.Error(ex.Message + ex.StackTrace);
            }
        }

        public static bool IsLoaded(string asset)
        {
            return _objects.ContainsKey(asset);
        }

        public static Dictionary<string, GameObject> LoadedPrefabs
        { get { return _objects; } }
        public static Dictionary<string, Sprite> LoadedSprites
        { get { return _sprites; } }
    }
}