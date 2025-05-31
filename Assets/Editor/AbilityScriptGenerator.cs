using UnityEditor;

public static class AbilityScriptGenerator
{
    [MenuItem("Assets/Create/Ability Script", false, 80)]
    public static void CreateAbilityScript()
    {
        string templatePath = "Assets/Editor/AbilityScriptTemplate.txt"; // adapte si besoin

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewAbility.cs");
    }
}
