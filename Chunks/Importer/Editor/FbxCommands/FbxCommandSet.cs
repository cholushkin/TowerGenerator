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

        public FbxCommandSet(string fbxCommandName) : base(fbxCommandName)
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
            ComponentClassName = classAndParams[0].Split('[')[0].Trim();
            ComponentIndex = ConvertInt(classAndParams[0].Split('[', ']')[1].Trim());

            if (classAndParams.Length == 1)
                return; // only add component

            PropertyName = classAndParams[1].Split('=')[0].Trim();
            PropertyValue = classAndParams[1].Split('=')[1].Trim();
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportState importState)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importState);

            Type compType = CustomTypeConvertor.GetType(ComponentClassName);

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
        }
    }
}


public static class CustomTypeConvertor
{
    public static object ImplicitConvert(string valueString, Type type)
    {
        if (type == typeof(Vector3))
            return StringToVector3(valueString);
        throw new NotImplementedException($"{valueString} has no converter");
    }

    public static object StringToVector3(string v3String)
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



    public static Type GetType(string TypeName)
    {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        var type = Type.GetType(TypeName);

        // If it worked, then we're done here
        if (type != null)
            return type;

        // If the TypeName is a full name, then we can try loading the defining assembly directly
        if (TypeName.Contains("."))
        {

            // Get the name of the assembly (Assumption is that we are using 
            // fully-qualified type names)
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

            // Attempt to load the indicated Assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;

            // Ask that assembly to return the proper Type
            type = assembly.GetType(TypeName);
            if (type != null)
                return type;

        }

        // If we still haven't found the proper type, we can enumerate all of the 
        // loaded assemblies and see if any of them define the type
        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {

            // Load the referenced assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                // See if that assembly defines the named type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }

        // The type just couldn't be found...
        return null;

    }
}
