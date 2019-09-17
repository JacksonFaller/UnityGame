using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private TKey[] _keys;

    [SerializeField]
    private TValue[] _values;

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        _keys = new TKey[this.Count];
        _values = new TValue[this.Count];

        int i = 0;
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            _keys[i] = pair.Key;
            _values[i] = pair.Value;
            i++;
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < _keys.Length; i++)
        {
            this.Add(_keys[i], _values[i]);
        }

        //_keys = null;
        //_values = null;
    }
}