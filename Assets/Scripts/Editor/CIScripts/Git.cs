using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Odyssey.CI
{
    public static class Git
    {
        const string application = @"git";

        /// <summary>
        /// Generate a version based on the latest tag and the amount of commits.
        /// Format: 0.1.2 (where 2 is the amount of commits).
        ///
        /// If no tag is present in the repository then v0.0 is assumed.
        /// This would result in 0.0.# where # is the amount of commits.
        /// </summary>
        public static string GenerateSemanticCommitVersion()
        {
            string version;

            if (HasAnyVersionTags())
            {
                version = GetSemanticCommitVersion();
                Console.WriteLine("Repository has a valid version tag.");
            }
            else
            {
                version = $"0.0.{GetTotalNumberOfCommits()}";
                Console.WriteLine("Repository does not have tags to base the version on.");
            }

            Console.WriteLine($"Version is {version}");

            return version;
        }

        /// <summary>
        /// Get the version of the current tag.
        ///
        /// The tag must point at HEAD for this method to work.
        ///
        /// Output Format:
        /// #.* (where # is the major version and * can be any number of any type of character)
        /// </summary>
        public static string GetTagVersion()
        {
            string version = Run(@"tag --points-at HEAD | grep v[0-9]*");

            if (version.Length == 0) return "";

            version = version.Substring(1);

            return version;
        }

        /// <summary>
        /// Get the total number of commits.
        /// </summary>
        static int GetTotalNumberOfCommits()
        {
            string numberOfCommitsAsString = Run(@"git rev-list --count HEAD");

            if (numberOfCommitsAsString.Length == 0) return 0;

            return int.Parse(numberOfCommitsAsString);
        }

        /// <summary>
        /// Whether or not the repository has any version tags yet.
        /// </summary>
        static bool HasAnyVersionTags()
        {
            return "0" != Run(@"tag --list --merged HEAD | grep v[0-9]* | wc -l");
        }

        /// <summary>
        /// Retrieves the build version from git based on the most recent matching tag and
        /// commit history. This returns the version as: {major.minor.build} where 'build'
        /// represents the nth commit after the tagged commit.
        /// Note: The initial 'v' and the commit hash are removed.
        /// </summary>
        static string GetSemanticCommitVersion()
        {
            // v0.1-2-g12345678 (where 2 is the amount of commits, g stands for git)
            string version = GetVersionString();

            if (version.Length == 0) return "";

            // 0.1-2
            version = version.Substring(1, version.LastIndexOf('-') - 1);
            // 0.1.2
            version = version.Replace('-', '.');

            return version;
        }

        /// <summary>
        /// Get version string.
        ///
        /// Format: `v0.1-2-g12345678` (where 2 is the amount of commits since the last tag)
        ///
        /// See: https://softwareengineering.stackexchange.com/questions/141973/how-do-you-achieve-a-numeric-versioning-scheme-with-git
        /// </summary>
        static string GetVersionString()
        {
            return Run(@"describe --tags --long --match ""v[0-9]*""");

            // Todo - implement split function based on this more complete query
            // return Run(@"describe --long --tags --dirty --always");
        }

        public static string GetCommitDescription()
        {
            return Run(@"show -s --format='%h by %an on %cd'");
        }

        public static string GetLastCommitHash()
        {
            return Run(@"rev-parse --short HEAD");
        }
        /// <summary>
        /// Runs git binary with any given arguments and returns the output.
        /// </summary>
        static string Run(string arguments)
        {
            using (var process = new System.Diagnostics.Process())
            {
                string workingDirectory = UnityEngine.Application.dataPath;

                int exitCode = process.Run(application, arguments, workingDirectory, out string output, out string errors);

                if (exitCode != 0) { return ""; }

                return output;
            }
        }

        // Execute an application or binary with given arguments
        //
        // See: https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        public static int Run(this Process process, string application,
          string arguments, string workingDirectory, out string output,
          out string errors)
        {
            // Configure how to run the application
            process.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = application,
                Arguments = arguments,
                WorkingDirectory = workingDirectory
            };

            // Read the output
            var outputBuilder = new StringBuilder();
            var errorsBuilder = new StringBuilder();
            process.OutputDataReceived += (_, args) => outputBuilder.AppendLine(args.Data);
            process.ErrorDataReceived += (_, args) => errorsBuilder.AppendLine(args.Data);

            // Run the application and wait for it to complete
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            // Format the output
            output = outputBuilder.ToString().TrimEnd();
            errors = errorsBuilder.ToString().TrimEnd();

            return process.ExitCode;
        }
    }

}
