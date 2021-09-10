﻿using System;
using TowerGenerator.ChunkImporter;
using UnityEngine;


namespace TowerGenerator.FbxCommands
{
    // see all commands: https://docs.google.com/spreadsheets/d/1sefKKZGdllpTpHPTX2AZMRrZCT5wT7QiN8GhPCoiGW4/edit#gid=0
    public abstract class FbxCommandBase
    {
        protected const int PriorityHighest = 0;
        protected const int PriorityLowest = Int32.MaxValue;
        public string RawInputFromFbx;
        private readonly string _fbxCommandName;

        protected FbxCommandBase(string fbxCommandName)
        {
            _fbxCommandName = fbxCommandName;
        }

        public string GetFbxCommandName()
        {
            return _fbxCommandName;
        }

        public abstract void ParseParameters(string parameters, GameObject gameObject);
        public abstract void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation);
        public virtual int GetExecutionPriority()
        {
            return PriorityHighest;
        }

        public void SetRawInputFromFbx(string fbxCmdName, string fbxParameters)
        {
            RawInputFromFbx = $"{fbxCmdName}({fbxParameters})";
        }

        #region Conversion helpers
        protected static float ConvertFloat01(string float01String)
        {
            float value = float.Parse(float01String);

            var clamped = Mathf.Clamp(value, 0.0f, 1.0f);
            if (Math.Abs(clamped - value) > 0.00001f)
                Debug.LogWarning($"Clamping happened {value} -> {clamped}");
            return clamped;
        }

        protected static int ConvertInt(string intString)
        {
            int value = Int32.Parse(intString);
            return value;
        }

        protected static uint ConvertUInt(string intString)
        {
            uint value = UInt32.Parse(intString);
            return value;
        }

        protected static bool ConvertBool(string boolString)
        {
            bool value = bool.Parse(boolString);
            return value;
        }

        protected T ConvertEnum<T>(string enumString) where T : struct
        {
            try
            {
                enumString = enumString.Replace('|', ',');
                T res = (T)Enum.Parse(typeof(T), enumString);
                if (!Enum.IsDefined(typeof(T), res))
                    return default(T);
                return res;
            }
            catch
            {
                return default(T);
            }
        }
        #endregion
    }
}
