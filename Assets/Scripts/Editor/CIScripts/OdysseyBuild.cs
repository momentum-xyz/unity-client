using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Odyssey.CI
{
    public static class OdysseyBuild
    {
        private static string pathToGitCommitCS = Application.dataPath + "/Scripts/ODYSSEY/GitCommit.cs";
        private static readonly string Eol = Environment.NewLine;
        private static readonly string[] Secrets = { "androidKeystorePass", "androidKeyaliasName", "androidKeyaliasPass" };

        public static void Build()
        {
            Console.WriteLine("**** Odyssey Build *****");

            // Save the information for the latest commit
            string gitDescription = Git.GetCommitDescription();
            string gitVersion = Git.GenerateSemanticCommitVersion();

            SaveCommitInBuildCode(gitDescription, gitVersion);

            Console.WriteLine("Building from commit: " + gitVersion + " / " + gitDescription);

            // Get which scenes to build from a custom arguments passed to Unity executable
            string scenesToBuild = CIUtils.GetCustomParameterFromGithubAction("buildScenes");
            string stackTrace = CIUtils.GetCustomParameterFromGithubAction("stackTrace");
            string useAddressablesStr = CIUtils.GetCustomParameterFromGithubAction("useAddressables");
            bool useAddressables = useAddressablesStr.Length > 0 ? (useAddressablesStr == "true") : false;
            bool doStackTrace = stackTrace.Length > 0 ? (stackTrace == "true") : false;

            bool buildWithCustomScenes = scenesToBuild.Length > 0 ? true : false;

            Debug.Log("BUILDING WITH CUSTOM SCENES: " + buildWithCustomScenes);

            string[] scenesToBuildArr = scenesToBuild.Split(',');
            string[] scenes = new string[scenesToBuildArr.Length];

            for (var i = 0; i < scenesToBuildArr.Length; ++i)
            {
                Console.WriteLine("Scene to be build: " + scenesToBuildArr[i]);
                scenes[i] = "Assets/Scenes/" + scenesToBuildArr[i] + ".unity";
            }

            Dictionary<string, string> options = GetValidatedOptions();

            PlayerSettings.bundleVersion = options["buildVersion"];

            // Remove stack trace
            if (!doStackTrace)
            {
                PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
                PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.None);
                PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
                PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
            }

            PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });

            // Needed in Unity 2021 to handle Generics
            PlayerSettings.SetAdditionalIl2CppArgs("--generic-virtual-method-iterations=2");

            AddressableAssetSettings.CleanPlayerContent();
            AddressableAssetSettings.BuildPlayerContent();

            var buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), options["buildTarget"]);
            var filePath = options["customBuildPath"];

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = buildWithCustomScenes ? scenes : GetEnabledScenesFromEditorSettings(),
                target = buildTarget,
                locationPathName = filePath,
            };

            BuildSummary buildSummary = BuildPipeline.BuildPlayer(buildPlayerOptions).summary;
            ReportSummary(buildSummary);
            ExitWithResult(buildSummary.result);
        }

        static string[] GetEnabledScenesFromEditorSettings()
        {
            return (
                from scene in EditorBuildSettings.scenes
                where scene.enabled
                where !string.IsNullOrEmpty(scene.path)
                select scene.path
            ).ToArray();
        }

        private static void ReportSummary(BuildSummary summary)
        {
            Console.WriteLine(
                $"{Eol}" +
                $"###########################{Eol}" +
                $"#      Build results      #{Eol}" +
                $"###########################{Eol}" +
                $"{Eol}" +
                $"Duration: {summary.totalTime.ToString()}{Eol}" +
                $"Warnings: {summary.totalWarnings.ToString()}{Eol}" +
                $"Errors: {summary.totalErrors.ToString()}{Eol}" +
                $"Size: {summary.totalSize.ToString()} bytes{Eol}" +
                $"{Eol}"
            );
        }

        public static Dictionary<string, string> GetValidatedOptions()
        {
            ParseCommandLineArguments(out Dictionary<string, string> validatedOptions);

            if (!validatedOptions.TryGetValue("projectPath", out string _))
            {
                Console.WriteLine("Missing argument -projectPath");
                EditorApplication.Exit(110);
            }

            if (!validatedOptions.TryGetValue("buildTarget", out string buildTarget))
            {
                Console.WriteLine("Missing argument -buildTarget");
                EditorApplication.Exit(120);
            }

            if (!Enum.IsDefined(typeof(BuildTarget), buildTarget ?? string.Empty))
            {
                EditorApplication.Exit(121);
            }

            if (!validatedOptions.TryGetValue("customBuildPath", out string _))
            {
                Console.WriteLine("Missing argument -customBuildPath");
                EditorApplication.Exit(130);
            }

            const string defaultCustomBuildName = "TestBuild";
            if (!validatedOptions.TryGetValue("customBuildName", out string customBuildName))
            {
                Console.WriteLine($"Missing argument -customBuildName, defaulting to {defaultCustomBuildName}.");
                validatedOptions.Add("customBuildName", defaultCustomBuildName);
            }
            else if (customBuildName == "")
            {
                Console.WriteLine($"Invalid argument -customBuildName, defaulting to {defaultCustomBuildName}.");
                validatedOptions.Add("customBuildName", defaultCustomBuildName);
            }

            return validatedOptions;
        }

        public static void ParseCommandLineArguments(out Dictionary<string, string> providedArguments)
        {
            providedArguments = new Dictionary<string, string>();
            string[] args = Environment.GetCommandLineArgs();

            Console.WriteLine(
                $"{Eol}" +
                $"###########################{Eol}" +
                $"#    Parsing settings     #{Eol}" +
                $"###########################{Eol}" +
                $"{Eol}"
            );

            // Extract flags with optional values
            for (int current = 0, next = 1; current < args.Length; current++, next++)
            {
                // Parse flag
                bool isFlag = args[current].StartsWith("-");
                if (!isFlag) continue;
                string flag = args[current].TrimStart('-');

                // Parse optional value
                bool flagHasValue = next < args.Length && !args[next].StartsWith("-");
                string value = flagHasValue ? args[next].TrimStart('-') : "";
                bool secret = Secrets.Contains(flag);
                string displayValue = secret ? "*HIDDEN*" : "\"" + value + "\"";

                // Assign
                Console.WriteLine($"Found flag \"{flag}\" with value {displayValue}.");
                providedArguments.Add(flag, value);
            }
        }

        private static void ExitWithResult(BuildResult result)
        {
            switch (result)
            {
                case BuildResult.Succeeded:
                    Console.WriteLine("Build succeeded!");
                    EditorApplication.Exit(0);
                    break;
                case BuildResult.Failed:
                    Console.WriteLine("Build failed!");
                    EditorApplication.Exit(101);
                    break;
                case BuildResult.Cancelled:
                    Console.WriteLine("Build cancelled!");
                    EditorApplication.Exit(102);
                    break;
                case BuildResult.Unknown:
                default:
                    Console.WriteLine("Build result is unknown!");
                    EditorApplication.Exit(103);
                    break;
            }
        }

        public static void SaveCommitInBuildCode(string description, string version)
        {
            File.WriteAllText(pathToGitCommitCS, "public static class GitCommit { public const string Description=\"" + description + "\";public const string Version=\"" + version + "\";}");
        }
    }
}
