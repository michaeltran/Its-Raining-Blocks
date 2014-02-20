/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MadLevelManager {

public class MadGUI {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static Color Darker(Color color) {
        return new Color(color.r * 0.9f, color.g * 0.9f, color.b * 0.9f);
    }
    
    public static Color Brighter(Color color) {
        return new Color(color.r * 1.1f, color.g * 1.1f, color.b * 1.1f);
    }
    
    public static Color ToggleColor(Color color, ref bool last) {
        if (last) {
            last = false;
            return Darker(color);
        } else {
            last = true;
            return color;
        }
    }
    
    public static bool Button(string label) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(15 * EditorGUI.indentLevel);
        bool state = GUILayout.Button(label);
        GUILayout.EndHorizontal();
        
        return state;
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
#region ScrollableList
    public class ScrollableList<T> where T : ScrollableListItem {
        List<T> items;
    
        public string label = "Scrollable List";
        public int height = 200;
        
        public bool selectionEnabled = false;
        public string emptyListMessage = "No elements!";
        
        RunnableVoid1<T> _selectionCallback = (arg1) => {};
        public RunnableVoid1<T> selectionCallback {
            get { return _selectionCallback; }
            set { _selectionCallback += value; }
        }
        
        T _selectedItem;
        public T selectedItem {
            get { return _selectedItem; }
            set { 
                DeselectAll();
                _selectedItem = value;
                
                if (value != null) {
                    value.selected = true;
                    selectionCallback(value);
                }
            }
        }
        
        Vector2 position;
        GUISkin skin;
        
        T scrollToItem;
        
        public ScrollableList(List<T> items) {
            this.items = items;
        }
        
        public void Draw() {
            if (skin == null) {
                skin = Resources.Load("GUISkins/editorSkin", typeof(GUISkin)) as GUISkin;
            }
        
            bool toggleColor = false;
            Color baseColor = GUI.color;
        
            GUILayout.Label(label);
            position = EditorGUILayout.BeginScrollView(
                position, false, true, GUILayout.Height(height));
                
            float y = 0;
                
            foreach (var item in items) {
                if (scrollToItem == item && Event.current.type == EventType.Repaint) {
                    position.y = y;
                    scrollToItem = null;
                }
            
                var rect = EditorGUILayout.BeginHorizontal();
                if (selectionEnabled) {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                        if (rect.Contains(Event.current.mousePosition)) {
                            selectedItem = item;
                        }
                    }
                }
                
                // component value based on skin
                float c = EditorGUIUtility.isProSkin ? 0 : 1;
                
                GUI.color = toggleColor ? Color.clear : new Color(c, c, c, 0.2f);
                toggleColor = !toggleColor;
                if (item.selected) {
                    GUI.color = toggleColor ? new Color(c, c, c, 0.4f) : new Color(c, c, c, 0.6f);
                }
                
                GUI.Box(rect, "", skin.box);
                GUI.color = baseColor;
                
                EditorGUILayout.BeginVertical();
                item.OnGUI();
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                
                y += rect.height;
            }
            
            if (items.Count == 0) {
                GUILayout.Label(emptyListMessage);
            }
                
            EditorGUILayout.EndScrollView();
            
            GUI.color = baseColor;
        }
        
        void DeselectAll() {
            foreach (var i in items) {
                i.selected = false;
            }
        }
        
        public void ScrollToItem(T item) {
            scrollToItem = item;
            Draw();
        }
    };
    
#endregion

#region ArrayList

    public class ArrayList<T> where T : class, new() {
    
        public static readonly RunnableVoid1<SerializedProperty> PropertyRenderer = (prop) =>
        {
            MadGUI.PropertyField(prop, "");
        };
    
        List<T> items;
        SerializedProperty arrayProperty;
        
        RunnableVoid1<T> genericRenderer;
        RunnableVoid1<SerializedProperty> propertyRenderer;
        
        public RunnableGeneric0<T> createFunctionGeneric = () => { return new T(); };
        public RunnableVoid1<SerializedProperty> createFunctionProperty = (element) => {
            // when creating new array element like this, the color will be initialized with
            // (0, 0, 0, 0) - zero aplha. This may be confusing for end user so this workaround looks
            // for color fields and sets them to proper values                  
            var enumerator = element.GetEnumerator();
            while (enumerator.MoveNext()) {
                var el = enumerator.Current as SerializedProperty;
                if (el.type == "ColorRGBA") {
                    el.colorValue = Color.black;
                }
            }
        };
        
        public RunnableVoid0 beforeAdd = () => {};
        public RunnableVoid1<T> beforeRemove = (arg1) => {};
        
        public RunnableVoid1<T> onAdd = (arg1) => {};
        public RunnableVoid1<T> onRemove = (arg1) => {};
        
        public ArrayList(SerializedProperty arrayProperty, RunnableVoid1<SerializedProperty> renderer) {
            this.arrayProperty = arrayProperty;
            propertyRenderer = renderer;
        }
        
        public ArrayList(List<T> items, RunnableVoid1<T> renderer) {
            this.items = items;
            genericRenderer = renderer;
        }
        
        // items accessors
        int ItemCount() {
            if (items != null) {
                return items.Count;
            } else {
                return arrayProperty.arraySize;
            }
        }
        
        T ItemAt(int index) {
            if (items != null) {
                return items[index];
            } else {
                return arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue as T;
            }
        }
        
        void RemoveItem(int index) {
            if (items != null) {
                items.RemoveAt(index);
            } else {
                arrayProperty.DeleteArrayElementAtIndex(index);
            }
        }
        
        void AddItem() {
            beforeAdd();
        
            if (items != null) {
                items.Add(createFunctionGeneric());
            } else {
                arrayProperty.arraySize++;
                var prop = arrayProperty.GetArrayElementAtIndex(ItemCount() - 1);
                createFunctionProperty(prop);
            }
        }
        
        void RenderItem(int index) {
            if (items != null) {
                genericRenderer(items[index]);
            } else {
                propertyRenderer(arrayProperty.GetArrayElementAtIndex(index));
            }
        }
        
        public bool Draw() {
            EditorGUI.BeginChangeCheck();
            if (ItemCount() == 0) {
                GUILayout.Label("   Use 'Add' button to add items");
            } else {
                Separator();
                int removeIndex = -1;
                
                int count = ItemCount();
                for (int i = 0; i < count; ++i) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    RenderItem(i);
                    EditorGUILayout.EndVertical();
                    
                    GUI.color = Color.red;
                    if (GUILayout.Button("X", GUILayout.ExpandWidth(false))) {
                        removeIndex = i;
                    }
                    GUI.color = Color.white;
                    
                    EditorGUILayout.EndHorizontal();
                    
                    if (i + 1 > count) {
                        EditorGUILayout.Space();
                    }
                    Separator();
                }
                
                if (removeIndex != -1) {
                    T item = ItemAt(removeIndex);
                    beforeRemove(item);
                    RemoveItem(removeIndex);
                    onRemove(item);
                }
            }
                
            GUI.color = Color.green;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.ExpandWidth(false))) {
                AddItem();
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            
            return EditorGUI.EndChangeCheck();
        }
    }

#endregion

#region Wrappers
    public static void Box(RunnableVoid0 runnable) {
        Box("", runnable);
    }
    
    public static void Box(string label, RunnableVoid0 runnable) {
        BeginBox(label);
        runnable();
        EndBox();
    }
          
    public static void BeginBox() {
        BeginBox("");
    }
    
    public static void BeginBox(string label) {
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < EditorGUI.indentLevel; ++i) {
            EditorGUILayout.Space();
        }
        var rect = EditorGUILayout.BeginVertical();
        
        GUI.Box(rect, GUIContent.none);
        if (!string.IsNullOrEmpty(label)) {
            GUILayout.Label(label, "BoldLabel");
        }
        
        EditorGUILayout.Space();
    }
    
    public static void EndBox() {
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
    
    public static void Indent(RunnableVoid0 runnable) {
        EditorGUI.indentLevel++;
        runnable();
        EditorGUI.indentLevel--;
    }
    
    public static void IndentBox(RunnableVoid0 runnable) {
        IndentBox("", runnable);
    }
    
    public static void IndentBox(string label, RunnableVoid0 runnable) {
        Box(label, () => {
            Indent(() => {
                runnable();
            });
        });
    }
    
    public static void Disabled(RunnableVoid0 runnable) {
        ConditionallyEnabled(false, runnable);
    }
    
    public static void ConditionallyEnabled(bool enabled, RunnableVoid0 runnable) {
        bool prevState = GUI.enabled;
        GUI.enabled = enabled;
        runnable();
        GUI.enabled = prevState;
    }
    
    public static void LineHelp(ref bool state, string helpMessage, RunnableVoid0 runnable) {
        EditorGUILayout.BeginHorizontal();
        runnable();
        state = GUILayout.Toggle(state, "?", "Button", GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();
        if (state) {
            MadGUI.Message("Help:\n" + helpMessage, MessageType.Info);
        }
    }
#endregion
#region Messages
    public static bool InfoFix(string message) {
        return MessageFix(message, MessageType.Info);
    }
    
    public static bool InfoFix(string message, string fixMessage) {
        return MessageFix(message, fixMessage, MessageType.Info);
    }
          
    public static bool WarningFix(string message) {
        return MessageFix(message, MessageType.Warning);
    }
    
    public static bool ErrorFix(string message) {
        return MessageFix(message, MessageType.Error);
    }
    
    public static bool ErrorFix(string message, string fixMessage) {
        return MessageFix(message, fixMessage, MessageType.Error);
    }
    
    public static bool MessageFix(string message, MessageType messageType) {
        return MessageWithButton(message, "Fix it", messageType);
    }
    
    public static bool MessageFix(string message, string fixMessage, MessageType messageType) {
        return MessageWithButton(message, fixMessage, messageType);
    }
    
    public static bool MessageWithButton(string message, string buttonLabel, MessageType messageType) {
        EditorGUILayout.HelpBox(message, messageType);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        bool result = GUILayout.Button(buttonLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        return result;
    }
    
    public static int MessageWithButtonMulti(string message, MessageType messageType, params string[] buttonLabel) {
        EditorGUILayout.HelpBox(message, messageType);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        int result = -1;
        for (int i = 0; i < buttonLabel.Length; ++i) {
            if (GUILayout.Button(buttonLabel[i])) {
                result = i;
            }
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        return result;
    }
    
    public static void Warning(string message) {
        Message(message, MessageType.Warning);
    }
    
    public static void Info(string message) {
        Message(message, MessageType.Info);
    }
    
    public static void Error(string message) {
        Message(message, MessageType.Error);
    }
    
    public static void Message(string message, MessageType messageType) {
        EditorGUILayout.HelpBox(message, messageType);
    }
#endregion

#region Properties
    public static void Validate(Validator0 validator, RunnableVoid0 runnable) {
        Color prevColor = GUI.color;
        if (GUI.enabled && !validator()) {
            GUI.color = Color.red;
        }
        
        runnable();
        
        GUI.color = prevColor;
    }

    public static void PropertyField(SerializedProperty obj, string label) {
        PropertyField(obj, label, (s) => true);
    }
    
    public static void PropertyField(SerializedProperty obj, string label, Validator validator) {
        Validate(() => validator(obj), () => {
            EditorGUILayout.PropertyField(obj, new GUIContent(label));
        });
    }
    
    public static void PropertyField(SerializedProperty obj, string label, string tooltip) {
        PropertyField(obj, label, tooltip, (s) => true);
    }
    
    public static void PropertyField(SerializedProperty obj, string label, string tooltip, Validator validator) {
        Validate(() => validator(obj), () => {
            EditorGUILayout.PropertyField(obj, new GUIContent(label, tooltip));
        });
    }
    
    public static void PropertyFieldSlider(SerializedProperty obj, float leftValue, float rightValue, string label) {
        obj.floatValue = EditorGUILayout.Slider(label, obj.floatValue, leftValue, rightValue);
    }
    
    public static void PropertyFieldVector2(SerializedProperty obj, string label) {
        obj.vector2Value = EditorGUILayout.Vector2Field(label, obj.vector2Value);
    }
    
    public static void PropertyFieldVector2Compact(SerializedProperty obj, string label) {
        PropertyFieldVector2Compact(obj, label, 0);
    }
    
    public static void PropertyFieldVector2Compact(SerializedProperty obj, string label, int labelWidth) {
        EditorGUILayout.BeginHorizontal();
        Vector2 v = obj.vector2Value;
        float x = v.x;
        float y = v.y;
        
        EditorGUI.BeginChangeCheck();
        if (labelWidth > 0) {
            GUILayout.Label(label, GUILayout.Width(labelWidth));
        } else {
            GUILayout.Label(label);
        }
        LookLikeControls(15);
        x = EditorGUILayout.FloatField("X", x);
        y = EditorGUILayout.FloatField("Y", y);
        LookLikeControls(0);
        
        if (EditorGUI.EndChangeCheck()) {
            obj.vector2Value = new Vector2(x, y);
        }
        EditorGUILayout.EndHorizontal();
    }
    
    public static void PropertyFieldEnumPopup(SerializedProperty obj, string label) {
        var names = obj.enumNames;
        // split names by camel-case
        for (int i = 0; i < names.Length; ++i) {
            string name = names[i];
            var newName = new StringBuilder();
            
            for (int j = 0; j < name.Length; ++j) {
                char c = name[j];
                if (char.IsUpper(c) && j > 0) {
                    newName.Append(" ");
                }
                
                newName.Append(c);
            }
            
            names[i] = newName.ToString();
        }
        
        int selectedIndex = obj.enumValueIndex;
        int newIndex = EditorGUILayout.Popup(label, selectedIndex, names);
        
        if (selectedIndex != newIndex) {
            obj.enumValueIndex = newIndex;
        }
    }
    
    public static void LookLikeControls(int labelWidth) {
        LookLikeControls(labelWidth, 0);
    }
    
    public static void LookLikeControls(int labelWidth, int fieldWidth) {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
        EditorGUIUtility.LookLikeControls(labelWidth, fieldWidth);
#else
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUIUtility.fieldWidth = fieldWidth;
#endif
    }
    
    public static void PropertyFieldToggleGroup(SerializedProperty obj, string label, RunnableVoid0 runnable) {
        obj.boolValue = EditorGUILayout.BeginToggleGroup(label, obj.boolValue);
        
        runnable();
        EditorGUILayout.EndToggleGroup();
    }
    
    public static void PropertyFieldToggleGroup2(SerializedProperty obj, string label, RunnableVoid0 runnable) {
        obj.boolValue = EditorGUILayout.Toggle(label, obj.boolValue);
        
        bool savedState = GUI.enabled;
        GUI.enabled = obj.boolValue;
        runnable();
        GUI.enabled = savedState;
    }
    
    public static void PropertyFieldToggleGroupInv2(SerializedProperty obj, string label, RunnableVoid0 runnable) {
        obj.boolValue = !EditorGUILayout.Toggle(label, !obj.boolValue);
        
        bool savedState = GUI.enabled;
        GUI.enabled = !obj.boolValue;
        runnable();
        GUI.enabled = savedState;
    }
    
    public static void PropertyFieldObjectsPopup<T>(Object target, string label, ref T selectedObject, List<T> objects,
        bool allowEditWhenDisabled) where T : Component {
        
        bool active = allowEditWhenDisabled || 
#if UNITY_3_5
        ((MonoBehaviour) target).gameObject.active;
#else
        ((MonoBehaviour) target).gameObject.activeInHierarchy;
#endif
        
        bool guiEnabledPrev = GUI.enabled;
        GUI.enabled = active && guiEnabledPrev;
    
        if (GUI.enabled) {
            int index = 0;
            List<string> names = objects.ConvertAll((T input) => input.name);
            
            if (selectedObject != null) {
                T so = selectedObject;
                int foundIndex = objects.FindIndex((obj) => obj == so);
                if (foundIndex != -1) {
                    index = foundIndex + 1;
                }
            }
            
            names.Insert(0, "--");
            
            index = EditorGUILayout.Popup(label, index, names.ToArray());
            
            if (index == 0) {
                if (selectedObject != null) {
                    selectedObject = null;
                    EditorUtility.SetDirty(target);
                }
            } else {
                var newObject = objects[index - 1];
                if (selectedObject != newObject) {
                    selectedObject = newObject;
                    EditorUtility.SetDirty(target);
                }
            }
        } else {
            if (selectedObject == null) {
                EditorGUILayout.Popup(label, 0, new string[] {"--"});
            } else {
                EditorGUILayout.Popup(label, 0, new string[] {selectedObject.name});
            }
        }
        
        GUI.enabled = guiEnabledPrev;
    }
    
    public static bool Foldout(string name, bool defaultState) {
        bool state = EditorPrefs.GetBool(name, defaultState);
        
        bool newState = EditorGUILayout.Foldout(state, name);
        if (newState != state) {
            EditorPrefs.SetBool(name, newState);
        }
        
        return newState;
    }
    
    public static void Separator() {
        var rect = EditorGUILayout.BeginHorizontal();
        int indentPixels = (EditorGUI.indentLevel + 1) * 10 - 5;
        GUI.Box(new Rect(indentPixels, rect.yMin, rect.width - indentPixels, rect.height), "");
        EditorGUILayout.EndHorizontal();
    }
#endregion
    
    public delegate void RunnableVoid0();
    public delegate void RunnableVoid1<T>(T arg1);
    public delegate T RunnableGeneric0<T>();
    public delegate T RunnableGeneric1<T>(T arg1);
    public delegate bool Validator(SerializedProperty property);
    public delegate bool Validator0();
    
    public abstract class ScrollableListItem {
        public bool selected;
        
        public abstract void OnGUI();
    }
    
    public class ScrollableListItemLabel : ScrollableListItem {
        public string label;
        
        public ScrollableListItemLabel(string label) {
            this.label = label;
        }
        
        public override void OnGUI() {
            EditorGUILayout.LabelField(label);
        }
    }
    
    public static Validator StringNotEmpty = (SerializedProperty s) => {
        return !string.IsNullOrEmpty(s.stringValue);
    };
    
    public static Validator ObjectIsSet = (SerializedProperty s) => {
        return s.objectReferenceValue != null;
    };
}

} // namespace