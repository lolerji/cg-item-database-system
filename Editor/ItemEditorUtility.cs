using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.AddressableAssets;

namespace CG.ItemDatabaseSystem.Editor
{
    public static class ItemEditorUtility
    {
        public static async void CreateItemIdFile()
        {
            var load = Addressables.LoadResourceLocationsAsync("Items");
            var locations = await load.Task;
            var ids = locations.Select(l => l.PrimaryKey);

            var ccu = new CodeCompileUnit();
            var cns = new CodeNamespace("CG.ItemDatabaseSystem");
            var ctd = new CodeTypeDeclaration("Items");

            ctd.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            foreach (var id in ids)
            {
                var field = new CodeMemberField();
                
                field.Name = id.Replace("Items/", "");
                field.InitExpression = new CodePrimitiveExpression(id);
                field.Type = new CodeTypeReference(typeof(string));
                field.Attributes = MemberAttributes.Const | MemberAttributes.Public;

                ctd.Members.Add(field);
            }

            cns.Types.Add(ctd);
            ccu.Namespaces.Add(cns);

            GenerateCSharpCode(ccu, "Assets/Clone Games/Code/Items.cs");
            AssetDatabase.Refresh();
        }
        
        private static void GenerateCSharpCode(CodeCompileUnit ccu, string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "CSharp";

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            using var stw = new StreamWriter(fileName);
            provider.GenerateCodeFromCompileUnit(ccu, stw, options);
        }
    }
}