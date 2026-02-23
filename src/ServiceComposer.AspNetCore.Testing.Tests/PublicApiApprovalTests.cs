using System.Reflection;
using System.Text;

namespace ServiceComposer.AspNetCore.Testing.Tests;

public class PublicApiApprovalTests
{
    [Fact]
    public void Public_api_is_approved()
    {
        var actual = GeneratePublicApi();
        var approvedPath = Path.Combine(AppContext.BaseDirectory, "PublicApi.approved.txt");
        var approved = File.ReadAllText(approvedPath);

        Assert.Equal(Normalize(approved), Normalize(actual));
    }

    static string GeneratePublicApi()
    {
        var assembly = typeof(WebApplicationFactoryWithWebHost<>).Assembly;
        var lines = new List<string>();

        foreach (var type in assembly.GetExportedTypes().OrderBy(t => t.FullName, StringComparer.Ordinal))
        {
            lines.Add($"type {FormatTypeName(type)}");

            foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).OrderBy(c => c.ToString(), StringComparer.Ordinal))
            {
                lines.Add($"  ctor {FormatConstructor(constructor)}");
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).OrderBy(p => p.Name, StringComparer.Ordinal))
            {
                lines.Add($"  property {FormatTypeName(property.PropertyType)} {property.Name}");
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                         .Where(m => !m.IsSpecialName)
                         .OrderBy(m => m.ToString(), StringComparer.Ordinal))
            {
                lines.Add($"  method {FormatTypeName(method.ReturnType)} {method.Name}({string.Join(", ", method.GetParameters().Select(FormatParameter))})");
            }
        }

        return string.Join(Environment.NewLine, lines);
    }

    static string FormatConstructor(ConstructorInfo constructor) => $"{constructor.DeclaringType!.Name}({string.Join(", ", constructor.GetParameters().Select(FormatParameter))})";

    static string FormatParameter(ParameterInfo parameter) => $"{FormatTypeName(parameter.ParameterType)} {parameter.Name}";

    static string FormatTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeName = type.GetGenericTypeDefinition().FullName!;
            var tickIndex = genericTypeName.IndexOf('`');
            var typeName = tickIndex >= 0 ? genericTypeName[..tickIndex] : genericTypeName;
            return $"{typeName}<{string.Join(", ", type.GetGenericArguments().Select(FormatTypeName))}>";
        }

        return type.FullName ?? type.Name;
    }

    static string Normalize(string value)
    {
        var builder = new StringBuilder(value.Length);
        using var reader = new StringReader(value);
        string? line;
        var first = true;
        while ((line = reader.ReadLine()) != null)
        {
            if (!first)
            {
                builder.Append('\n');
            }

            builder.Append(line.TrimEnd());
            first = false;
        }

        return builder.ToString();
    }
}
