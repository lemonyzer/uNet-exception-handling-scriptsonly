#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

using UnityEditorInternal;
using System;
using System.Reflection;

namespace UnityEnhancements
{

    public class EditorTools {

        // Get the sorting layer names
        //int popupMenuIndex;//The selected GUI popup Index
        static public string[] GetSortingLayerNames()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            string[] sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
            //		foreach (string layer in sortingLayers)
            //		{
            //			Debug.Log(layer);
            //		}
            return sortingLayers;
        }

        // Get the unique sorting layer IDs -- tossed this in for good measure
        static public int[] GetSortingLayerUniqueIDs()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
            int[] sortingLayersUniqueIDs = (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
            //		foreach (int layerId in sortingLayersUniqueIDs)
            //		{
            //			Debug.Log(layerId);
            //		}
            return sortingLayersUniqueIDs;
        }

        static public string GetSortingLayerName(int sortingLayerID, int[] sortingLayersUniqueIDs, string[] sortingLayerNames)
        {
            if (sortingLayersUniqueIDs.Length != sortingLayerNames.Length)
                return "Default";

            for (int i = 0; i < sortingLayersUniqueIDs.Length; i++) //here we initialize our popupMenuIndex with the current Sort Layer Name
            {
                if (sortingLayersUniqueIDs[i] == sortingLayerID)
                    return sortingLayerNames[i];
            }
            Debug.LogError("Sorting Layer " + sortingLayerID + " nicht gefunden");
            return "Default";
        }

        static public int GetSortingLayerNumber(string sortingLayerName, int[] sortingLayersUniqueIDs, string[] sortingLayerNames)
        {
            if (sortingLayersUniqueIDs.Length != sortingLayerNames.Length)
                return 0;

            for (int i = 0; i < sortingLayerNames.Length; i++) //here we initialize our popupMenuIndex with the current Sort Layer Name
            {
                if (sortingLayerNames[i] == sortingLayerName)
                    return sortingLayersUniqueIDs[i];
            }
            Debug.LogError("Sorting Layer " + sortingLayerName + " nicht gefunden");
            return 0;
        }

    }
}
#endif