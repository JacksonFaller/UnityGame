using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class LayerMaskExtensions
{
    /// <summary>
    /// Checks if layer mask contains layer. Always false for layers with index 0
    /// </summary>
    /// <param name="layerMask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool ContainsLayer(this LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask.value) != 0;
    }
}
