using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    // Custom attribute and drawer to access to interface via an inspector view.
    // https://qiita.com/Teach/items/54769db9bb4ab5d7ce79

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ComponentRestrictionAttribute : PropertyAttribute
    {
        public Type type;
        public ComponentRestrictionAttribute(Type type)
        {
            this.type = type;
        }
    }
}
