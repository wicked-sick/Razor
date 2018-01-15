﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Razor.Tools;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis.CommandLine;
using Microsoft.Extensions.CommandLineUtils;
using Roslyn.Utilities;

namespace Microsoft.AspNetCore.Razor.Tasks
{
    public abstract class DotNetToolTask : ToolTask
    {
        private CancellationTokenSource _razorServerCts;

        public bool Debug { get; set; }

        public bool DebugTool { get; set; }

        [Required]
        public string ToolAssembly { get; set; }

        public string ServerAssembly { get; set; }

        public bool UseServer { get; set; }

        protected override string ToolName => "dotnet";

        // If we're debugging then make all of the stdout gets logged in MSBuild
        protected override MessageImportance StandardOutputLoggingImportance => DebugTool ? MessageImportance.High : base.StandardOutputLoggingImportance;

        protected override MessageImportance StandardErrorLoggingImportance => MessageImportance.High;

        internal abstract RequestCommand Command { get; }

        protected override string GenerateFullPathToTool()
        {
#if NETSTANDARD2_0
            if (!string.IsNullOrEmpty(DotNetMuxer.MuxerPath))
            {
                return DotNetMuxer.MuxerPath;
            }
#endif

            // use PATH to find dotnet
            return ToolExe;
        }

        protected override string GenerateCommandLineCommands()
        {
            return $"exec \"{ToolAssembly}\"" + (DebugTool ? " --debug" : "");
        }

        protected override string GetResponseFileSwitch(string responseFilePath)
        {
            return "@\"" + responseFilePath + "\"";
        }

        protected abstract override string GenerateResponseFileCommands();

        public override bool Execute()
        {
            if (Debug)
            {
                while (!Debugger.IsAttached)
                {
                    Log.LogMessage(MessageImportance.High, "Waiting for debugger in pid: {0}", Process.GetCurrentProcess().Id);
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }

            return base.Execute();
        }

        protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
        {
            if (UseServer &&
                !string.IsNullOrEmpty(ServerAssembly) &&
                TryExecuteOnServer(pathToTool, responseFileCommands, commandLineCommands, out var result))
            {
                return result;
            }

            return base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);
        }

        protected override void LogToolCommand(string message)
        {
            if (Debug)
            {
                Log.LogMessage(MessageImportance.High, message);
            }
            else
            {
                base.LogToolCommand(message);
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            _razorServerCts?.Cancel();
        }

        protected virtual bool TryExecuteOnServer(string pathToTool, string responseFileCommands, string commandLineCommands, out int result)
        {
            CompilerServerLogger.Log("Server execution started.");
            using (_razorServerCts = new CancellationTokenSource())
            {
                CompilerServerLogger.Log($"CommandLine = '{commandLineCommands}'");
                CompilerServerLogger.Log($"BuildResponseFile = '{responseFileCommands}'");

                var clientDir = Path.GetDirectoryName(ServerAssembly);

                var workingDir = CurrentDirectoryToUse();
                var buildPaths = new BuildPathsAlt(
                    clientDir: clientDir,
                    // MSBuild doesn't need the .NET SDK directory
                    sdkDir: null,
                    workingDir: workingDir,
                    tempDir: BuildServerConnection.GetTempPath(workingDir));

                var responseTask = BuildServerConnection.RunServerCompilation(
                    Command,
                    GetArguments(responseFileCommands),
                    buildPaths,
                    keepAlive: null,
                    libEnvVariable: LibDirectoryToUse(),
                    cancellationToken: _razorServerCts.Token);

                responseTask.Wait(_razorServerCts.Token);

                var response = responseTask.Result;
                if (response.Type == BuildResponse.ResponseType.Completed &&
                    response is CompletedBuildResponse completedResponse)
                {
                    CompilerServerLogger.Log("Server execution completed.");

                    result = completedResponse.ReturnCode;
                    return true;
                }
            }

            CompilerServerLogger.Log("Server execution failed.");
            result = -1;

            return false;
        }

        /// <summary>
        /// Get the current directory that the compiler should run in.
        /// </summary>
        private string CurrentDirectoryToUse()
        {
            // ToolTask has a method for this. But it may return null. Use the process directory
            // if ToolTask didn't override. MSBuild uses the process directory.
            string workingDirectory = GetWorkingDirectory();
            if (string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = Directory.GetCurrentDirectory();
            }
            return workingDirectory;
        }

        /// <summary>
        /// Get the "LIB" environment variable, or NULL if none.
        /// </summary>
        private string LibDirectoryToUse()
        {
            // First check the real environment.
            var libDirectory = Environment.GetEnvironmentVariable("LIB");

            // Now go through additional environment variables.
            var additionalVariables = EnvironmentVariables;
            if (additionalVariables != null)
            {
                foreach (var variable in EnvironmentVariables)
                {
                    if (variable.StartsWith("LIB=", StringComparison.OrdinalIgnoreCase))
                    {
                        libDirectory = variable.Substring(4);
                    }
                }
            }

            return libDirectory;
        }

        private List<string> GetArguments(string responseFileCommands)
        {
            var responseFileArguments =
                CommandLineUtilities.SplitCommandLineIntoArguments(responseFileCommands, removeHashComments: true);
            return responseFileArguments.ToList();
        }
    }
}
