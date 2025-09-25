using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// This attribute links the PropertyDrawer to your TagSet class.
[CustomPropertyDrawer(typeof(TowerGenerator.TagSet))]
public class TagSetInspector : PropertyDrawer
{
    // Cache for ReorderableList instances, keyed by property path.
    // This is crucial for handling multiple TagSet fields and ensuring state is maintained.
    private Dictionary<string, ReorderableList> reorderableListsCache = new Dictionary<string, ReorderableList>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // BeginProperty / EndProperty ensure that prefab override logic works correctly.
        EditorGUI.BeginProperty(position, label, property);

        // Get or create the ReorderableList for the current property.
        ReorderableList list = GetOrCreateReorderableList(property, label);

        // Let the ReorderableList handle its drawing and height.
        // We pass the full position rect it should occupy.
        list.DoList(position);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get or create the ReorderableList to calculate its height.
        ReorderableList list = GetOrCreateReorderableList(property, label);
        return list.GetHeight();
    }

    private ReorderableList GetOrCreateReorderableList(SerializedProperty property, GUIContent label)
    {
        string propertyPath = property.propertyPath;
        ReorderableList listInstance;

        // Find the "Tags" list property within the TagSet property.
        // Your TagSet class has "public List<Tag> Tags;"
        SerializedProperty tagsListProperty = property.FindPropertyRelative("Tags");

        // Error handling if the "Tags" property cannot be found.
        if (tagsListProperty == null)
        {
            Debug.LogError($"TagSetInspector: Could not find 'Tags' list property in '{property.name}' at path '{propertyPath}'. " +
                             "Ensure TowerGenerator.TagSet has a public List<Tag> Tags; field.");
            // Return a dummy list to prevent further null errors, though it won't function.
            return new ReorderableList(property.serializedObject, null, false, false, false, false);
        }
        if (!tagsListProperty.isArray)
        {
            Debug.LogError($"TagSetInspector: Property 'Tags' at path '{propertyPath}' is not an array or list.");
            return new ReorderableList(property.serializedObject, tagsListProperty, false, false, false, false);
        }

        // Check cache:
        // Re-create the list if it's not in cache, or if the underlying SerializedObject/property has changed.
        // This handles cases like recompilation or selecting a different object.
        if (!reorderableListsCache.TryGetValue(propertyPath, out listInstance) ||
            listInstance.serializedProperty == null || // List's property somehow became null
            listInstance.serializedProperty.serializedObject != tagsListProperty.serializedObject || // Target object changed
            listInstance.serializedProperty.propertyPath != tagsListProperty.propertyPath) // Property path changed
        {
            // Create a new ReorderableList instance.
            listInstance = new ReorderableList(property.serializedObject, tagsListProperty,
                                               draggable: true, displayHeader: true,
                                               displayAddButton: true, displayRemoveButton: true);

            // Callback to draw the header of the list.
            listInstance.drawHeaderCallback = (Rect rect) =>
            {
                // Use the main property label for the header.
                EditorGUI.LabelField(rect, label.text + " (Tags)");
            };

            // Callback to draw each element in the list.
            listInstance.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                // Ensure the index is valid.
                if (index >= tagsListProperty.arraySize) return;

                SerializedProperty elementProperty = tagsListProperty.GetArrayElementAtIndex(index);
                // Your TagSet.Tag class has "public string Name;" and "public float Value;"
                SerializedProperty nameProperty = elementProperty.FindPropertyRelative("Name");
                SerializedProperty valueProperty = elementProperty.FindPropertyRelative("Value");

                if (nameProperty == null || valueProperty == null)
                {
                    EditorGUI.LabelField(rect, "Error: 'Name' or 'Value' not found in TagSet.Tag element.");
                    return;
                }

                // Adjust rect for vertical padding within the element row.
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                float spacing = 5f;
                // Divide the available width for Name and Value fields.
                float fieldWidth = (rect.width - spacing) / 2f;

                Rect nameRect = new Rect(rect.x, rect.y, fieldWidth, rect.height);
                Rect valueRect = new Rect(rect.x + fieldWidth + spacing, rect.y, fieldWidth, rect.height);

                // Use BeginChangeCheck/EndChangeCheck to detect modifications.
                EditorGUI.BeginChangeCheck();
                string newName = EditorGUI.TextField(nameRect, nameProperty.stringValue);
                float newValue = EditorGUI.FloatField(valueRect, valueProperty.floatValue);

                if (EditorGUI.EndChangeCheck())
                {
                    nameProperty.stringValue = newName;
                    // Clamp value based on your Tag constructor's assertion, if desired in editor.
                    // valueProperty.floatValue = Mathf.Clamp01(newValue);
                    valueProperty.floatValue = newValue; // Or allow any float if assertions are only for runtime
                }
            };

            // Callback when an element is reordered.
            listInstance.onReorderCallback = (ReorderableList l) =>
            {
                // This is crucial: Apply changes to the serialized object when the list is reordered.
                // ReorderableList handles moving the element in the SerializedProperty (array) internally.
                // We just need to tell Unity to save this change.
                // No need to call ApplyModifiedProperties on l.serializedProperty.serializedObject,
                // as 'property.serializedObject' (passed to ReorderableList constructor) is the correct one.
                property.serializedObject.ApplyModifiedProperties();
            };

            // Callback to determine the height of each element.
            listInstance.elementHeightCallback = (index) =>
            {
                return EditorGUIUtility.singleLineHeight + 4f; // Base height + vertical padding.
            };
            
            // Optional: Customize add behavior if your Tag class needs specific initialization.
            // By default, Unity will try to create a new element (e.g., by default constructor or copying).
            // If your Tag class has no parameterless constructor and you want specific default values:
            listInstance.onAddCallback = (ReorderableList l) =>
            {
                var index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index; // Select the new element
                var element = l.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty nameProp = element.FindPropertyRelative("Name");
                SerializedProperty valueProp = element.FindPropertyRelative("Value");
                
                // Initialize with default values that satisfy your Tag constructor's assertions
                if (nameProp != null) nameProp.stringValue = "NewTag";
                if (valueProp != null) valueProp.floatValue = 1.0f; // Default to 1.0f as per your Set method

                property.serializedObject.ApplyModifiedProperties(); // Apply after adding
            };


            // Store the newly created list in the cache.
            reorderableListsCache[propertyPath] = listInstance;
        }
        else
        {
            // If list was found in cache, ensure its underlying SerializedProperty is up-to-date.
            // This can be important if the SerializedObject instance changed but the propertyPath remained the same.
            listInstance.serializedProperty = tagsListProperty;
        }

        return listInstance;
    }
}