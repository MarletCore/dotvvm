﻿using System.IO;
using System.Linq;
using DotVVM.Utils.ProjectService;
using DotVVM.Utils.ProjectService.Lookup;
using DotVVM.Utils.ProjectService.Operations.Providers;

namespace DotVVM.CommandLine.Commands.Logic.Compiler
{
    public abstract class DotvvmToolProvider
    {
        public static string CombineNugetPath(IResolvedProjectMetadata metadata, string mainModuleRelativePath)
        {
            if (metadata.DotvvmPackageNugetFolders == null || !metadata.DotvvmPackageNugetFolders.Any()) return null;

            var nugetPath = metadata.DotvvmPackageNugetFolders.FirstOrDefault(s => File.Exists(Path.Combine(s, mainModuleRelativePath)));

            return !string.IsNullOrWhiteSpace(nugetPath) ? Path.Combine(nugetPath, mainModuleRelativePath) : null;
        }

        public static string CombineDotvvmRepositoryRoot(IResolvedProjectMetadata metadata,
            ProjectDependency dotvvmDependency, string toolsDotvvmCompilerExe)
        {
            var dotvvmAbsPath = Path.IsPathRooted(dotvvmDependency.ProjectPath) ? dotvvmDependency.ProjectPath : Path.Combine(metadata.ProjectRootDirectory, dotvvmDependency.ProjectPath);
            var dotvvmAbsDir = new FileInfo(Path.GetFullPath(dotvvmAbsPath)).Directory;
            if (dotvvmAbsDir == null) return null;
            var executablePath = Path.GetFullPath(Path.Combine(dotvvmAbsDir.Parent.FullName, toolsDotvvmCompilerExe));
            return File.Exists(executablePath) ? executablePath : null;

        }

        public static DotvvmToolMetadata CreateMetadataOrDefault(string mainModule, DotvvmToolExecutableVersion version)
        {
            if (string.IsNullOrWhiteSpace(mainModule)) return null;
            return new DotvvmToolMetadata() {
                MainModulePath = mainModule,
                Version = version
            };
        }
    }
}