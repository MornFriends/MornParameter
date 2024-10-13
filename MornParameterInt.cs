using System;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MornParameter
{
    [Serializable]
    public struct MornParameterInt
    {
        [SerializeField] private int _value;
        [SerializeField] private int _minValue;
        [SerializeField] private int _maxValue;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private ValueType _valueType;

        internal enum ValueType
        {
            Constant,
            Curve,
            RandomBetweenTwoConstants,
        }

        public int Value
        {
            get
            {
                switch (_valueType)
                {
                    case ValueType.Constant:
                        return _value;
                    case ValueType.Curve:
                        return Mathf.RoundToInt(_curve.Evaluate(Random.value));
                    case ValueType.RandomBetweenTwoConstants:
                        return Random.Range(_minValue, _maxValue);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MornParameterInt))]
    internal class MornParameterIntDrawer : PropertyDrawer
    {
        private SerializedProperty _value;
        private SerializedProperty _minValue;
        private SerializedProperty _maxValue;
        private SerializedProperty _curve;
        private SerializedProperty _valueType;
        private readonly int[] _valueRange = new int[2];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TryInitialize(property, ref _value, "_value");
            TryInitialize(property, ref _minValue, "_minValue");
            TryInitialize(property, ref _maxValue, "_maxValue");
            TryInitialize(property, ref _curve, "_curve");
            TryInitialize(property, ref _valueType, "_valueType");
            var paramRect = new Rect(position.x, position.y, position.width - 20, EditorGUIUtility.singleLineHeight);
            var enumRect = new Rect(position.x + position.width - 20, position.y, 20, EditorGUIUtility.singleLineHeight);
            paramRect = EditorGUI.PrefixLabel(paramRect, label);
            switch ((MornParameterInt.ValueType)_valueType.enumValueIndex)
            {
                case MornParameterInt.ValueType.Constant:
                    EditorGUI.PropertyField(paramRect, _value, GUIContent.none);
                    break;
                case MornParameterInt.ValueType.Curve:
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
                case MornParameterInt.ValueType.RandomBetweenTwoConstants:
                    paramRect.x -= 15;
                    paramRect.width += 15;
                    _valueRange[0] = _minValue.intValue;
                    _valueRange[1] = _maxValue.intValue;
                    EditorGUI.MultiIntField(paramRect,
                        new GUIContent[]
                        {
                            new(" "),
                            new(" "),
                        },
                        _valueRange);
                    _minValue.intValue = _valueRange[0];
                    _maxValue.intValue = _valueRange[1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _valueType.enumValueIndex = EditorGUI.Popup(enumRect, _valueType.enumValueIndex, Enum.GetNames(typeof(MornParameterInt.ValueType)));
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
#endif
}