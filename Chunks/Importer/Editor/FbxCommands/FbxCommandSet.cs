using System;
using System.ComponentModel;
using System.Reflection;
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Sets the value of the property for the Unity component
    // Examples:
    // Set(HoveringLocal[0], AlongVector=(0,1,0)) // [0] for support of multiple scripts with the same name
    // Set(HoveringLocal[0], Ease= InOutSine)
    public class FbxCommandSet : FbxCommandBase
    {
        public string ComponentClassName;
        public int ComponentIndex;
        public string PropertyName;
        public string PropertyValue;

        public FbxCommandSet(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
        }

        public override int GetExecutionPriority()
        {
            return PriorityLowest;
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsFalse(string.IsNullOrWhiteSpace(parameters));

            var classAndParams = parameters.Split(new[] { ',' }, 2);
            if (classAndParams[0].Contains("["))
            {
                ComponentClassName = classAndParams[0].Split('[')[0].Trim();
                ComponentIndex = ConvertInt(classAndParams[0].Split('[', ']')[1].Trim());
            }
            else
            {
                ComponentClassName = classAndParams[0];
            }

            if (classAndParams.Length == 1)
                return; // only add component

            PropertyName = classAndParams[1].Split('=')[0].Trim();
            PropertyValue = classAndParams[1].Split('=')[1].Trim();
        }

        public override void Execute(GameObject gameObject, ChunkImportState importState)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importState);

            Type compType = CustomTypeConvertor.GetType(ComponentClassName, importState.ImportSource.ResolveTypeAsmDefs);
            if (compType == null)
                throw new Exception($"No such type as '{ComponentClassName}'");

            var components = gameObject.GetComponents(compType);
            if (components == null || components.Length <= ComponentIndex) // no components with such index, add it
            {
                gameObject.AddComponent(compType);
                components = gameObject.GetComponents(compType);
            }

            if (PropertyName == null)
                return; // only add component

            var comp = components[ComponentIndex];

            var fieldInfo = compType.GetField(PropertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (fieldInfo != null)
            {
                var fType = fieldInfo.FieldType;
                object fieldValue;
                try
                {
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(fieldInfo.FieldType);
                    fieldValue = typeConverter.ConvertFromInvariantString(PropertyValue);
                }
                catch (Exception)
                {
                    fieldValue = CustomTypeConvertor.ImplicitConvert(PropertyValue, fType);
                }
                fieldInfo.SetValue(comp, fieldValue);
            }
            else
            {
                Debug.LogError($"No such property '{PropertyName}' in component '{compType.Name}'");
            }
            
            importState.Inc("SetAmount");
        }
    }
}


public static class CustomTypeConvertor
{
    public static object ImplicitConvert(string valueString, Type type)
    {
        if (type == typeof(Vector3))
            return StringToVector3(valueString);
        if (type == typeof(Vector2Int))
            return StringToVector2Int(valueString);
        throw new NotImplementedException($"{valueString} has no converter. Try to implement one: string({valueString})->{type}");
    }

    private static object StringToVector3(string v3String)
    {
        // Remove the parentheses
        if (v3String.StartsWith("(") && v3String.EndsWith(")"))
        {
            v3String = v3String.Substring(1, v3String.Length - 2);
        }

        // split the items
        string[] sArray = v3String.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
    
    private static object StringToVector2Int(string v3String)
    {
        // Remove the parentheses
        if (v3String.StartsWith("(") && v3String.EndsWith(")"))
        {
            v3String = v3String.Substring(1, v3String.Length - 2);
        }

        // split the items
        string[] sArray = v3String.Split(',');

        // store as a Vector2Int
        Vector2Int  result = new Vector2Int(
            int.Parse(sArray[0]),
            int.Parse(sArray[1]));

        return result;
    }

    public static Type GetType(string typeName, string[] additionalAsmDefs = null)
    {
        // 1. Try Type.GetType first
        var type = Type.GetType(typeName);
        if (type != null)
            return type;

        // 2. If typeName is fully qualified, try loading the assembly directly
        if (typeName.Contains("."))
        {
            var assemblyName = typeName.Substring(0, typeName.IndexOf('.'));
            try
            {
                var assembly = Assembly.Load(assemblyName);
                type = assembly?.GetType(typeName);
                if (type != null)
                    return type;
            }
            catch
            {
                // ignored
            }
        }

        // 3. Check referenced assemblies of current assembly
        var currentAssembly = Assembly.GetExecutingAssembly();
        foreach (var asmName in currentAssembly.GetReferencedAssemblies())
        {
            try
            {
                var asm = Assembly.Load(asmName);
                type = asm?.GetType(typeName);
                if (type != null)
                    return type;
            }
            catch
            {
                // ignored
            }
        }

        // 4. Check additional assemblies explicitly passed in
        if (additionalAsmDefs != null)
        {
            foreach (var asmName in additionalAsmDefs)
            {
                try
                {
                    var asm = Assembly.Load(asmName);
                    type = asm?.GetType(typeName);
                    if (type != null)
                        return type;
                }
                catch
                {
                    // ignored
                }
            }
        }

        // 5. Type not found
        return null;
    }

}