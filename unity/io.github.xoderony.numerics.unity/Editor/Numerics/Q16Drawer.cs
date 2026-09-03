using System.Globalization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Xoderony.Numerics.Unity.Editor;

[CustomPropertyDrawer(typeof(Q16))]
public sealed class Q16Drawer : PropertyDrawer {

    private const double MinimumValue = (double)int.MinValue / Q16.Scale;

    private const double MaximumValue = (double)int.MaxValue / Q16.Scale;

    private const string ValueFormat = "G17";

    private const string InvalidInputMessage = "Q16 input must be a finite number or numeric expression between -32768 and 32767.9999847412109375. The stored value was not changed.";

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        var rawProperty = property.FindPropertyRelative(nameof(Q16.RawValue));
        var field = new Q16Field(property.displayName);
        field.AddToClassList(BaseField<double>.alignedFieldUssClassName);
        field.SetEnabled(property.editable);
        RefreshField(field, rawProperty, property.tooltip);

        field.RegisterValueChangedCallback(evt => {
            if (!TryGetRawValue(evt.newValue, out var rawValue)) {
                Debug.LogWarning(InvalidInputMessage);
                RefreshField(field, rawProperty, property.tooltip);
                return;
            }

            if (rawProperty.hasMultipleDifferentValues || rawProperty.intValue != rawValue) {
                rawProperty.intValue = rawValue;
                rawProperty.serializedObject.ApplyModifiedProperties();
            }
            RefreshField(field, rawProperty, property.tooltip);
        });
        field.TrackPropertyValue(rawProperty, changedProperty => {
            RefreshField(field, changedProperty, property.tooltip);
        });
        return field;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var rawProperty = property.FindPropertyRelative(nameof(Q16.RawValue));
        var fieldLabel = new GUIContent(label) {
            tooltip = GetTooltip(rawProperty, label.tooltip)
        };
        fieldLabel = EditorGUI.BeginProperty(position, fieldLabel, property);
        var previousMixedValue = EditorGUI.showMixedValue;
        try {
            EditorGUI.showMixedValue = rawProperty.hasMultipleDifferentValues;
            using var disabledScope = new EditorGUI.DisabledScope(!property.editable);
            var text = ToValue(rawProperty.intValue).ToString(ValueFormat, CultureInfo.InvariantCulture);
            EditorGUI.BeginChangeCheck();
            // DelayedDoubleField 不公开显示格式；显式使用往返精度，避免提交时丢失底层刻度。
            text = EditorGUI.DelayedTextField(position, fieldLabel, text, EditorStyles.numberField);
            if (!EditorGUI.EndChangeCheck()) {
                return;
            }
            if (!TryParseRawValue(text, out var rawValue)) {
                Debug.LogWarning(InvalidInputMessage);
                return;
            }
            if (rawProperty.hasMultipleDifferentValues || rawProperty.intValue != rawValue) {
                rawProperty.intValue = rawValue;
            }
        } finally {
            EditorGUI.showMixedValue = previousMixedValue;
            EditorGUI.EndProperty();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight;
    }

    private static double ToValue(int rawValue) {
        return (double)rawValue / Q16.Scale;
    }

    private static bool TryGetRawValue(double value, out int rawValue) {
        if (double.IsNaN(value) || value < MinimumValue || value > MaximumValue) {
            rawValue = default;
            return false;
        }
        rawValue = (int)(value * Q16.Scale);
        return true;
    }

    private static bool TryParseRawValue(string text, out int rawValue) {
        var parsed = double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value);
        if (!parsed) {
            parsed = ExpressionEvaluator.Evaluate(text, out value);
        }
        if (parsed && TryGetRawValue(value, out rawValue)) {
            return true;
        }
        rawValue = default;
        return false;
    }

    private static void RefreshField(DoubleField field, SerializedProperty rawProperty, string tooltip) {
        field.SetValueWithoutNotify(ToValue(rawProperty.intValue));
        field.showMixedValue = rawProperty.hasMultipleDifferentValues;
        field.tooltip = GetTooltip(rawProperty, tooltip);
    }

    private static string GetTooltip(SerializedProperty rawProperty, string tooltip) {
        var details = rawProperty.hasMultipleDifferentValues
            ? "Q16: multiple different values."
            : string.Format(CultureInfo.InvariantCulture, "Q16: {0:G17}\nExact fraction: {1}/{2}\nRawValue: {1}", ToValue(rawProperty.intValue), rawProperty.intValue, Q16.Scale);
        return string.IsNullOrEmpty(tooltip) ? details : tooltip + "\n\n" + details;
    }

    private sealed class Q16Field : DoubleField {

        public Q16Field(string label) : base(label) {
            isDelayed = true;
            formatString = ValueFormat;
        }

        protected override double StringToValue(string text) {
            // 无效输入交由值回调拒绝，避免混合值编辑时把首个对象的值写入全部对象。
            return TryParseRawValue(text, out var rawValue) ? ToValue(rawValue) : double.NaN;
        }
    }
}
