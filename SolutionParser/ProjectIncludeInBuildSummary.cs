using System;
using System.Collections.Generic;

namespace SolutionParser
{
    public class ProjectIncludeInBuildSummary
    {
        public ProjectIncludeInBuildSummary(string projectName, string platformName)
        {
            ProjectName = projectName;
            PlatformName = platformName;
            Configurations = new List<Tuple<string, bool>>();
        }

        public string ProjectName { get; }
        public string PlatformName { get; }

        public IList<Tuple<string, bool>> Configurations { get; }
    }
}
