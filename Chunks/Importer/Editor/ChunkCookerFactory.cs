using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkCookerFactory
    {
        public class RegistrationEntry
        {
            public string Name;
            public Func<IChunkCooker> Creator;
        }

        private static readonly Dictionary<string, Func<IChunkCooker>> ChunkCookerRegistry = new();

        static ChunkCookerFactory()
        {
            RegisterChunkCooker(new RegistrationEntry
                { Name = "DefaultChunkCooker", Creator = () => new ChunkCookerDefault() });
        }

        public static void RegisterChunkCooker(RegistrationEntry entry)
        {
            if (!ChunkCookerRegistry.ContainsKey(entry.Name))
            {
                ChunkCookerRegistry[entry.Name] = entry.Creator;
            }
        }

        public static IChunkCooker CreateChunkCooker(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = "DefaultChunkCooker";
            if (ChunkCookerRegistry.TryGetValue(name, out var creator))
            {
                return creator();
            }

            Debug.LogError($"ChunkCooker with name '{name}' not found.");
            return null;
        }
    }
}