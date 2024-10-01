using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<SerializableKeyValuePair> list = new List<SerializableKeyValuePair>();

    [Serializable]
    public struct SerializableKeyValuePair
    {
        public TKey Key;
        public TValue Value;

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public void SetValue(TValue value)
        {
            Value = value;
        }
    }

    private Dictionary<TKey, uint> KeyPositions => _keyPositions.Value;
    private Lazy<Dictionary<TKey, uint>> _keyPositions;

    public SerializableDictionary()
    {
        _keyPositions = new Lazy<Dictionary<TKey, uint>>(MakeKeyPositions);
    }

    public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
    {
        _keyPositions = new Lazy<Dictionary<TKey, uint>>(MakeKeyPositions);

        if (dictionary == null)
        {
            throw new ArgumentException("The passed dictionary is null.");
        }

        foreach (KeyValuePair<TKey, TValue> pair in dictionary)
        {
            Add(pair.Key, pair.Value);
        }
    }

    private Dictionary<TKey, uint> MakeKeyPositions()
    {
        int numEntries = list.Count;

        Dictionary<TKey, uint> result = new Dictionary<TKey, uint>(numEntries);

        for (int i = 0; i < numEntries; ++i)
        {
            result[list[i].Key] = (uint) i;
        }

        return result;
    }

    public void OnBeforeSerialize()
    {
        list.Clear();
        foreach(KeyValuePair<TKey, TValue> pair in this)
        {
            list.Add(new SerializableKeyValuePair(pair.Key, pair.Value));
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        if(list.Count == 0)
            return;

        if (list.Select(x => x.Key).Distinct().Count() != list.Count)
            throw new System.Exception(string.Format("There are duplicate keys in the deserialized dictionary for type {0}.", typeof(TKey)));

        foreach (SerializableKeyValuePair pair in list)
        {
            this.Add(pair.Key, pair.Value);
        }

        _keyPositions = new Lazy<Dictionary<TKey, uint>>(MakeKeyPositions);
    }

    #region IDictionary
    public new TValue this[TKey key]
    {
        get => list[(int) KeyPositions[key]].Value;
        set
        {
            if (KeyPositions.TryGetValue(key, out uint index))
            {
                list[(int) index].SetValue(value);
            }
            else
            {
                KeyPositions[key] = (uint) list.Count;

                list.Add(new SerializableKeyValuePair(key, value));
            }
        }
    }

    public new ICollection<TKey> Keys => list.Select(tuple => tuple.Key).ToArray();
    public new ICollection<TValue> Values => list.Select(tuple => tuple.Value).ToArray();

    public new void Add(TKey key, TValue value)
    {
        if (KeyPositions.ContainsKey(key))
        {
            throw new ArgumentException("An element with the same key already exists in the dictionary.");
        }
        else
        {
            KeyPositions[key] = (uint) list.Count;

            list.Add(new SerializableKeyValuePair(key, value));
        }
    }

    public new bool ContainsKey(TKey key) => KeyPositions.ContainsKey(key);

    public bool Remove(KeyValuePair<TKey, TValue> kvp) => Remove(kvp.Key);
    #endregion
}