using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace MetaMall.Editor
{
    public static class ScriptTemplateMenu
    {
        const int BasePriority = -20;

        [MenuItem("Assets/New Script/C# Class", false, BasePriority)]
        static void CreateClass()
        {
            CreateScript("NewClass.cs",
                "using System;\n\npublic class #SCRIPTNAME#\n{\n}\n");
        }

        [MenuItem("Assets/New Script/MonoBehaviour", false, BasePriority + 1)]
        static void CreateMono()
        {
            CreateScript("NewMonoBehaviour.cs",
                "using UnityEngine;\n\npublic class #SCRIPTNAME# : MonoBehaviour\n{\n}\n");
        }

        [MenuItem("Assets/New Script/Enum", false, BasePriority + 2)]
        static void CreateEnum()
        {
            CreateScript("NewEnum.cs",
                "public enum #SCRIPTNAME#\n{\n}\n");
        }

        [MenuItem("Assets/New Script/Interface", false, BasePriority + 3)]
        static void CreateInterface()
        {
            CreateScript("NewInterface.cs",
                "public interface #SCRIPTNAME#\n{\n}\n");
        }

        static void CreateScript(string defaultName, string template)
        {
            var tmpPath = Path.Combine(Application.temporaryCachePath, "ScriptTemplate.cs.txt");
            File.WriteAllText(tmpPath, template, Encoding.UTF8);

            var icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<DoCreateScript>(),
                defaultName,
                icon,
                tmpPath);
        }
    }

    internal class DoCreateScript : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var fileName = Path.GetFileNameWithoutExtension(pathName);
            var template = File.ReadAllText(resourceFile, Encoding.UTF8);
            var content = template.Replace("#SCRIPTNAME#", fileName);

            File.WriteAllText(pathName, content, Encoding.UTF8);
            AssetDatabase.ImportAsset(pathName);

            var asset = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}
