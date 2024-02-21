using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomLanguages
{
    public static class Extensions
    {
        public static T Clone<T>(this T component, string name) where T : Component
            => Clone(component.gameObject, name).GetComponent<T>();

        public static GameObject Clone(this GameObject go, string name)
        {
            var goT = go.transform;
            var clone = UnityEngine.Object.Instantiate(go, goT.parent);
            clone.name = name;
            var cloneT = clone.transform;
            cloneT.localPosition = goT.localPosition;
            cloneT.localEulerAngles = goT.localEulerAngles;
            cloneT.localScale = goT.localScale;
            return clone;
        }
    }
}
