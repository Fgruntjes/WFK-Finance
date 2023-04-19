using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Pulumi;

namespace App.Deploy.Utils;

internal static class EnvFileWriter
{
    public static void Write(string path, InputMap<string> env)
    {
        env.Apply(env =>
        {
            var envFile = new StringBuilder();
            foreach (var kvp in env)
            {
                envFile.AppendLine($"{kvp.Key}='{kvp.Value}'");
            }
            File.WriteAllText(path, envFile.ToString());

            return env;
        });
    }
}