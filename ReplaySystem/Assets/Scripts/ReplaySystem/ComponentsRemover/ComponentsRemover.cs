using System;
using System.Linq;
using UnityEngine;

namespace ComponentsRemover
{
    public class ComponentsRemover : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("GameObject from which components will be removed")]
        private GameObject _rootGameObject;

        private readonly Type[] _typesToAlwaysIgnore = { typeof(Transform), typeof(ComponentsRemover), typeof(ComponentsTypesToIgnore) };

        public void Remove()
        {
            DestroyOrDisableComponents();
        }

        private void DestroyOrDisableComponents()
        {
            var typesToIgnore = GetIgnoredTypes();

            var components = _rootGameObject.GetComponentsInChildren<Component>(true);

            foreach (var component in components)
            {
                if (component == null) continue;

                RemoveComponentAndDependencies(component, typesToIgnore);
            }
        }

        private Type[] GetIgnoredTypes()
        {
            Type[] types = _typesToAlwaysIgnore;

            var ignoredTypesComponents = GetComponents<ComponentsTypesToIgnore>();

            foreach (var ignoredTypesComponent in ignoredTypesComponents)
            {
                types = types.Concat(ignoredTypesComponent.Types).ToArray();
            }

            return types;
        }

        private void RemoveComponentAndDependencies(Component componentToDestroy, Type[] ignoredTypes)
        {
            if (ShouldComponentBeIgnored(componentToDestroy, ignoredTypes)) return;

            var requiredComponents = componentToDestroy.GetDependentComponents().ToArray();

            foreach (var component in requiredComponents)
            {
                if (component == null) continue;
                RemoveComponentAndDependencies(component, ignoredTypes);
            }

            TryDestroyOrDisable(componentToDestroy);
        }

        private bool ShouldComponentBeIgnored(Component component, Type[] ignoredTypes)
        {
            var componentType = component.GetType();
            return ignoredTypes.Any(t => componentType.IsAssignableFrom(t) || componentType.IsSubclassOf(t));
        }

        private void TryDestroyOrDisable(Component component)
        {
            try
            {
                Destroy(component);
            }
            catch (Exception e)
            {
                if (component is Behaviour behaviour) behaviour.enabled = false;
                else LogExceptionMessage(component, e.Message);
            }
        }

        private void LogExceptionMessage(Component component, string exceptionMessage)
        {
            var message = string.Format(
                $"Cannot destroy nor deactivate component {0} of {1} gameObject. Exception: {2}",
                component,
                component.gameObject,
                exceptionMessage);

            Debug.LogError(message);
        }
    }
}