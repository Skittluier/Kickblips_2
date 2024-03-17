using System;

// This file is auto-generated. Do not modify or move this file.

namespace SuperUnityBuild.Generated
{
    public enum ReleaseType
    {
        None,
        _Itch_io__Release,
        _Itch_io__Development,
        _Steam__Development,
        _Steam__Release,
        _Standalone__Development,
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
        public static readonly DateTime buildDate = new DateTime(638463110793651052);
        public const string version = "0.0.3";
        public const ReleaseType releaseType = ReleaseType._Standalone__Development;
        public const Platform platform = Platform.PC;
        public const ScriptingBackend scriptingBackend = ScriptingBackend.IL2CPP;
        public const Architecture architecture = Architecture.Windows_x64;
        public const Distribution distribution = Distribution.None;
    }
}

