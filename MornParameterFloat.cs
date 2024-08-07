using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MornParameter
{
    [Serializable]
    public struct MornParameterFloat
    {
        [SerializeField] private float _value;
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private ValueType _valueType;

        internal enum ValueType
        {
            Constant,
            Curve,
            RandomBetweenTwoConstants,
        }

        public float Value
        {
            get
            {
                switch (_valueType)
                {
                    case ValueType.Constant:
                        return _value;
                    case ValueType.Curve:
                        return _curve.Evaluate(Random.value);
                    case ValueType.RandomBetweenTwoConstants:
                        return Random.Range(_minValue, _maxValue);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(MornParameterFloat))]
    internal class MornParameterFloatDrawer : PropertyDrawer
    {
        private SerializedProperty _value;
        private SerializedProperty _minValue;
        private SerializedProperty _maxValue;
        private SerializedProperty _curve;
        private SerializedProperty _valueType;
        private readonly float[] _valueRange = new float[2];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TryInitialize(property, ref _value, "_value");
            TryInitialize(property, ref _minValue, "_minValue");
            TryInitialize(property, ref _maxValue, "_maxValue");
            TryInitialize(property, ref _curve, "_curve");
            TryInitialize(property, ref _valueType, "_valueType");
            var paramRect = new Rect(position.x, position.y, position.width - 20, EditorGUIUtility.singleLineHeight);
            var enumRect = new Rect(position.x + position.width - 20, position.y, 20, EditorGUIUtility.singleLineHeight);

            // propertyのlabelを表示
            paramRect = EditorGUI.PrefixLabel(paramRect, label);
            switch ((MornParameterFloat.ValueType)_valueType.enumValueIndex)
            {
                case MornParameterFloat.ValueType.Constant:
                    EditorGUI.PropertyField(paramRect, _value, GUIContent.none);
                    break;
                case MornParameterFloat.ValueType.Curve:
                    EditorGUI.PropertyField(paramRect, _curve, GUIContent.none);
                    var animCurve = _curve.animationCurveValue;
                    var keys = animCurve.keys;
                    if (keys.Length > 0)
                    {
                        var minTime = keys[0].time;
                        var maxTime = minTime;
                        for (var i = 0; i < keys.Length; ++i)
                        {
                            minTime = Mathf.Min(minTime, keys[i].time);
                            maxTime = Mathf.Max(maxTime, keys[i].time);
                        }

                        var range = maxTime - minTime;
                        for (var i = 0; i < keys.Length; ++i)
                        {
                            keys[i].time = (keys[i].time - minTime) / range;
                        }

                        animCurve.keys = keys;
                        _curve.animationCurveValue = animCurve;
                        _curve.serializedObject.ApplyModifiedProperties();
                    }

                    break;
                case MornParameterFloat.ValueType.RandomBetweenTwoConstants:
                    paramRect.x -= 15;
                    paramRect.width += 15;
                    _valueRange[0] = _minValue.floatValue;
                    _valueRange[1] = _maxValue.floatValue;
                    EditorGUI.MultiFloatField(paramRect,
                        new GUIContent[]
                        {
                            new(" "),
                            new(" "),
                        },
                        _valueRange);
                    _minValue.floatValue = _valueRange[0];
                    _maxValue.floatValue = _valueRange[1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _valueType.enumValueIndex = EditorGUI.Popup(enumRect, _valueType.enumValueIndex, Enum.GetNames(typeof(MornParameterFloat.ValueType)));
        }

        private void TryInitialize(SerializedProperty origin, ref SerializedProperty target, string name)
        {
            if (target == null)
            {
                target = origin.FindPropertyRelative(name);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}