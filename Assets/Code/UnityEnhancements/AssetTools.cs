using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UnityEnhancements
{
    public class AssetTools {



        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <returns><c>true</c>, if folder was created, <c>false</c> otherwise.</returns>
        /// <param name="pathRelativeToAssetsPath">Path relative to assets path. eg. Prefabs/Characters/Mario</param>
        public static bool CreateFolder(string pathRelativeToAssetsPath)
        {
            string[] pathSegments = pathRelativeToAssetsPath.Split(new char[] { '/' });
            string accumulatedUnityFolder = "Assets";                   // Startwert
            string accumulatedSystemFolder = Application.dataPath;      // Startwert C:/Users/Aryan/Documents/SuperMarioWars_UnityNetwork/Assets

            //		Debug.Log("Unity : " + accumulatedUnityFolder);
            //		Debug.Log("System: " + accumulatedSystemFolder);

            string lastExistedFolder = accumulatedUnityFolder;

            foreach (string folder in pathSegments)
            {
                accumulatedSystemFolder += "/" + folder;
                accumulatedUnityFolder += "/" + folder;

                string guidFolder = "";
                if (!System.IO.Directory.Exists(accumulatedSystemFolder))
                {
                    //Debug.LogWarning(accumulatedSystemFolder + " Ordner existiert nicht!\n" +
                    //                  accumulatedUnityFolder + " Ordner existiert nicht!");

                    //Debug.Log("parentFolder = " + lastExistedFolder + " (letzter existierender Ordner)");
                    string guidParentFolder = AssetDatabase.AssetPathToGUID(lastExistedFolder);
                    if (guidParentFolder != "")
                    {
                        //Debug.Log("guidParentFolder = " + guidParentFolder);
                        guidFolder = AssetDatabase.CreateFolder(lastExistedFolder, folder);                         // TODO  ------------ WTF ordnerangabe geht!  GUID angabe geht nicht!!!!
                        if (guidFolder != "")
                        {
                            //Debug.Log(accumulatedSystemFolder + " Ordner wurder erfolgreich erstellt! \n" +
                            //           accumulatedUnityFolder + " Ordner wurder erfolgreich erstellt! \n");
                        }
                        else
                        {
                            //Debug.LogError("Ordner " + folder + " konnte in " + lastExistedFolder + " nicht erstellt werden!");
                            //						Debug.LogError (accumulatedSystemFolder + " konnte nicht erstellt werden! \n" +
                            //						                accumulatedUnityFolder + " konnte nicht erstellt werden! \n");
                            return false;
                        }
                    }
                    else
                    {
                        //Debug.LogError(accumulatedSystemFolder + " guidParentFolder konnte nicht gefunden werden! \n" +
                        //                accumulatedUnityFolder + " guidParentFolder konnte nicht gefunden werden! \n");
                        return false;
                    }
                }
                else
                {
                    //				Debug.Log (accumulatedSystemFolder + " existiert! \n" +
                    //				           accumulatedUnityFolder + " existiert!");
                }

                lastExistedFolder = accumulatedUnityFolder;
            }
            return true;
        }


    }

}

