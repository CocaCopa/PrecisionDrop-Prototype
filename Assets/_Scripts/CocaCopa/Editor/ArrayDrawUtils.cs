#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CocaCopa.EditorUtils {
    /// <summary>
    /// Utility helper for drawing SerializedProperty arrays/lists using a ReorderableList
    /// with optional scroll behaviour after a configurable number of visible elements.
    /// </summary>
    public static class ArrayDrawUtils {
        private const float ScrollbarWidth = 12f;
        private const float WheelSpeed = 18f;

        private static readonly Dictionary<string, ReorderableList> Lists = new();
        private static readonly Dictionary<string, Vector2> Scrolls = new();

        /// <summary>
        /// Header configuration options for a drawn array.<br/>
        /// Allows specifying a custom label and GUIStyle.
        /// </summary>
        public readonly struct HeaderOpt {

            /// <summary>
            /// Header label text. If null or empty, the property's displayName is used.
            /// </summary>
            public readonly string Label;

            /// <summary>
            /// GUIStyle used when rendering the header label.
            /// </summary>
            public readonly GUIStyle Style;

            /// <summary>
            /// Creates header options using the default EditorStyles.label style.
            /// </summary>
            public HeaderOpt(string label) {
                Label = label;
                Style = EditorStyles.label;
            }

            /// <summary>
            /// Creates header options with a custom GUIStyle.
            /// </summary>
            public HeaderOpt(string label, GUIStyle style) {
                Label = label;
                Style = style;
            }
        }

        /// <summary>
        /// Layout configuration options for the rendered array.<br/>
        /// Controls scroll threshold and bottom padding.
        /// </summary>
        public readonly struct RectOpt {

            /// <summary>
            /// Maximum number of visible elements before scroll mode activates.
            /// </summary>
            public readonly int MaxVisibleWithoutScroll;

            /// <summary>
            /// Extra space added below the array block.
            /// </summary>
            public readonly float BottomPadding;

            /// <summary>
            /// Creates layout options with default bottom padding.
            /// </summary>
            public RectOpt(int maxVisibleWithoutScroll) {
                MaxVisibleWithoutScroll = maxVisibleWithoutScroll;
                BottomPadding = 2f;
            }

            /// <summary>
            /// Creates layout options with explicit scroll threshold and bottom padding.
            /// </summary>
            public RectOpt(int maxVisibleWithoutScroll, float bottomPadding) {
                MaxVisibleWithoutScroll = maxVisibleWithoutScroll;
                BottomPadding = bottomPadding;
            }
        }

        /// <summary>
        /// Draws a custom reorderable array with default layout configuration.
        /// Scroll activates after 10 elements and applies 20px bottom padding.
        /// </summary>
        /// <param name="arrayProp">SerializedProperty representing the array or list.</param>
        /// <param name="headerOptions">Header label and style configuration.</param>
        public static void DrawCustomArray(SerializedProperty arrayProp, HeaderOpt headerOptions) {
            DrawCustomArray(arrayProp, headerOptions, new RectOpt(10, 20f));
        }

        /// <summary>
        /// Draws a custom reorderable array with configurable header and layout behavior.
        /// Automatically switches to scroll mode after the configured element threshold.
        /// 
        /// NOTE:
        /// If used inside a PropertyDrawer, ensure sufficient height is allocated,
        /// otherwise layout-based padding may not appear.
        /// </summary>
        /// <param name="arrayProp">SerializedProperty representing the array or list.</param>
        /// <param name="headerOpt">Header rendering configuration.</param>
        /// <param name="rectOpt">Layout configuration controlling scroll behavior and padding.</param>
        public static void DrawCustomArray(SerializedProperty arrayProp, HeaderOpt headerOpt, RectOpt rectOpt) {
            if (arrayProp == null) {
                EditorGUILayout.HelpBox("DrawCustomArray: arrayProp is null.", MessageType.Error);
                return;
            }

            if (!arrayProp.isArray || arrayProp.propertyType == SerializedPropertyType.String) {
                EditorGUILayout.HelpBox(
                    $"DrawCustomArray expects an array/list SerializedProperty. Got: {arrayProp.propertyType} at {arrayProp.propertyPath}",
                    MessageType.Error
                );
                EditorGUILayout.PropertyField(arrayProp, true);
                return;
            }

            var list = GetOrCreateList(arrayProp, headerOpt.Label);

            string collapseKey = GetCollapseKey(arrayProp);
            bool expanded = SessionState.GetBool(collapseKey, true);

            float headerH = list.headerHeight;

            // ================= HEADER =================
            // IMPORTANT: Allocate padding INSIDE the rect Unity reserves, otherwise it gets clipped.
            float headerBlockH = headerH + (expanded ? 0f : Mathf.Max(0f, rectOpt.BottomPadding));
            var rawHeaderRect = GUILayoutUtility.GetRect(0f, headerBlockH, GUILayout.ExpandWidth(true));
            var headerRect = EditorGUI.IndentedRect(new Rect(rawHeaderRect.x, rawHeaderRect.y, rawHeaderRect.width, headerH));

            if (Event.current.type == EventType.Repaint)
                ReorderableList.defaultBehaviours.DrawHeaderBackground(headerRect);

            HandleHeaderToggle(headerRect, ref expanded, collapseKey);
            DrawHeaderWithFoldout(headerRect, arrayProp, headerOpt, expanded);

            if (!expanded)
                return;

            // ============ NORMAL MODE (no scroll) ============
            if (arrayProp.arraySize <= rectOpt.MaxVisibleWithoutScroll) {
                DrawListWithoutHeader(list, rectOpt.BottomPadding);
                return;
            }

            // ============ SCROLL MODE ============
            string scrollKey = arrayProp.propertyPath;
            if (!Scrolls.TryGetValue(scrollKey, out var scroll))
                scroll = Vector2.zero;

            float footerH = list.footerHeight;
            float elementsViewportH = GetElementsViewportHeight(list, rectOpt.MaxVisibleWithoutScroll);

            float bottomPad = Mathf.Max(0f, rectOpt.BottomPadding);
            float contentH = elementsViewportH + footerH;
            float totalH = contentH + bottomPad;

            var fullRect = GUILayoutUtility.GetRect(0f, totalH, GUILayout.ExpandWidth(true));
            var contentRect = new Rect(fullRect.x, fullRect.y, fullRect.width, contentH);

            var elementsRect = EditorGUI.IndentedRect(new Rect(
                contentRect.x,
                contentRect.y,
                contentRect.width,
                elementsViewportH
            ));

            var footerRect = EditorGUI.IndentedRect(new Rect(
                contentRect.x,
                elementsRect.yMax,
                contentRect.width,
                footerH
            ));

            float totalElementsContentH = Mathf.Max(0f, list.GetHeight() - headerH - footerH);
            float maxScrollY = Mathf.Max(0f, totalElementsContentH - elementsViewportH);

            var e = Event.current;
            if (e.type == EventType.ScrollWheel && elementsRect.Contains(e.mousePosition)) {
                scroll.y += e.delta.y * WheelSpeed;
                scroll.y = Mathf.Clamp(scroll.y, 0f, maxScrollY);
                e.Use();
            }

            var scrollbarRect = new Rect(
                elementsRect.xMax - ScrollbarWidth,
                elementsRect.y,
                ScrollbarWidth,
                elementsRect.height
            );

            var elementsViewportRect = new Rect(
                elementsRect.x,
                elementsRect.y,
                elementsRect.width - ScrollbarWidth,
                elementsRect.height
            );

            scroll.y = GUI.VerticalScrollbar(
                scrollbarRect,
                scroll.y,
                elementsViewportH,
                0f,
                totalElementsContentH
            );

            scroll.y = Mathf.Clamp(scroll.y, 0f, maxScrollY);

            GUI.BeginGroup(elementsViewportRect);
            {
                var virtualFullRect = new Rect(
                    0f,
                    -scroll.y - headerH,
                    elementsViewportRect.width,
                    list.GetHeight()
                );

                list.DoList(virtualFullRect);
            }
            GUI.EndGroup();

            ReorderableList.defaultBehaviours.DrawFooter(footerRect, list);

            Scrolls[scrollKey] = scroll;
        }

        // =====================================================

        private static void HandleHeaderToggle(Rect headerRect, ref bool expanded, string collapseKey) {
            var e = Event.current;

            if (e.type == EventType.MouseDown &&
                e.button == 0 &&
                headerRect.Contains(e.mousePosition)) {
                expanded = !expanded;
                SessionState.SetBool(collapseKey, expanded);
                e.Use();
                GUI.changed = true;
            }
        }

        private static void DrawHeaderWithFoldout(Rect headerRect, SerializedProperty arrayProp, HeaderOpt headerOpt, bool expanded) {
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            try {
                const float triangleW = 1f;

                var triangleRect = new Rect(
                    headerRect.x + 17.5f,
                    headerRect.y + 1f,
                    triangleW,
                    headerRect.height - 2f
                );

                var labelRect = new Rect(
                    triangleRect.xMax + 2f,
                    headerRect.y,
                    headerRect.width - (triangleW + 6f),
                    headerRect.height
                );

                EditorGUI.Foldout(triangleRect, expanded, GUIContent.none, false);

                var title = string.IsNullOrEmpty(headerOpt.Label)
                    ? arrayProp.displayName
                    : headerOpt.Label;

                EditorGUI.LabelField(labelRect, title, headerOpt.Style);
            }
            finally { EditorGUI.indentLevel = oldIndent; }
        }

        private static void DrawListWithoutHeader(ReorderableList list, float bottomPadding) {
            float headerH = list.headerHeight;
            float contentH = list.GetHeight() - headerH;

            float bottomPad = Mathf.Max(0f, bottomPadding);
            float totalH = contentH + bottomPad;

            var rect = GUILayoutUtility.GetRect(0f, totalH, GUILayout.ExpandWidth(true));
            var contentRect = new Rect(rect.x, rect.y, rect.width, contentH);

            var indentedRect = EditorGUI.IndentedRect(contentRect);

            GUI.BeginGroup(indentedRect);
            {
                var local = new Rect(0f, -headerH, indentedRect.width, contentH + headerH);
                list.DoList(local);
            }
            GUI.EndGroup();
        }

        private static string GetCollapseKey(SerializedProperty arrayProp) {
            int id = arrayProp.serializedObject.targetObject != null
                ? arrayProp.serializedObject.targetObject.GetInstanceID()
                : 0;

            return $"CocaCopa.ArrayDrawUtils.Expanded.{id}.{arrayProp.propertyPath}";
        }

        private static float GetElementsViewportHeight(ReorderableList list, int visibleCount) {
            int count = Mathf.Min(visibleCount, list.count);

            if (count <= 0) {
                float one = list.elementHeightCallback != null
                    ? list.elementHeightCallback(0)
                    : list.elementHeight;
                return one + 6f;
            }

            float h = 0f;
            for (int i = 0; i < count; i++) {
                h += list.elementHeightCallback != null
                    ? list.elementHeightCallback(i)
                    : list.elementHeight;
            }

            return h + 4f;
        }

        private static ReorderableList GetOrCreateList(SerializedProperty arrayProp, string header) {
            var key = arrayProp.propertyPath;

            if (Lists.TryGetValue(key, out var existing)) {
                if (existing.serializedProperty != null &&
                    existing.serializedProperty.serializedObject == arrayProp.serializedObject) {
                    existing.serializedProperty = arrayProp;
                    return existing;
                }

                Lists.Remove(key);
            }

            var list = new ReorderableList(
                arrayProp.serializedObject,
                arrayProp,
                true, true, true, true
            );

            list.drawElementCallback = (rect, index, isActive, isFocused) => {
                var element = arrayProp.GetArrayElementAtIndex(index);
                rect.y += 1f;
                rect.height = EditorGUI.GetPropertyHeight(element, true);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            list.elementHeightCallback = index => {
                var element = arrayProp.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + 2f;
            };

            Lists[key] = list;
            return list;
        }
    }
}
#endif
