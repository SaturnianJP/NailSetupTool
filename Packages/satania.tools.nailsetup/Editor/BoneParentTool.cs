using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Copyright © 2024 さたにあしょっぴんぐ
//Released under the MIT license
//https://opensource.org/licenses/mit-license.php

namespace satania.shopping.tool
{
    enum eBone
    {
        [InspectorName("選択してください。")] None,
        [InspectorName("右手(RightHand)/親指(Thumb)/第一関節(Proximal)")] RightThumbProximal = HumanBodyBones.RightThumbProximal,
        [InspectorName("右手(RightHand)/親指(Thumb)/第二関節(Intermediate)")] RightThumbIntermediate = HumanBodyBones.RightThumbIntermediate,
        [InspectorName("右手(RightHand)/親指(Thumb)/第三関節(Distal)")] RightThumbDistal = HumanBodyBones.RightThumbDistal,

        [InspectorName("右手(RightHand)/人差し指(Index)/第一関節(Proximal)")] RightIndexProximal = HumanBodyBones.RightIndexProximal,
        [InspectorName("右手(RightHand)/人差し指(Index)/第二関節(Intermediate)")] RightIndexIntermediate = HumanBodyBones.RightIndexIntermediate,
        [InspectorName("右手(RightHand)/人差し指(Index)/第三関節(Distal)")] RightIndexDistal = HumanBodyBones.RightIndexDistal,

        [InspectorName("右手(RightHand)/中指(Middle)/第一関節(Proximal)")] RightMiddleProximal = HumanBodyBones.RightMiddleProximal,
        [InspectorName("右手(RightHand)/中指(Middle)/第二関節(Intermediate)")] RightMiddleIntermediate = HumanBodyBones.RightMiddleIntermediate,
        [InspectorName("右手(RightHand)/中指(Middle)/第三関節(Distal)")] RightMiddleDistal = HumanBodyBones.RightMiddleDistal,

        [InspectorName("右手(RightHand)/薬指(Ring)/第一関節(Proximal)")] RightRingProximal = HumanBodyBones.RightRingProximal,
        [InspectorName("右手(RightHand)/薬指(Ring)/第二関節(Intermediate)")] RightRingIntermediate = HumanBodyBones.RightRingIntermediate,
        [InspectorName("右手(RightHand)/薬指(Ring)/第三関節(Distal)")] RightRingDistal = HumanBodyBones.RightRingDistal,

        [InspectorName("右手(RightHand)/小指(Little)/第一関節(Proximal)")] RightLittleProximal = HumanBodyBones.RightLittleProximal,
        [InspectorName("右手(RightHand)/小指(Little)/第二関節(Intermediate)")] RightLittleIntermediate = HumanBodyBones.RightLittleIntermediate,
        [InspectorName("右手(RightHand)/小指(Little)/第三関節(Distal)")] RightLittleDistal = HumanBodyBones.RightLittleDistal,

        [InspectorName("左手(LeftHand)/親指(Thumb)/第一関節(Proximal)")] LeftThumbProximal = HumanBodyBones.LeftThumbProximal,
        [InspectorName("左手(LeftHand)/親指(Thumb)/第二関節(Intermediate)")] LeftThumbIntermediate = HumanBodyBones.LeftThumbIntermediate,
        [InspectorName("左手(LeftHand)/親指(Thumb)/第三関節(Distal)")] LeftThumbDistal = HumanBodyBones.LeftThumbDistal,

        [InspectorName("左手(LeftHand)/人差し指(Index)/第一関節(Proximal)")] LeftIndexProximal = HumanBodyBones.LeftIndexProximal,
        [InspectorName("左手(LeftHand)/人差し指(Index)/第二関節(Intermediate)")] LeftIndexIntermediate = HumanBodyBones.LeftIndexIntermediate,
        [InspectorName("左手(LeftHand)/人差し指(Index)/第三関節(Distal)")] LeftIndexDistal = HumanBodyBones.LeftIndexDistal,

        [InspectorName("左手(LeftHand)/中指(Middle)/第一関節(Proximal)")] LeftMiddleProximal = HumanBodyBones.LeftMiddleProximal,
        [InspectorName("左手(LeftHand)/中指(Middle)/第二関節(Intermediate)")] LeftMiddleIntermediate = HumanBodyBones.LeftMiddleIntermediate,
        [InspectorName("左手(LeftHand)/中指(Middle)/第三関節(Distal)")] LeftMiddleDistal = HumanBodyBones.LeftMiddleDistal,

        [InspectorName("左手(LeftHand)/薬指(Ring)/第一関節(Proximal)")] LeftRingProximal = HumanBodyBones.LeftRingProximal,
        [InspectorName("左手(LeftHand)/薬指(Ring)/第二関節(Intermediate)")] LeftRingIntermediate = HumanBodyBones.LeftRingIntermediate,
        [InspectorName("左手(LeftHand)/薬指(Ring)/第三関節(Distal)")] LeftRingDistal = HumanBodyBones.LeftRingDistal,

        [InspectorName("左手(LeftHand)/小指(Little)/第一関節(Proximal)")] LeftLittleProximal = HumanBodyBones.LeftLittleProximal,
        [InspectorName("左手(LeftHand)/小指(Little)/第二関節(Intermediate)")] LeftLittleIntermediate = HumanBodyBones.LeftLittleIntermediate,
        [InspectorName("左手(LeftHand)/小指(Little)/第三関節(Distal)")] LeftLittleDistal = HumanBodyBones.LeftLittleDistal,
    }

    public class BoneParentTool : EditorWindow
    {
        #region Rect
        private Rect titlePosition = new Rect(650 / 2 - (235 / 2), 6, 235, 235);
        #endregion

        #region Variables
        private eBone selectedBone = eBone.None;
        private Transform targetObj;
        private Animator _avatarAnimator;
        #endregion

        [MenuItem("さたにあしょっぴんぐ/ネイルセットアップツール/小物装着ツール", priority = -10)]
        private static void Init()
        {
            var window = GetWindow<BoneParentTool>();
            window.titleContent = new GUIContent("小物装着ツール");
            window.minSize = window.maxSize = new Vector2(650, 180);
            window.Show();
        }

        #region GUI
        public GUIStyle GetTitleLabelStyle()
        {
            var titleStyle = new GUIStyle();

            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.UpperCenter;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 30;

            return titleStyle;
        }

        private void OnGUI()
        {
            #region タイトル
            GUI.Label(titlePosition, new GUIContent("小物装着ツール"), GetTitleLabelStyle());
            #endregion

            GUILayout.Space(90 - 30);

            EditorGUI.BeginChangeCheck();
            _avatarAnimator = EditorGUILayout.ObjectField("アバター", _avatarAnimator, typeof(Animator), true) as Animator;
            if (EditorGUI.EndChangeCheck())
            {
                if (!_avatarAnimator.isHuman)
                {
                    _avatarAnimator = null;
                    return;
                }    
            }

            GUILayout.Space(15);

            targetObj = EditorGUILayout.ObjectField("オブジェクト", targetObj, typeof(Transform), true) as Transform;

            selectedBone = (eBone)EditorGUILayout.EnumPopup("つける場所", selectedBone);

            GUILayout.Space(15);

            if (GUILayout.Button("つける"))
            {
                if (selectedBone == eBone.None || targetObj == null || _avatarAnimator == null)
                    return;

                Transform bone = _avatarAnimator.GetBoneTransform((HumanBodyBones)selectedBone);
                if (bone == null) 
                    return;

                targetObj.transform.SetParent(bone);
            }
        }
        #endregion
    }
}

