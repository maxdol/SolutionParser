using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolutionParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options(args);
            if (options.Help)
            {
                options.ShowHelp();
                return;
            }

            if (!options.IsValid)
            {
                options.ShowHelp();
                return;
            }
            Console.WriteLine($"Parse {options.Solution} {(options.Project != null ? "filter to project " + options.Project : "")}");

            var sln = SolutionFile.Parse(options.Solution);

            if (options.Project != null)
            {
                var prj = sln.ProjectsInOrder.SingleOrDefault(p => p.ProjectName == options.Project);
                if (prj == null)
                {
                    Console.WriteLine($"Project {options.Project} was not found in {options.Solution}");
                    options.ShowHelp();
                    return;
                }

                ProcessProject(prj, options);
            }
            else
            {
                foreach (var p in sln.ProjectsInOrder.OrderBy(x => x.ProjectName))
                {
                    ProcessProject(p, options);
                }

                PrintIncludeInBuildSummary(options, sln);
                PrintDependenciesTree(options, sln);
            }
        }

        private static void PrintIncludeInBuildSummary(Options options, SolutionFile sln)
        {
            var projectsInConfigurations = new List<ProjectIncludeInBuildSummary>();
            foreach (var prj in sln.ProjectsInOrder.OrderBy(x => x.ProjectName))
            {
                foreach (var c in prj.ProjectConfigurations.OrderBy(c => new ConfigurationAndPlatform(c.Key).PlatformName))
                {
                    var solutionConf = new ConfigurationAndPlatform(c.Key);
                    var projectConf = c.Value;
                    var projectPlatform = new ProjectIncludeInBuildSummary(prj.ProjectName, solutionConf.PlatformName);
                    var last = projectsInConfigurations.LastOrDefault();
                    if (last == null || last.PlatformName != projectPlatform.PlatformName || last.ProjectName != projectPlatform.ProjectName)
                    {
                        projectPlatform.Configurations.Add(Tuple.Create(solutionConf.ConfigurationName, projectConf.IncludeInBuild));
                        projectsInConfigurations.Add(projectPlatform);
                    }
                    else
                    {
                        last.Configurations.Add(Tuple.Create(solutionConf.ConfigurationName, projectConf.IncludeInBuild));
                    }
                }
            }

            Console.WriteLine($"\n\n---------------------------\n\n");

            if (options.OrderByPlatform)
                projectsInConfigurations = projectsInConfigurations.OrderBy(x => x.PlatformName).ToList();

            foreach (var p in projectsInConfigurations)
            {
                bool allIncluded = p.Configurations.All(t => t.Item2);
                bool allExcluded = p.Configurations.All(t => !t.Item2);
                string sign = allIncluded ? "+" :
                              allExcluded ? "-"
                                          : "?";
                string configs = string.Join(", ", p.Configurations.OrderBy(t => t.Item1)
                                        .Select(t => $"({t.Item1} {(t.Item2 ? "+" : "-")})"));
                Console.WriteLine($"{p.ProjectName,-30} {p.PlatformName,-30} {sign}\t [{configs}]");
            }
        }
        private static void PrintDependenciesTree(Options options, SolutionFile sln)
        {
            Console.WriteLine("\n\n================ Project dependencies ===================");
            var projGuids = sln.ProjectsInOrder.ToDictionary(p => p.ProjectGuid.ToUpper(), p => p.ProjectName);
            foreach (var p in sln.ProjectsInOrder.OrderBy(x => x.ProjectName).Where(p => p.ProjectType != SolutionProjectType.SolutionFolder))
            {
                Console.WriteLine($"{p.ProjectName,-30}");
                foreach ( var d in p.Dependencies.Select(x => projGuids[x]))
                {
                    Console.WriteLine($"    - sln {d,-30}");
                }

                var prj = new Project(p.AbsolutePath);
                var references = prj.ProjectReferences.ToList();
                foreach (var d in references.Select(x => $"{projGuids[x.Item2.ToUpper()],-30}{(x.Item3 != null ? " Condition = " + x.Item3 : "") }"))
                {
                    Console.WriteLine($"    --prj {d,-30}");
                }
            }
        }

        private static void ProcessProject(ProjectInSolution prj, Options options)
        {
            var projectConfigurations = options.Platform != null ? prj.ProjectConfigurations.Where(c => new ConfigurationAndPlatform(c.Key).PlatformName == options.Platform) : prj.ProjectConfigurations;
            projectConfigurations = options.Configuration != null ? projectConfigurations.Where(c => new ConfigurationAndPlatform(c.Key).ConfigurationName == options.Configuration) : projectConfigurations;

            if (options.OrderByPlatform)
                projectConfigurations = projectConfigurations.OrderBy(c => new ConfigurationAndPlatform(c.Key).PlatformName);

            foreach (var c in projectConfigurations)
            {
                var solutionConf = new ConfigurationAndPlatform(c.Key);
                var projectConf = c.Value;
                var prefix = options.Project == null ? $"{prj.ProjectName,-30}" : "";
                string warn = solutionConf.ConfigurationName != projectConf.ConfigurationName ? "!!" : "  ";
                warn += solutionConf.PlatformName != projectConf.PlatformName ? "!!" : "  ";
                Console.WriteLine($"{prefix}{solutionConf.ConfigurationName,-20} | {solutionConf.PlatformName,-30} : {projectConf.ConfigurationName,-20} | {projectConf.PlatformName,-30} {(projectConf.IncludeInBuild ? "+" : "-")} {warn}");
            }
        }
    }
}
