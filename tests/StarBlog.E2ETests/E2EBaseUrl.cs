namespace StarBlog.E2ETests;

internal static class E2EBaseUrl {
    public static bool TryGet(out string baseUrl) {
        var baseUrlEnv = Environment.GetEnvironmentVariable("E2E_BASE_URL");
        if (string.IsNullOrWhiteSpace(baseUrlEnv)) {
            baseUrl = string.Empty;
            return false;
        }

        if (!Uri.TryCreate(baseUrlEnv, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)) {
            baseUrl = string.Empty;
            return false;
        }

        baseUrl = baseUrlEnv.TrimEnd('/');
        return true;
    }
}
