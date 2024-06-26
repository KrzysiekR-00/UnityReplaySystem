using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ComponentsRemover
{
    internal static class Extensions
    {
        internal static IEnumerable<Component> GetDependentComponents(this Component component)
        {
            var componentsOfGameObject = component.gameObject.GetComponents<Component>();

            var componentType = component.GetType();

            for (int i = 0; i < componentsOfGameObject.Length; i++)
            {
                if (componentsOfGameObject[i] == null) continue;

                if (componentsOfGameObject[i].RequiresComponentOfType(componentType)) yield return componentsOfGameObject[i];
            }
        }

        private static bool RequiresComponentOfType(this Component component, Type type)
        {
            var attributes = component.GetType().GetCustomAttributes(true).OfType<RequireComponent>();

            var requires = attributes.Any(a =>
                a.m_Type0?.IsAssignableFrom(type) == true ||
                a.m_Type1?.IsAssignableFrom(type) == true ||
                a.m_Type2?.IsAssignableFrom(type) == true
            );

            return requires;
        }
    }
}