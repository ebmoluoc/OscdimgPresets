using CsharpHelpers.Helpers;
using System;

namespace OscdimgPresets.Services
{

    public interface IArgumentService
    {
        string PresetName { get; }
        string SourcePath { get; }
        bool CreateNow { get; }
    }


    public sealed class ArgumentService : IArgumentService
    {

        private readonly string[] _args;


        public ArgumentService()
        {
            _args = Environment.GetCommandLineArgs();
        }


        public ArgumentService(string[] args)
        {
            _args = args;
        }


        public string PresetName
        {
            get { return EnvironmentHelper.GetArgument("-p:", _args); }
        }


        public string SourcePath
        {
            get { return EnvironmentHelper.GetArgument("-s:", _args); }
        }


        public bool CreateNow
        {
            get { return EnvironmentHelper.GetArgument("-c", _args) == ""; }
        }

    }

}
