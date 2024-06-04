using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Base class for creating custom property drawers for polymorphic properties.
/// </summary>
/// <typeparam name="T">The base type of the polymorphic properties.</typeparam>
public abstract class PolymorphicPropertyDrawer<T> : PropertyDrawer where T : class
{

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the "None" option is allowed in the dropdown menu.
    /// </summary>
    protected virtual bool noneAllowed => false;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public sealed override float GetPropertyHeight ( SerializedProperty property, GUIContent label )
    {

        if (fieldInfo.FieldType != typeof ( T ) || property.propertyType != SerializedPropertyType.ManagedReference)
        {
            return PropertyHeight ( property, label, false );
        }
        else
        {
            Validate ( property );
            float height = EditorGUIUtility.singleLineHeight;

            float propertyHeight = PropertyHeight ( property, label, true );

            if (propertyHeight > 0 && property.isExpanded)
            {
                height += EditorGUIUtility.standardVerticalSpacing + propertyHeight;
            }

            return height;
        }
    }

    /// <inheritdoc/>
    public sealed override void OnGUI ( Rect position, SerializedProperty property, GUIContent label )
    {
        Rect labelRect = new ( position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight );
        Rect dropDownRect = new ( labelRect.x + labelRect.width + EditorGUIUtility.standardVerticalSpacing, position.y, position.width - (labelRect.width + EditorGUIUtility.standardVerticalSpacing), EditorGUIUtility.singleLineHeight );

        if (fieldInfo.FieldType == typeof ( T ) && property.propertyType == SerializedPropertyType.ManagedReference)
        {
            float propertyHeight = PropertyHeight ( property, label, true );
            object reference = property.managedReferenceValue;

            if (reference != null && propertyHeight > 0)
            {
                property.isExpanded = EditorGUI.Foldout ( labelRect, property.isExpanded, label, true );
            }
            else
            {
                EditorGUI.LabelField ( labelRect, label );
            }

            if (EditorGUI.DropdownButton ( dropDownRect, new GUIContent ( ObjectNames.NicifyVariableName ( PostProcessName ( reference?.GetType ().Name ?? "None" ) ) ), FocusType.Passive ))
            {
                GenericMenu menu = CreateMenu ( property );
                menu.DropDown ( dropDownRect );
            }

            if (reference != null && propertyHeight > 0 && property.isExpanded)
            {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.indentLevel++;
                DrawProperty ( position, property, label, true );
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            DrawProperty ( position, property, label, false );
        }
    }

    /// <summary>
    /// Calculates the height of the property.
    /// </summary>
    /// <param name="property">The SerializedProperty to calculate height for.</param>
    /// <param name="label">The GUIContent for the property.</param>
    /// <param name="isPolymorphic">Indicates whether the property is polymorphic.</param>
    /// <returns>The height of the property.</returns>
    protected virtual float PropertyHeight ( SerializedProperty property, GUIContent label, bool isPolymorphic )
    {
        return 0;
    }

    /// <summary>
    /// Draws the property.
    /// </summary>
    /// <param name="position">The position to draw the property.</param>
    /// <param name="property">The SerializedProperty to draw.</param>
    /// <param name="label">The GUIContent for the property.</param>
    /// <param name="isPolymorphic">Indicates whether the property is polymorphic.</param>
    protected virtual void DrawProperty ( Rect position, SerializedProperty property, GUIContent label, bool isPolymorphic )
    {

    }

    /// <summary>
    /// Post-processes the type names for the dropdown.
    /// </summary>
    /// <param name="name">The type name to be post-processed.</param>
    /// <returns>The post-processed type name.</returns>
    protected virtual string PostProcessName ( string name )
    {
        return name;
    }

    private GenericMenu CreateMenu ( SerializedProperty property )
    {
        GenericMenu menu = new ();

        int count = PolymorphicTypeManager<T>.TypeCount;

        if (noneAllowed || count == 0)
        {
            menu.AddItem ( new GUIContent ( "None" ), property.managedReferenceValue == null, () => SetProperty ( property, null ) );
        }

        if (PolymorphicTypeManager<T>.TypeCount > 0)
        {
            if (noneAllowed)
            {
                menu.AddSeparator ( "" );
            }

            if (count > 10)
            {
                PopulateLarge ( menu, property );
            }
            else
            {
                PopulateSmall ( menu, property );
            }
        }

        return menu;
    }

    private void PopulateSmall ( GenericMenu menu, SerializedProperty property )
    {
        foreach (PolymorphicTypeManager<T>.TypeData data in PolymorphicTypeManager<T>.Types)
        {
            menu.AddItem (
                new GUIContent ( ObjectNames.NicifyVariableName ( PostProcessName ( data.Name ) ) ),
                property.managedReferenceValue != null && property.managedReferenceValue.GetType () == data.Type,
                () => SetProperty ( property, data.Instance ) );
        }
    }

    private void PopulateLarge ( GenericMenu menu, SerializedProperty property )
    {
        foreach (PolymorphicTypeManager<T>.TypeData data in PolymorphicTypeManager<T>.Types)
        {
            string name = ObjectNames.NicifyVariableName ( PostProcessName ( data.Name ) );
            string category = name [ 0 ].ToString ();

            menu.AddItem (
                new GUIContent ( $"{category}/{name}" ),
                property.managedReferenceValue != null && property.managedReferenceValue.GetType () == data.Type,
                () => SetProperty ( property, data.Instance ) );
        }
    }

    private static void SetProperty ( SerializedProperty property, T value )
    {
        property.serializedObject.Update ();
        property.managedReferenceValue = value;
        property.serializedObject.ApplyModifiedProperties ();
    }

    private void Validate ( SerializedProperty property )
    {
        if (!noneAllowed && property.managedReferenceValue == null && PolymorphicTypeManager<T>.Types.Any ())
        {
            SetProperty ( property, PolymorphicTypeManager<T>.Default?.Instance );
        }
    }

    #endregion
}