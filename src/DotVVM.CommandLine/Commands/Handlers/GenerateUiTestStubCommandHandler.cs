﻿using System;
using System.IO;
using System.Linq;
using DotVVM.CommandLine.Commands.Core;
using DotVVM.CommandLine.Commands.Logic.SeleniumGenerator;
using DotVVM.CommandLine.Core;
using DotVVM.CommandLine.Core.Metadata;
using DotVVM.CommandLine.Core.Templates;
using DotVVM.Utils.ProjectService;
using DotVVM.Utils.ProjectService.Lookup;

namespace DotVVM.CommandLine.Commands.Handlers
{
    public class GenerateUiTestStubCommandHandler : CommandBase
    {
        public override string Name => "Generate UI Test Stub";

        public override string[] Usages => new[] { "dotvvm gen uitest <NAME>", "dotvvm gut <NAME>" };

        private const string PageObjectsText = "PageObjects";

        public override bool TryConsumeArgs(Arguments args, DotvvmProjectMetadata dotvvmProjectMetadata)
        {
            if (string.Equals(args[0], "gen", StringComparison.CurrentCultureIgnoreCase)
                && string.Equals(args[1], "uitest", StringComparison.CurrentCultureIgnoreCase))
            {
                args.Consume(2);
                return true;
            }

            if (string.Equals(args[0], "gut", StringComparison.CurrentCultureIgnoreCase))
            {
                args.Consume(1);
                return true;
            }

            return false;
        }

        public override void Handle(Arguments args, DotvvmProjectMetadata dotvvmProjectMetadata)
        {
            var metadataService = new DotvvmProjectMetadataService();
            
            // make sure the test directory exists
            if (string.IsNullOrEmpty(dotvvmProjectMetadata.UITestProjectPath))
            {
                var hintProjectName = $"..\\{dotvvmProjectMetadata.ProjectName}.Tests";
                dotvvmProjectMetadata.UITestProjectPath = ConsoleHelpers.AskForValue($"Enter the path to the test project\n(relative to DotVVM project directory, e.g. '{hintProjectName}'): ", hintProjectName);
            }

            var testProjectDirectory = dotvvmProjectMetadata.GetUITestProjectFullPath();
            if (!Directory.Exists(testProjectDirectory))
            {
                GenerateTestProject(testProjectDirectory);
            }

            // make sure we know the test project namespace
            if (string.IsNullOrEmpty(dotvvmProjectMetadata.UITestProjectRootNamespace))
            {
                dotvvmProjectMetadata.UITestProjectRootNamespace = Path.GetFileName(testProjectDirectory);
            }

            // save the metadata
            metadataService.Save(dotvvmProjectMetadata);

            var configuration = new ProjectServiceConfiguration {
                Version = CsprojVersion.DotNetSdk,
                LookupFolder = Environment.CurrentDirectory
            };

            var results = new ProjectSystemSearcher().Search(configuration).ToList();

            SeleniumGeneratorLauncher.Start(args, dotvvmProjectMetadata);
        }

        private static void GenerateTestProject(string projectDirectory)
        {
            var projectFileName = Path.GetFileName(projectDirectory);
            var testProjectPath = Path.Combine(projectDirectory, projectFileName + ".csproj");
            var fileContent = GetProjectFileTextContent();

            FileSystemHelpers.WriteFile(testProjectPath, fileContent);

            CreatePageObjectsDirectory(projectDirectory);
        }

        private static void CreatePageObjectsDirectory(string projectDirectory)
        {
            var objectsDirectory = Path.Combine(projectDirectory, PageObjectsText);
            if (!Directory.Exists(objectsDirectory))
            {
                Directory.CreateDirectory(objectsDirectory);
            }
        }

        private static string GetProjectFileTextContent()
        {
            var projectTemplate = new TestProjectTemplate();

            return projectTemplate.TransformText();
        }
    }
}
