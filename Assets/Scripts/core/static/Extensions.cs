using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.IO;
//using Newtonsoft.Json.Linq;

public static class Extensions
{
    public static float ToFloat(this double x)
    {
        return (float) x;
    }

    public static Vector3 setZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 setY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 setX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static void setZ(this Transform t, float z, bool world = false)
    {
        if (world)
        {
            t.position = t.position.setZ(z);
        }
        else
        {
            t.localPosition = t.position.setZ(z);
        }
    }

    public static void setY(this Transform t, float y, bool world = false)
    {
        if (world)
        {
            t.position = t.position.setY(y);
        }
        else
        {
            t.localPosition = t.position.setY(y);
        }
    }

    public static void setX(this Transform t, float x, bool world = false)
    {
        if (world)
        {
            t.position = t.position.setX(x);
        }
        else
        {
            t.localPosition = t.position.setX(x);
        }
    }

    public static Transform resetPRS(this Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        t.localRotation = Quaternion.identity;

        return t;
    }

    public static GameObject loadPrefab(string prefabName)
    {
        var prefab = Resources.Load(prefabName) as GameObject;
        if (prefab == null)
        {
            Debug.LogError("Cannot load prefab '" + prefabName + "'!");
            return null;
        }

        return prefab;
    }

    public static Transform addChildResetPRS(this Transform parent, Transform child)
    {
        child.SetParent(parent);

        return child.resetPRS();
    }

    public static T addChildResetPRS<T>(this Transform parent, T child)
        where T : Component
    {
        parent.addChildResetPRS(child.transform);
        return child;
    }

    public static T addChildResetPRS<T>(this Component parent, T child)
        where T : Component
    {
        parent.transform.addChildResetPRS(child.transform);
        return child;
    }

    public static T addChildResetPRS<T>(this GameObject parent, T child)
        where T : Component
    {
        parent.transform.addChildResetPRS(child.transform);
        return child;
    }

    public static GameObject addChildResetPRS(this GameObject parent, GameObject child)
    {
        parent.transform.addChildResetPRS(child.transform);
        return child;
    }

    public static GameObject addChildResetPRS(this Transform parent, GameObject child)
    {
        parent.addChildResetPRS(child.transform);
        return child;
    }

    public static GameObject instantiateChild(this GameObject _this, GameObject prefab, bool alignToParent = false)
    {
        return _this.transform.instantiateChild(prefab, alignToParent);
    }

    public static GameObject instantiateChild(this Transform _this, GameObject prefab, bool alignToParent = false)
    {
        var obj = GameObject.Instantiate(prefab);

        if (alignToParent)
        {
            obj.transform.SetParent(_this.parent);
            obj.transform.alignAs(_this);
        }
        else
        {
            _this.addChildResetPRS(obj);
        }

        return obj;
    }

    public static void alignAs(this GameObject obj, GameObject other)
    {
        obj.transform.alignAs(other.transform);
    }

    public static void alignAs(this Transform obj, Transform other)
    {
        obj.localPosition = other.localPosition;
        obj.localRotation = other.localRotation;
        obj.localScale = other.localScale;
    }

    public static Color setA(this Color color, float a)
    {
        color.a = a;
        return color;
    }

    public static T GetComponentInChildren<T>(this Component component, bool includeInactive)
        where T : Component
    {
        if (!includeInactive)
        {
            return component.GetComponentInChildren<T>();
        }

        var components = component.GetComponentsInChildren<T>();
        if (components == null || components.Length == 0)
        {
            return null;
        }

        return components[0];
    }

    public static Quaternion dirToRotation2D(this Vector2 dir)
    {
        var angle = Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg;
        return Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public static Quaternion dirToRotation2D(this Vector3 dir)
    {
        var angle = Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg;
        return Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public static Quaternion dirToRotationX0Z(this Vector3 dir)
    {
        var angle = Mathf.Atan2(dir.z, dir.x)*Mathf.Rad2Deg;
        return Quaternion.Euler(new Vector3(0, -angle, 0));
    }

    public static float dirToAngleDegX0Z(this Vector3 dir)
    {
        return -Mathf.Atan2(dir.z, dir.x)*Mathf.Rad2Deg;
    }

    public static void setRandomRotation(this Transform t, Vector3 axis)
    {
        t.Rotate(axis, Random.Range(0f, 360f));
    }

    public static int numDigits(this int number)
    {
        int digits = 1, pten = 10;
        while (pten <= number)
        {
            ++digits;
            pten *= 10;
        }
        return digits;
    }

    public static void destroyAllChildren(this Transform t)
    {
        var cnt = t.childCount;
        for (var i = 0; i < cnt; ++i)
        {
            Object.Destroy(t.GetChild(i).gameObject);
        }
    }

	public static void scanFolder(string f)
	{
		Debug.Log("Folder: " + f);
		
		var txtFiles = Directory.GetFiles(f);
		foreach (string currentFile in txtFiles) {
			Debug.Log("File: " + currentFile);
		}
		
		string[] subs = Directory.GetDirectories(f);
		foreach(string sub in subs)
			scanFolder(sub);
	}

    public static void setLayerRecursively(this GameObject go, int layerNumber)
    {
        var transs = go.GetComponentsInChildren<Transform>(true);
        foreach (var trans in transs)
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    #region Array

    public static bool Exists<T>(this T[] array, Predicate<T> match)
    {
        return Array.Exists(array, match);
    }

    public static T Find<T>(this T[] array, Predicate<T> match)
    {
        return Array.Find(array, match);
    }

    public static T[] FindAll<T>(this T[] array, Predicate<T> match)
    {
        return Array.FindAll(array, match);
    }

    public static int IndexOf(this Array array, object value)
    {
        return Array.IndexOf(array, value);
    }

    public static int IndexOf(this Array array, object value, int startIndex)
    {
        return Array.IndexOf(array, value, startIndex);
    }

    public static int IndexOf(this Array array, object value, int startIndex, int count)
    {
        return Array.IndexOf(array, value, startIndex, count);
    }

    public static int IndexOf<T>(this T[] array, T value)
    {
        return Array.IndexOf(array, value);
    }

    public static int IndexOf<T>(this T[] array, T value, int startIndex)
    {
        return Array.IndexOf(array, value, startIndex);
    }

    public static int IndexOf<T>(this T[] array, T value, int startIndex, int count)
    {
        return Array.IndexOf(array, value, startIndex, count);
    }

    public static int LastIndexOf(this Array array, object value)
    {
        return Array.LastIndexOf(array, value);
    }

    public static int LastIndexOf(this Array array, object value, int startIndex)
    {
        return Array.LastIndexOf(array, value, startIndex);
    }

    public static int LastIndexOf(this Array array, object value, int startIndex, int count)
    {
        return Array.LastIndexOf(array, value, startIndex, count);
    }

    public static int LastIndexOf<T>(this T[] array, T value)
    {
        return Array.LastIndexOf(array, value);
    }

    public static int LastIndexOf<T>(this T[] array, T value, int startIndex)
    {
        return Array.LastIndexOf(array, value, startIndex);
    }

    public static int LastIndexOf<T>(this T[] array, T value, int startIndex, int count)
    {
        return Array.LastIndexOf(array, value, startIndex, count);
    }

    public static void Reverse(this Array array)
    {
        Array.Reverse(array);
    }

    public static void Reverse(this Array array, int index, int length)
    {
        Array.Reverse(array, index, length);
    }

    #endregion //Array

    #region iTween

    //public static void putOnPath(this Transform t, Transform[] path, float percent)
    //{
    //    iTween.PutOnPath(t, path, percent);
    //}

    //public static void putOnPath(this Transform t, Vector3[] path, float percent)
    //{
    //    iTween.PutOnPath(t, path, percent);
    //}

    //public static void putOnPath(this GameObject g, Transform[] path, float percent)
    //{
    //    iTween.PutOnPath(g, path, percent);
    //}

    //public static void putOnPath(this GameObject g, Vector3[] path, float percent)
    //{
    //    iTween.PutOnPath(g, path, percent);
    //}

    #endregion

    #region Newtonsoft.Json

    //public static T ToObjectOrDefault<T>(this JToken token)
    //{
    //    try
    //    {
    //        return token.ToObject<T>();
    //    }
    //    catch (Exception)
    //    {
    //        return default(T);
    //    }
    //}

    #endregion
}