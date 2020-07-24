using Microsoft.Extensions.CommandLineUtils;

namespace SolutionParser
{
    internal class Options
    {
        private CommandLineApplication commandLineApplication;

        private CommandOption SolutionOption { get; }
        private CommandOption ProjectOption { get; }
        private CommandOption ConfigurationOption { get; }
        private CommandOption PlatformOption { get; }
        private CommandOption OrderByPlatformOption { get; }
        private CommandOption HelpOption { get; }

        public string Solution { get; }
        public string Project { get; }
        public string Configuration { get; }
        public string Platform { get; }
        public bool OrderByPlatform => OrderByPlatformOption.HasValue();
        public bool Help => HelpOption.HasValue();

        public bool IsValid => Solution != null;

        public Options(string[] args)
        {
            commandLineApplication = new CommandLineApplication();
            SolutionOption = commandLineApplication.Option("-s|--solution", "[Mandatory] Solution file (*.sln) path", CommandOptionType.SingleValue);
            ProjectOption = commandLineApplication.Option("-p|--project", "[Optional] Project name", CommandOptionType.SingleValue);
            ConfigurationOption = commandLineApplication.Option("-c|--configuration", "[Optional] Solution configuration name (if defined filter only this configuration)", CommandOptionType.SingleValue);
            PlatformOption = commandLineApplication.Option("-l|--platform", "[Optional] Solution platform name (if defined filter only this patform)", CommandOptionType.SingleValue);
            OrderByPlatformOption = commandLineApplication.Option("-op|--orderByPlatform", "[Optional] Output ordered by platform", CommandOptionType.NoValue);
            HelpOption = commandLineApplication.Option("-h|--help", "Show help", CommandOptionType.NoValue);
            var res = commandLineApplication.Execute(args);

            Solution = SolutionOption.HasValue() ? SolutionOption.Value() : null;
            Project = ProjectOption.HasValue() ? ProjectOption.Value() : null;
            Configuration = ConfigurationOption.HasValue() ? ConfigurationOption.Value() : null;
            Platform      = PlatformOption.HasValue() ? PlatformOption.Value() : null;
        }


        public void ShowHelp() => commandLineApplication.ShowHelp();
    }
}
