using System.IO;
using System.Text;
using Pulumi;

namespace App.Deploy.Utils;

internal static class EnvFileWriter
{
    public static void Write(string path, InputMap<string> env)
    {
        ToString(env).Apply(envData =>
        {
            File.WriteAllText(path, envData);
            return env;
        });
    }

    public static Output<string> ToString(InputMap<string> env)
    {
        return env.Apply(env =>
        {
            var envFile = new StringBuilder();
            foreach (var kvp in env)
            {
                envFile.AppendLine($"{kvp.Key}='{kvp.Value}'");
            }

            return envFile.ToString();
        });
    }
}