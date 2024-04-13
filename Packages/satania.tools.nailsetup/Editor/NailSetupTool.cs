using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

//Copyright © 2024 さたにあしょっぴんぐ
//Released under the MIT license
//https://opensource.org/licenses/mit-license.php

namespace satania.shopping.tool
{
    enum eFinger
    {
        Index,
        Little,
        Middle,
        Ring,
        Thumb
    }

    enum eJoint
    {
        Proximal,
        Intermediate,
        Distal
    }

    enum eHand
    {
        Left,
        Right
    }

    public class NailSetupTool : EditorWindow
    {
        #region Bone Database
        string Hand_Regex = "hand|wrist|Hand";
        
        /*
         * Left
         * _L
         * left
         * _l
         * .l
         * .L
        */
        string Left_Regex = "(?:[Ll]eft|.[Ll]|_[Ll])";

        /*
         * Right
         * _R
         * right
         * _r
         * .R
         * .r
        */
        string Right_Regex = "(?:[Rr]ight|.[Rr]|_[Rr])";

        string Proximal_Regex = "(?:[Pp]ro|_1)";
        string Intermediate_Regex = "(?:[Ii]nter|_2)";
        string Distal_Regex = "(?:[Dd]is|_3)";

        /*
L_IndexFinger
Index
index
Left_Hand_Index_
*/
        string Left_Index_Regex = "(?:L(?:eft_Hand_Index_|_IndexFinger)|[Ii]ndex)";

        /*
L_LittleFinger
Little
little
Left_Hand_Pinky_
        */
        string Left_Little_Regex = "(?:L(?:eft_Hand_Pinky_|_LittleFinger|ittle)|little)";

        /*
L_MiddleFinger
Middle
middle
Left_Hand_Middle_
        */
        string Left_Middle_Regex = "(?:L(?:eft_Hand_Middle_|_MiddleFinger)|[Mm]iddle)";

        /*
L_RingFinger
Ring
ring
Left_Hand_Ring_
        */
        string Left_Ring_Regex = "(?:L(?:eft_Hand_Ring_|_RingFinger)|[Rr]ing)";

        /*
Left_Hand_Thumb_
Thumb
thumb
L_ThumbFinger
        */
        string Left_Thumb_Regex = "(?:L(?:eft_Hand_Thumb_|_ThumbFinger)|[Tt]humb)";

        /*
R_IndexFinger
Index
index
Right_Hand_Index_
        */
        string Right_Index_Regex = "(?:R(?:ight_Hand_Index_|_IndexFinger)|[Ii]ndex)";

        /*
R_LittleFinger
Little
little
Right_Hand_Pinky_
        */
        string Right_Little_Regex = "(?:R(?:ight_Hand_Pinky_|_LittleFinger)|[Ll]ittle)";

        /*
R_MiddleFinger
Middle
middle
Right_Hand_Middle_
        */
        string Right_Middle_Regex = "(?:R(?:ight_Hand_Middle_|_MiddleFinger)|[Mm]iddle)";

        /*
R_RingFinger
Ring
ring
Right_Hand_Ring_
        */
        string Right_Ring_Regex = "(?:R(?:i(?:ght_Hand_Ring_|ng)|_RingFinger)|ring)";

        /*
Right_Hand_Thumb_
Thumb
thumb
R_ThumbFinger
        */
        string Right_Thumb_Regex = "(?:R(?:ight_Hand_Thumb_|_ThumbFinger)|[Tt]humb)";
        #endregion

        #region Rect
        private Rect titlePosition = new Rect(650 / 2　- (235 / 2), 6, 235, 235);
        #endregion

        #region Variables
        /// <summary>
        /// ネイルをセットするアバターを選択
        /// </summary>
        private Animator _avatarAnimator;

        /// <summary>
        /// ボーンのフィールドを表示するか
        /// </summary>
        private bool isExtendBoneField = false;

        private bool isExtendNailField = false;

        private bool isSkinnedNail = true;

        /// <summary>
        /// ネイルのルートオブジェクト
        /// </summary>
        private Transform _nailTransform;

        /// <summary>
        /// 左手の指のボーンリスト
        /// </summary>
        private readonly HumanBodyBones[,] _leftFingers =
        {
            {                
                HumanBodyBones.LeftIndexProximal,
                HumanBodyBones.LeftLittleProximal,
                HumanBodyBones.LeftMiddleProximal,
                HumanBodyBones.LeftRingProximal,
                HumanBodyBones.LeftThumbProximal 
            },
            {
                HumanBodyBones.LeftIndexIntermediate,
                HumanBodyBones.LeftLittleIntermediate,
                HumanBodyBones.LeftMiddleIntermediate,
                HumanBodyBones.LeftRingIntermediate,
                HumanBodyBones.LeftThumbIntermediate
            },
            {
                HumanBodyBones.LeftIndexDistal,
                HumanBodyBones.LeftLittleDistal,
                HumanBodyBones.LeftMiddleDistal,
                HumanBodyBones.LeftRingDistal,
                HumanBodyBones.LeftThumbDistal
            }
        };

        /// <summary>
        /// 右手の指のボーンリスト
        /// </summary>
        private readonly HumanBodyBones[,] _rightFingers =
        {
            {
                HumanBodyBones.RightIndexProximal,
                HumanBodyBones.RightLittleProximal,
                HumanBodyBones.RightMiddleProximal,
                HumanBodyBones.RightRingProximal,
                HumanBodyBones.RightThumbProximal
            },
            {
                HumanBodyBones.RightIndexIntermediate,
                HumanBodyBones.RightLittleIntermediate,
                HumanBodyBones.RightMiddleIntermediate,
                HumanBodyBones.RightRingIntermediate,
                HumanBodyBones.RightThumbIntermediate
            },
            {
                HumanBodyBones.RightIndexDistal,
                HumanBodyBones.RightLittleDistal,
                HumanBodyBones.RightMiddleDistal,
                HumanBodyBones.RightRingDistal,
                HumanBodyBones.RightThumbDistal
            }
        };

        private Texture _boothLogo;

        private readonly string[] _fingerTexts = new string[] { "人差し指", "小指", "中指", "薬指", "親指" };

        /// <summary>
        /// アバターの左手の指の関節
        /// </summary>
        private Transform[,] _leftFingerTransforms = new Transform[3, 5];

        /// <summary>
        /// アバターの右手の指の関節
        /// </summary>
        private Transform[,] _rightFingerTransforms = new Transform[3, 5];

        private Transform[,] _nailLeftTransforms = new Transform[3, 5];
        private Transform[,] _nailRightTransforms = new Transform[3, 5];

        /// <summary>
        /// アバターの手のボーンを入れる配列
        /// </summary>
        private Transform[] _avatarHandTransforms = new Transform[2];
        private Transform[] _nailHandTransforms = new Transform[2];

        private Transform[,] _meshNailObjects = new Transform[2, 5];

        Vector2 _nailScrollerPosition, _avatarBoneScrollPosition;
        #endregion

        #region ボーン
        /// <summary>
        /// ダミーボーンを使っているタイプのボーンを探す
        /// </summary>
        /// <param name="bone">ダミーボーン</param>
        /// <param name="handBone">手のボーン</param>
        /// <param name="realBone">コンストレイントがついてるボーン</param>
        /// <returns></returns>
        private bool IsDummyBone(Transform bone, Transform handBone, out Transform realBone)
        {
            realBone = null;

            if (bone == null || handBone == null)
                return false;

            var rotConstraints = handBone.GetComponentsInChildren<RotationConstraint>(true);
            if (rotConstraints.Length > 0)
            {
                for (int i = 0; i < rotConstraints.Length; i++)
                {
                    //ソースが2個以上ある場合は別のギミックの可能性があるのでスルー
                    if (rotConstraints[i].sourceCount > 1)
                        continue;

                    ConstraintSource source = rotConstraints[i].GetSource(0);

                    //実際に参照している場合は本物を返す
                    if (source.sourceTransform == bone)
                    {
                        realBone = rotConstraints[i].transform;
                        return true;
                    }
                }
            }

            return false;
        }

        public void GetFingerBones(Animator avatar)
        {
            if (avatar == null || !avatar.isHuman)
                return;

            _avatarHandTransforms[(int)eHand.Left] = avatar.GetBoneTransform(HumanBodyBones.LeftHand);
            _avatarHandTransforms[(int)eHand.Right] = avatar.GetBoneTransform(HumanBodyBones.RightHand);

            Transform leftHand = _avatarHandTransforms[(int)eHand.Left];
            Transform rightHand = _avatarHandTransforms[(int)eHand.Right];

            //左手用
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    _leftFingerTransforms[j, i] = avatar.GetBoneTransform(_leftFingers[j, i]);

                    if (IsDummyBone(_leftFingerTransforms[j, i], leftHand, out Transform realBone))
                    {
                        _leftFingerTransforms[j, i] = realBone;
                    }
                }
            }

            //右手用
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    _rightFingerTransforms[j, i] = avatar.GetBoneTransform(_rightFingers[j, i]);

                    if (IsDummyBone(_rightFingerTransforms[j, i], rightHand, out Transform realBone))
                    {
                        _rightFingerTransforms[j, i] = realBone;
                    }
                }
            }
        }

        public static Transform FindIndex_Loop(Transform parent, string matchPattern, string word = "")
        {
            if (parent == null)
                return null;

            foreach (Transform childbone in parent)
            {
                if (Regex.IsMatch(childbone.name, matchPattern, RegexOptions.IgnoreCase))
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        if (!Regex.IsMatch(childbone.name, word, RegexOptions.IgnoreCase))
                        {
                            if (childbone.childCount > 0)
                            {
                                Transform temp = FindIndex_Loop(childbone, matchPattern, word);
                                if (temp != null)
                                    return temp;
                            }
                        }
                    }
                    return childbone;
                }
            }
            return null;
        }

        public Transform GetTransformByRegex(Transform target, string matchPattern, string subPattern)
        {
            if (string.IsNullOrEmpty(matchPattern))
                return null;

            foreach (var boneTransform in target.GetComponentsInChildren<Transform>(true))
            {
                if (Regex.IsMatch(boneTransform.name, matchPattern, RegexOptions.IgnoreCase))
                {
                    if (!string.IsNullOrEmpty(subPattern))
                    {
                        if (Regex.IsMatch(boneTransform.name, subPattern, RegexOptions.IgnoreCase))
                        {
                            return boneTransform;
                        }
                    }
                    else
                    {
                        return boneTransform;
                    }
                }
            }

            return null;
        }

        //アーマチュアに含まれる指のボーンの取得を試みる関数
        public void GetNailBones(Transform nail)
        {
            _nailHandTransforms[(int)eHand.Left] = GetTransformByRegex(nail, Hand_Regex, Left_Regex);
            _nailHandTransforms[(int)eHand.Right] = GetTransformByRegex(nail, Hand_Regex, Right_Regex);

            Transform leftHandTarget = _nailHandTransforms[(int)eHand.Left] == null ? nail : _nailHandTransforms[(int)eHand.Left];
            Transform rightHandTarget = _nailHandTransforms[(int)eHand.Right] == null ? nail : _nailHandTransforms[(int)eHand.Right];

            string[] leftRegexes = new string[] { Left_Index_Regex, Left_Little_Regex, Left_Middle_Regex, Left_Ring_Regex, Left_Thumb_Regex };
            string[] rightRegexes = new string[] { Right_Index_Regex, Right_Little_Regex, Right_Middle_Regex, Right_Ring_Regex, Right_Thumb_Regex };

            string[] jointRegexes = new string[] { Proximal_Regex, Intermediate_Regex, Distal_Regex };

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    _nailLeftTransforms[j, i] = GetTransformByRegex(leftHandTarget, leftRegexes[i], jointRegexes[j]);
                    _nailRightTransforms[j, i] = GetTransformByRegex(rightHandTarget, rightRegexes[i], jointRegexes[j]);
                }
            }
        }
        #endregion

        #region Gameobject
        public static bool isPrefab(GameObject obj)
        {
            //プレハブのインスタンスなのかをチェック
            PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(obj);

            return status == PrefabInstanceStatus.Connected || status == PrefabInstanceStatus.Disconnected;
        }
        #endregion

        #region GUI
        [MenuItem("さたにあしょっぴんぐ/ネイルセットアップツール/ネイルセットアップツール", priority = -10)]
        private static void Init()
        {
            var window = GetWindow<NailSetupTool>();
            window.titleContent = new GUIContent("ネイルセットアップツール");
            window.minSize = window.maxSize = new Vector2(650, 585);
            window.Show();
        }

        public Texture GetTextureFromGUID(string guid, string path)
        {
            string guidPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(guidPath))
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    guidPath = path;
                else
                    return null;
            }

            return AssetDatabase.LoadAssetAtPath<Texture>(guidPath);
        }

        public Texture GetBoothLogo()
        {
            if (_boothLogo != null)
                return _boothLogo;

            _boothLogo = GetTextureFromGUID("90ed6510c412e08439a1ca63209e7ea4", "Assets/さたにあしょっぴんぐ/NailSetupTool/Icons/logo_icon_r.png");
            return _boothLogo;
        }

        public void DrawLinkedButton(Texture logo, string url, Rect rect)
        {
            if (GUI.Button(rect, logo))
            {
                Help.BrowseURL(url);
            }
        }

        public GUIStyle GetTitleLabelStyle()
        {
            var titleStyle = new GUIStyle();

            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.UpperCenter;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 30;

            return titleStyle;
        }

        public GUIStyle GetButtonStyle()
        {
            var buttonStyle = new GUIStyle(GUI.skin.button);

            return buttonStyle;
        }

        private void DrawSkinnedNailField()
        {
            #region ネイルを入れるフィールド
            EditorGUI.BeginChangeCheck();
            _nailTransform = EditorGUILayout.ObjectField("ネイル", _nailTransform, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck())
            {
                _nailLeftTransforms = new Transform[3, 5];
                _nailRightTransforms = new Transform[3, 5];

                _nailHandTransforms[(int)eHand.Left] = null;
                _nailHandTransforms[(int)eHand.Right] = null;

                if (_nailTransform != null)
                {
                    if (!_nailTransform.gameObject.scene.IsValid())
                    {
                        _nailTransform = null;
                        EditorUtility.DisplayDialog("Nail Setup Tool", "オブジェクトがシーンに存在しません。\nシーンに配置してからツールに入れてください。", "はい");
                        return;
                    }

                    GetNailBones(_nailTransform);
                }
            }
            #endregion

            #region ネイルのボーンを表示するフィールド
            using (new GUILayout.VerticalScope(GetButtonStyle()))
            {
                isExtendNailField = EditorGUILayout.ToggleLeft("ネイルのボーン詳細設定", isExtendNailField);

                if (isExtendNailField)
                {
                    _nailScrollerPosition = GUILayout.BeginScrollView(_nailScrollerPosition, GUILayout.Height(150));

                    //GUILayout.Label("左手");
                    _nailHandTransforms[(int)eHand.Left] = EditorGUILayout.ObjectField("左手", _nailHandTransforms[(int)eHand.Left], typeof(Transform), true) as Transform;
                    for (int i = 0; i < 5; i++)
                    {
                        using (new GUILayout.HorizontalScope(GUI.skin.button))
                        {
                            GUILayout.Label($"{_fingerTexts[i]}", GUILayout.Width(90));
                            for (int j = 0; j < 3; j++)
                            {
                                EditorGUILayout.ObjectField(_nailLeftTransforms[j, i], typeof(Transform), true);
                            }
                        }
                    }

                    GUILayout.Space(20);

                    _nailHandTransforms[(int)eHand.Right] = EditorGUILayout.ObjectField("右手", _nailHandTransforms[(int)eHand.Right], typeof(Transform), true) as Transform;
                    for (int i = 0; i < 5; i++)
                    {
                        using (new GUILayout.HorizontalScope(GUI.skin.button))
                        {
                            GUILayout.Label($"{_fingerTexts[i]}", GUILayout.Width(90));
                            for (int j = 0; j < 3; j++)
                            {
                                EditorGUILayout.ObjectField(_nailRightTransforms[j, i], typeof(Transform), true);
                            }
                        }
                    }

                    GUILayout.EndScrollView();
                }
            }
            #endregion
        }

        private void DrawMeshNailField()
        {
            using (new GUILayout.VerticalScope(GetButtonStyle()))
            {
                isExtendNailField = EditorGUILayout.ToggleLeft("ネイルのボーン詳細設定", isExtendNailField);
                if (isExtendNailField)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        //左手用フィールド
                        using (new GUILayout.VerticalScope(GetButtonStyle()))
                        {
                            GUILayout.Label("左手");
                            for (int i = 0; i < 5; i++)
                            {
                                using (new GUILayout.HorizontalScope(GetButtonStyle()))
                                {
                                    GUILayout.Label(_fingerTexts[i], GUILayout.Width(50));
                                    _meshNailObjects[(int)eHand.Left, i] = EditorGUILayout.ObjectField(_meshNailObjects[(int)eHand.Left, i], typeof(Transform), true) as Transform;
                                }
                            }
                        }

                        //右手用フィールド
                        using (new GUILayout.VerticalScope(GetButtonStyle()))
                        {
                            GUILayout.Label("右手");
                            for (int i = 0; i < 5; i++)
                            {
                                using (new GUILayout.HorizontalScope(GetButtonStyle()))
                                {
                                    GUILayout.Label(_fingerTexts[i], GUILayout.Width(50));
                                    _meshNailObjects[(int)eHand.Right, i] = EditorGUILayout.ObjectField(_meshNailObjects[(int)eHand.Right, i], typeof(Transform), true) as Transform;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool DoSkinnedNail()
        {
            if (_nailTransform == null)
            {
                EditorUtility.DisplayDialog("Nail Setup Tool", "ネイルオブジェクトがセットされてません！\nネイルのオブジェクトをツールに入れてからネイルをつけてください！", "OK");
                return false;
            }

            if (_avatarAnimator == null)
            {
                EditorUtility.DisplayDialog("Nail Setup Tool", "ネイルをつけるアバターがセットされてません！\nアバターをツールに入れてからネイルをつけてください！", "OK");
                return false;
            }

            //アバターとネイルのバックアップをとる
            var copyAvatar = Instantiate(_avatarAnimator.gameObject);
            copyAvatar.gameObject.SetActive(false);

            //アバターとネイルのバックアップをとる
            var copyNail = Instantiate(_nailTransform.gameObject);
            copyNail.gameObject.SetActive(false);

            if (isPrefab(_nailTransform.gameObject))
                PrefabUtility.UnpackPrefabInstance(_nailTransform.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);

            //ネイルのオブジェクトをアバターの中に入れる
            _nailTransform.SetParent(_avatarAnimator.transform);

            //左手を入れる
            if (_avatarHandTransforms[(int)eHand.Left] != null && _nailHandTransforms[(int)eHand.Left] != null)
                _nailHandTransforms[(int)eHand.Left].SetParent(_avatarHandTransforms[(int)eHand.Left]);

            //右手を入れる
            if (_avatarHandTransforms[(int)eHand.Right] != null && _nailHandTransforms[(int)eHand.Right] != null)
                _nailHandTransforms[(int)eHand.Right].SetParent(_avatarHandTransforms[(int)eHand.Right]);

            //取得したネイルのボーンをアバターの指に入れる
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (_leftFingerTransforms[j, i] == null || _nailLeftTransforms[j, i] == null) continue;

                    Undo.RecordObject(_nailLeftTransforms[j, i], $"SetParent Nail {_nailLeftTransforms[j, i].name}");
                    _nailLeftTransforms[j, i].SetParent(_leftFingerTransforms[j, i]);
                }
            }

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (_rightFingerTransforms[j, i] == null || _nailRightTransforms[j, i] == null) continue;

                    Undo.RecordObject(_rightFingerTransforms[j, i], $"SetParent Nail {_rightFingerTransforms[j, i].name}");
                    _nailRightTransforms[j, i].SetParent(_rightFingerTransforms[j, i]);
                }
            }

            return true;
        }

        private bool DoMeshNail()
        {
            if (_avatarAnimator == null)
            {
                EditorUtility.DisplayDialog("Nail Setup Tool", "ネイルをつけるアバターがセットされてません！\nアバターをツールに入れてからネイルをつけてください！", "OK");
                return false;
            }

            //アバターのバックアップをとる
            var copyAvatar = Instantiate(_avatarAnimator.gameObject);
            copyAvatar.gameObject.SetActive(false);

            for (int i = 0; i < 5; i++)
            {
                if (_meshNailObjects[0, i] == null || _leftFingerTransforms[(int)eJoint.Distal, i] == null) continue;

                if (isPrefab(_meshNailObjects[0, i].transform.parent.gameObject))
                    PrefabUtility.UnpackPrefabInstance(_meshNailObjects[0, i].transform.parent.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);

                _meshNailObjects[0, i].SetParent(_leftFingerTransforms[(int)eJoint.Distal, i]);
            }

            for (int i = 0; i < 5; i++)
            {
                if (_meshNailObjects[1, i] == null || _rightFingerTransforms[(int)eJoint.Distal, i] == null) continue;

                if (isPrefab(_meshNailObjects[1, i].transform.parent.gameObject))
                    PrefabUtility.UnpackPrefabInstance(_meshNailObjects[1, i].transform.parent.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);

                _meshNailObjects[1, i].SetParent(_rightFingerTransforms[(int)eJoint.Distal, i]);
            }

            return true;
        }

        private void OnGUI()
        {
            #region タイトル
            GUI.Label(titlePosition, new GUIContent("Nail Setup Tool"), GetTitleLabelStyle());
            #endregion

            #region リンク
            if (GUI.Button(new Rect(500, 2, 100, 43), "小物装着ツール"))
            {
                var window = GetWindow<BoneParentTool>();
                window.titleContent = new GUIContent("小物装着ツール");
                window.minSize = window.maxSize = new Vector2(650, 180);
                window.Show();
            }

            DrawLinkedButton(GetBoothLogo(), "https://saturnianshop.booth.pm/", new Rect(605, 2, 43, 43));
            #endregion

            GUILayout.Space(90);

            #region アバターを入れるフィールド
            EditorGUI.BeginChangeCheck();
            _avatarAnimator = EditorGUILayout.ObjectField("アバター", _avatarAnimator, typeof(Animator), true) as Animator;
            if (EditorGUI.EndChangeCheck())
            {
                //アバターのフィールドが変更された場合に配列をリセット
                _leftFingerTransforms = new Transform[3, 5];
                _rightFingerTransforms = new Transform[3, 5];

                _avatarHandTransforms[(int)eHand.Left] = null;
                _avatarHandTransforms[(int)eHand.Right] = null;

                //中身がある場合は再度指のボーンを取得
                if (_avatarAnimator != null)
                {
                    if (!_avatarAnimator.gameObject.scene.IsValid())
                    {
                        _avatarAnimator = null;
                        EditorUtility.DisplayDialog("Nail Setup Tool", "アバターがシーンに存在しません。\nシーンに配置してからツールに入れてください。", "はい");
                        return;
                    }

                    GetFingerBones(_avatarAnimator);
                }
            }
            #endregion

            #region アバターのボーンを表示するフィールド
            using (new GUILayout.VerticalScope(GetButtonStyle()))
            {
                isExtendBoneField = EditorGUILayout.ToggleLeft("ボーン詳細設定", isExtendBoneField);
         
                if (isExtendBoneField)
                {
                    _avatarBoneScrollPosition = GUILayout.BeginScrollView(_avatarBoneScrollPosition, GUILayout.Height(150));

                    //GUILayout.Label("左手");
                    _avatarHandTransforms[(int)eHand.Left] = EditorGUILayout.ObjectField("左手", _avatarHandTransforms[(int)eHand.Left], typeof(Transform), true) as Transform;
                    for (int i = 0; i < 5; i++)
                    {
                        using (new GUILayout.HorizontalScope(GUI.skin.button))
                        {
                            GUILayout.Label($"{_fingerTexts[i]}", GUILayout.Width(90));
                            for (int j = 0; j < 3; j++)
                            {
                                EditorGUILayout.ObjectField(_leftFingerTransforms[j, i], typeof(Transform), true);
                            }
                        }
                    }

                    GUILayout.Space(20);

                    _avatarHandTransforms[(int)eHand.Right] = EditorGUILayout.ObjectField("右手", _avatarHandTransforms[(int)eHand.Right], typeof(Transform), true) as Transform;
                    for (int i = 0; i < 5; i++)
                    {
                        using (new GUILayout.HorizontalScope(GUI.skin.button))
                        {
                            GUILayout.Label($"{_fingerTexts[i]}", GUILayout.Width(90));
                            for (int j = 0; j < 3; j++)
                            {
                                EditorGUILayout.ObjectField(_rightFingerTransforms[j, i], typeof(Transform), true);
                            }
                        }
                    }

                    GUILayout.EndScrollView();
                }
            }
            #endregion

            GUILayout.Space(10);

            isSkinnedNail = EditorGUILayout.ToggleLeft("スキニング済みのネイル", isSkinnedNail);

            #region ネイルのリストを表示するフィールド
            if (isSkinnedNail)
                DrawSkinnedNailField();
            else
                DrawMeshNailField();
            #endregion

            GUILayout.Space(10);

            #region ネイルをつけるボタン
            if (GUILayout.Button("ネイルをつける", GUILayout.Height(50)))
            {
                if (isSkinnedNail)
                {
                    if (DoSkinnedNail())
                        EditorUtility.DisplayDialog("Nail Setup Tool", "ネイルをつけました！", "はい");
                }
                else
                {
                    if (DoMeshNail())
                        EditorUtility.DisplayDialog("Nail Setup Tool", "ネイルをつけました！", "はい");
                }
            }
            #endregion
        }
        #endregion

        #region デバッグ用
        public void DrawSlider(ref Rect rect, float min, float max)
        {
            rect.x = EditorGUILayout.Slider(rect.x, min, max);
            rect.y = EditorGUILayout.Slider(rect.y, min, max);
            rect.width = EditorGUILayout.Slider(rect.width, min, max);
            rect.height = EditorGUILayout.Slider(rect.height, min, max);
        }
        #endregion
    }
}

