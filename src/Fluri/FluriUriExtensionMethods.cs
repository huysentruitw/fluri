using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class FluriUriExtensionMethods
    {
        private static readonly string FakeSchemeAndServer = $"fake://{Guid.NewGuid():N}";
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static Uri WithQueryParameter(this Uri uri, string parameterName) => uri.WithQueryParameter(parameterName, null);

        public static Uri WithQueryParameter(this Uri uri, string parameterName, string parameterValue)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            return uri.CombineQueryParameters((Name: parameterName, Value: parameterValue));
        }

        public static Uri WithQueryParameters(this Uri uri, IReadOnlyDictionary<string, string> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            return uri.CombineQueryParameters(parameters.Select(x => (Name: x.Key, Value: x.Value)).ToArray());
        }

        public static Uri WithQueryParameters(this Uri uri, object parametersObject, bool ignoreNullValues = false)
        {
            if (parametersObject == null) throw new ArgumentNullException(nameof(parametersObject));

            var parameterProperties = PropertyCache.GetOrAdd(
                parametersObject.GetType(),
                type => type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty).ToArray());

            var parameters = parameterProperties.Select(property => (Name: property.Name, Value: property.GetValue(parametersObject)?.ToString()));
            if (ignoreNullValues) parameters = parameters.Where(x => x.Value != null);
            return uri.CombineQueryParameters(parameters.ToArray());
        }

        private static Uri CombineQueryParameters(this Uri uri, params (string Name, string Value)[] parameters)
        {
            var absoluteUri = uri.IsAbsoluteUri ? uri : new Uri(new Uri(FakeSchemeAndServer), uri);

            var uriWithoutQueryAndFragment = absoluteUri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped);

            var builder = new UriBuilder(uriWithoutQueryAndFragment)
            {
                Query = absoluteUri.GetCombinedQueryComponent(parameters),
                Fragment = absoluteUri.GetComponents(UriComponents.Fragment, UriFormat.Unescaped)
            };

            if (uri.IsAbsoluteUri) return builder.Uri;

            var pattern = $"^{FakeSchemeAndServer}{(uri.OriginalString[0] != '/' ? "/" : string.Empty)}";
            var relativeUri = Regex.Replace(builder.Uri.AbsoluteUri, pattern, string.Empty);
            return new Uri(relativeUri, UriKind.Relative);
        }

        private static string GetCombinedQueryComponent(this Uri uri, params (string Name, string Value)[] parameters)
        {
            var builder = new StringBuilder(uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped));
            foreach (var parameter in parameters)
            {
                if (builder.Length > 0) builder.Append("&");
                builder.Append(Uri.EscapeUriString(parameter.Name));
                if (parameter.Value == null) continue;
                builder.Append("=");
                builder.Append(Uri.EscapeUriString(parameter.Value));
            }

            return builder.ToString();
        }

        public static Uri WithFragment(this Uri uri, string fragment)
        {
            if (string.IsNullOrEmpty(fragment)) throw new ArgumentNullException(nameof(fragment));

            var absoluteUri = uri.IsAbsoluteUri ? uri : new Uri(new Uri(FakeSchemeAndServer), uri);

            var uriWithoutFragment = absoluteUri.GetComponents(UriComponents.SchemeAndServer | UriComponents.PathAndQuery, UriFormat.Unescaped);

            var builder = new UriBuilder(uriWithoutFragment)
            {
                Fragment = Uri.EscapeUriString(fragment)
            };

            if (uri.IsAbsoluteUri) return builder.Uri;

            var pattern = $"^{FakeSchemeAndServer}{(uri.OriginalString[0] != '/' ? "/" : string.Empty)}";
            var relativeUri = Regex.Replace(builder.Uri.AbsoluteUri, pattern, string.Empty);
            return new Uri(relativeUri, UriKind.Relative);
        }
    }
}
