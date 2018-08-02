using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class NullReferenceTest : IPreprocessBuild, IProcessScene {
    private readonly HashSet<Object> m_checkedObjects;

    public NullReferenceTest() {
        m_checkedObjects = new HashSet<Object>();
    }

    int IOrderedCallback.callbackOrder {
        get {
            return 0;
        }
    }

    [MenuItem("Criminal Town/Check Null References In Resources Folders")]
    private static void CheckNullReferencesInResourcesFoldersMenu() {
        NullReferenceTest nullReferenceTest = new NullReferenceTest();
        nullReferenceTest.CheckNullReferencesInResourcesFolders();
    }

    [MenuItem("Criminal Town/Check Null References In Current Scene")]
    private static void CheckNullReferencesInCurrentSceneMenu() {
        NullReferenceTest nullReferenceTest = new NullReferenceTest();
        nullReferenceTest.CheckNullReferencesInCurrentScene();
    }

    public void OnPreprocessBuild(BuildTarget target, string path) {
        CheckNullReferencesInResourcesFolders();
    }

    public void OnProcessScene(Scene scene) {
        if (EditorApplication.isPlaying) {
            return;
        }
        CheckNullReferencesInCurrentScene();
    }

    public void CheckNullReferencesInCurrentScene() {
        EditorUtility.DisplayProgressBar("NullReferenceTest", "Finding objects in " + SceneManager.GetActiveScene().name + "...", 0f);
        try {
            GameObject[] objects = SceneManager.GetActiveScene().GetRootGameObjects();
            if (objects.Length == 0) {
                return;
            }
            float progressDelta = 1f / objects.Length;
            float progress = 0f;

            foreach (GameObject obj in objects) {
                EditorUtility.DisplayProgressBar("NullReferenceTest", "Finding null references in " + SceneManager.GetActiveScene().name + "...", progress);
                RecursiveFindComponentsAndCheckNullReference(obj.transform);
                progress += progressDelta;
            }
        } finally {
            EditorUtility.ClearProgressBar();
        }
    }

    public void CheckNullReferencesInResourcesFolders() {
        EditorUtility.DisplayProgressBar("NullReferenceTest", "Finding objects in resources folders...", 0f);
        try {
            GameObject[] gameObjects = Resources.LoadAll<GameObject>("");
            ScriptableObject[] scriptableObjects = Resources.LoadAll<ScriptableObject>("");

            if (gameObjects.Length == 0 && scriptableObjects.Length == 0) {
                return;
            }
            float progressDelta = 1f / (gameObjects.Length + scriptableObjects.Length);
            float progress = 0f;

            foreach (GameObject gameObject in gameObjects) {
                EditorUtility.DisplayProgressBar("NullReferenceTest", "Finding null references in resources folders...", progress);
                progress += progressDelta;
                RecursiveFindComponentsAndCheckNullReference((gameObject).transform);
            }
            foreach (ScriptableObject scriptableObject in scriptableObjects) {
                EditorUtility.DisplayProgressBar("NullReferenceTest", "Finding null references in resources folders...", progress);
                progress += progressDelta;
                CheckSingleObject(scriptableObject, scriptableObject, null, scriptableObject.GetType().Name, false);
            }
        } finally {
            EditorUtility.ClearProgressBar();
        }
    }

    private void RecursiveFindComponentsAndCheckNullReference([NotNull] Transform transform) {
        foreach (Component component in transform.GetComponents<Component>()) {
            //if (IsGearGamesObject(component)) {
                CheckSingleObject(component, transform.gameObject, "GameObject", "Component: " + component.GetType().Name, false);
            //}
        }
        foreach (Transform t in transform) {
            RecursiveFindComponentsAndCheckNullReference(t);
        }
    }

    private void CheckSingleObject([CanBeNull] object obj, [NotNull] Object unityObjReference, [CanBeNull] string containingClassName, [NotNull] string objectName, bool isCanBeNull) {
        if (IsObjectNull(obj)) {
            if (!isCanBeNull) {
                Debug.LogError("NullReference in serialized field: " + GetPathToObject(unityObjReference) +
                    " Class: " + containingClassName + " Field: " + objectName, unityObjReference);
            }
            return;
        }
        if (obj is Object) {
            if (!m_checkedObjects.Add((Object) obj)) {
                return;
            }
        }
        if (obj is GameObject) {
            RecursiveFindComponentsAndCheckNullReference(((GameObject) obj).transform);
            return;
        }
//        if (!IsGearGamesObject(obj)) {
//            return;
//        }

        List<FieldInfo> fieldInfos = GetObjectiveSerializeNotNullFields(obj);
        if (fieldInfos == null) {
            return;
        }
        foreach (FieldInfo fieldInfo in fieldInfos) {
            if (IsCollectionType(fieldInfo.FieldType)) {
                if (IsSerializableCollectionType(fieldInfo.FieldType)) {
                    CheckCollection((ICollection) fieldInfo.GetValue(obj), obj is MonoBehaviour ? ((MonoBehaviour) obj).gameObject : unityObjReference, obj.GetType().ToString(),
                        fieldInfo.Name, IsItemInCollectionCanBeNull(fieldInfo));
                }
            } else {
                CheckSingleObject(fieldInfo.GetValue(obj), obj is MonoBehaviour ? ((MonoBehaviour) obj).gameObject : unityObjReference,
                    obj.GetType().ToString(), fieldInfo.Name, IsFieldCanBeNull(fieldInfo));
            }
        }
    }

    private void CheckCollection([CanBeNull] ICollection collection, [NotNull] Object unityObjReference, [NotNull] string containingClassName, [NotNull] string collectionName, bool isItemInCollectionCanBeNull) {
        if (collection == null) {
            return;
        }
        if (collection.Count == 0) {
            return;
        }

        Type type = collection.GetType();
        Type elementType = null;
        Assert.IsTrue(type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>), "Checkable collection must be List or Array");

        if (type.IsArray) {
            elementType = type.GetElementType();
        } else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
            elementType = type.GetGenericArguments().Single();
        }
        if (elementType == null) {
            return;
        }
        if (elementType.IsPrimitive) {
            return;
        }

        int i = 0;
        foreach (object item in collection) {
            CheckSingleObject(item, unityObjReference, containingClassName, collectionName + "[" + i + "]", isItemInCollectionCanBeNull);
            i++;
        }
    }

    [Pure, ContractAnnotation("=>false, obj:notnull; =>true, obj:canbenull")]
    private static bool IsObjectNull([CanBeNull] object obj) {
        if (obj == null) {
            return true;
        }
        Object unityObject = obj as Object;
        return (object) unityObject != null && unityObject.Equals(null);
    }

    [Pure]
    private static bool IsGearGamesObject([NotNull] object obj) {
        string ns = obj.GetType().Namespace;
        return ns != null && ns.StartsWith("com.geargames");
    }

    [CanBeNull]
    private static List<FieldInfo> GetObjectiveSerializeNotNullFields([NotNull] object obj) {
        FieldInfo[] fieldInfos = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        return fieldInfos.Where(fieldInfo => IsFieldObjectiveSerializable(fieldInfo)).ToList();
    }

    [Pure]
    private static bool IsFieldObjectiveSerializable([NotNull] FieldInfo fieldInfo) {
        if (fieldInfo.IsDefined(typeof(NonSerializedAttribute), false)) {
            return false;
        }
        if (fieldInfo.FieldType.IsPrimitive) {
            return false;
        }
        if (fieldInfo.FieldType.IsSubclassOf(typeof(Delegate))) {
            return false;
        }
        if ((fieldInfo.FieldType.IsSubclassOf(typeof(Object)) || fieldInfo.FieldType.IsSerializable) && (fieldInfo.IsPublic || fieldInfo.IsDefined(typeof(SerializeField), false))) {
            return true;
        }
        return false;
    }

    [Pure]
    private static bool IsFieldCanBeNull([NotNull] FieldInfo fieldInfo) {
        return fieldInfo.IsDefined(typeof(CanBeNullAttribute), false);
    }

    [Pure]
    private static bool IsItemInCollectionCanBeNull([NotNull] FieldInfo fieldInfo) {
        //todo: Find out where is ItemCanBeNull attribute
        return false;
        //return fieldInfo.IsDefined(typeof(ItemCanBeNullAttribute), false);
    }

    [Pure]
    private static bool IsCollectionType([NotNull] Type type) {
        return typeof(ICollection).IsAssignableFrom(type);
    }

    [Pure]
    private static bool IsSerializableCollectionType([NotNull] Type type) {
        Type elementType;
        if (type.IsArray) {
            elementType = type.GetElementType();
        } else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
            elementType = type.GetGenericArguments().Single();
        } else {
            return false;
        }
        if (elementType == null) {
            Debug.LogError("Can not get an element type of collection");
            return false;
        }
        return (elementType.IsSerializable || elementType.IsSubclassOf(typeof(Object)));
    }

    [NotNull]
    private static string GetPathToObject([NotNull] Object obj) {
        if (obj is GameObject) {
            GameObject gameObject = (GameObject) obj;
            string path = "/" + gameObject.name;
            while (gameObject.transform.parent != null) {
                gameObject = gameObject.transform.parent.gameObject;
                path = "/" + gameObject.name + path;
            }

            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath) == false) {
                path = assetPath + path;
            } else {
                path = gameObject.scene.name + path;
            }
            return path;
        }
        if (obj is ScriptableObject) {
            return AssetDatabase.GetAssetPath(obj);
        }
        return "";
    }
}