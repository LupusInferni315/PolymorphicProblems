using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer ( typeof ( StandardNameGenerator ) )]
public class StandardNameGeneratorPropertyDrawer : INameGeneratorPropertyDrawer
{
    protected override float PropertyHeight ( SerializedProperty property, GUIContent label, bool isPolymorphic )
    {
        float height = 0.0f;

        height += EditorGUI.GetPropertyHeight ( property.FindPropertyRelative ( "m_Feminine" ) ) + EditorGUIUtility.standardVerticalSpacing;
        height += EditorGUI.GetPropertyHeight ( property.FindPropertyRelative ( "m_Masculine" ) ) + EditorGUIUtility.standardVerticalSpacing;
        height += EditorGUI.GetPropertyHeight ( property.FindPropertyRelative ( "m_Neutral" ) ) + EditorGUIUtility.standardVerticalSpacing;
        height += EditorGUI.GetPropertyHeight ( property.FindPropertyRelative ( "m_FamilyNames" ) );

        return height;
    }

    protected override void DrawProperty ( Rect position, SerializedProperty property, GUIContent label, bool isPolymorphic )
    {
        Rect pos = new Rect ( position.x, position.y, position.width, EditorGUI.GetPropertyHeight ( property.FindPropertyRelative ( "m_Feminine" ) ) );
        EditorGUI.PropertyField ( pos, property.FindPropertyRelative ( "m_Feminine" ) );

        pos.y += pos.height + EditorGUIUtility.standardVerticalSpacing;
        pos.height = EditorGUI.GetPropertyHeight ( property.FindPropertyRelative ( "m_Masculine" ) );
        EditorGUI.PropertyField ( pos, property.FindPropertyRelative ( "m_Masculine" ) );

        pos.y += pos.height + EditorGUIUtility.standardVerticalSpacing;
        pos.height = EditorGUI.GetPropertyHeight ( property.FindPropertyRelative ( "m_Neutral" ) );
        EditorGUI.PropertyField ( pos, property.FindPropertyRelative ( "m_Neutral" ) );

        pos.y += pos.height + EditorGUIUtility.standardVerticalSpacing;
        pos.height = EditorGUI.GetPropertyHeight ( property.FindPropertyRelative ( "m_FamilyNames" ) );
        EditorGUI.PropertyField ( pos, property.FindPropertyRelative ( "m_FamilyNames" ) );
    }
}