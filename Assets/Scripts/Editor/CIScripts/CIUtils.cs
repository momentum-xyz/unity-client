using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey.CI
{
    public static class CIUtils
    {
        /// <summary>
        /// Gets a parameter defined in customParamaters option in build-docker-image workflow
        /// ex: customParamaters: -buildScene SCENE_NAME
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCustomParameterFromGithubAction(string name)
        {
            var args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-" + name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
            return "";
        }
    }
}

