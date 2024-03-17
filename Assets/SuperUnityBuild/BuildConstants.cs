using System;

// This file is auto-generated. Do not modify or move this file.

namespace SuperUnityBuild.Generated
{
    public enum ReleaseType
    {
        None,
        Release,
        Development,
    }

    public enum Platform
    {
        None,
        WebGL,
        PC,
    }

    public enum ScriptingBackend
    {
        None,
        IL2CPP,
    }

    public enum Architecture
    {
        None,
        WebGL,
        Windows_x86,
        Windows_x64,
    }

    public enum Distribution
    {
        None,
    }

    public static class BuildConstants
    {
        public static readonly DateTime buildDate = new DateTime(638461228372525558);
        public const string version = "0.0.1";
        public const ReleaseType releaseType = ReleaseType.Development;
        public const Platform platform = Platform.PC;
        public const ScriptingBackend scriptingBackend = ScriptingBackend.IL2CPP;
        public const Architecture architecture = Architecture.Windows_x64;
        public const Distribution distribution = Distribution.None;
    }
}

