using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    // Custom attribute and drawer to access to interface via an inspector view.
    // https://qiita.com/Teach/items/54769db9bb4ab5d7ce79

    [CustomPropertyDrawer(typeof(ComponentRestrictionAttribute))]
    public class ComponentRestrictionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var restriction = (ComponentRestrictionAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                EditorGUI.ObjectField(position, property, restriction.type);
            }
            else
            {
                EditorGUI.PropertyField(position, property);
            }
        }
    }
}
