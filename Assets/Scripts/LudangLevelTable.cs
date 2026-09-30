
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LudangLevelTable", menuName = "Scriptable Objects/LudangLevelTable")]
public class LudangLevelTable : ScriptableObject
{

    [Serializable]
    public class LudangLevel
    {

        public string displayName;

        public Sprite sprite;

        [Min(0.1f)]
        public float size = 1f;

        public int mergeScore = 10;

    }

    public LudangLevel[] levels;

    public int Count
    {

        get
        {

            return levels != null ? levels.Length : 0;

        }

    }

    public LudangLevel Get(int index)
    {

        if (levels == null || levels.Length == 0)
        {

            return null;

        }

        index = Mathf.Clamp(index, 0, levels.Length - 1);

        return levels[index];

    }

}

