using System;
using System.Collections.Generic;

namespace MissionPlanner.AIWaypointPlanner
{
    public enum ApiProtocol
    {
        Responses,
        ChatCompletions
    }

    public enum ApiAuthenticationMode
    {
        Bearer,
        ApiKeyHeader,
        None
    }

    public enum ApiReasoningLevel
    {
        Off,
        Low,
        Medium,
        High,
        ExtraHigh,
        Ultra
    }

    public sealed class ApiConnectionSettings
    {
        public string BaseUrl { get; set; }
        public ApiProtocol Protocol { get; set; }
        public ApiAuthenticationMode AuthenticationMode { get; set; }
        public string Model { get; set; }
        public string ApiKey { get; set; }
        public string ProjectId { get; set; }
        public ApiReasoningLevel ReasoningLevel { get; set; } = ApiReasoningLevel.Off;
        public string DisplayLanguageCode { get; set; } = UiStrings.DefaultLanguageCode;

        public Uri BuildEndpoint(string relativePath)
        {
            Uri baseUri = ValidateBaseUrl(BaseUrl, DisplayLanguageCode);
            string normalized = baseUri.AbsoluteUri.TrimEnd('/') + "/";
            return new Uri(new Uri(normalized, UriKind.Absolute), relativePath.TrimStart('/'));
        }

        public void Validate()
        {
            ValidateBaseUrl(BaseUrl, DisplayLanguageCode);
            if (string.IsNullOrWhiteSpace(Model))
                throw new ArgumentException(UiStrings.Get(DisplayLanguageCode, "Api.ErrorModelRequired"), "Model");
            if (AuthenticationMode != ApiAuthenticationMode.None && string.IsNullOrWhiteSpace(ApiKey))
                throw new ArgumentException(UiStrings.Get(DisplayLanguageCode, "Api.ErrorKeyRequired"), "ApiKey");
            if (!Enum.IsDefined(typeof(ApiReasoningLevel), ReasoningLevel))
                throw new ArgumentException(UiStrings.Get(DisplayLanguageCode, "Api.ErrorReasoningInvalid"), "ReasoningLevel");
        }

        public static Uri ValidateBaseUrl(string baseUrl)
        {
            return ValidateBaseUrl(baseUrl, UiStrings.DefaultLanguageCode);
        }

        public static Uri ValidateBaseUrl(string baseUrl, string languageCode)
        {
            Uri uri;
            if (string.IsNullOrWhiteSpace(baseUrl) ||
                !Uri.TryCreate(baseUrl.Trim(), UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException(UiStrings.Get(languageCode, "Api.ErrorBaseUrlInvalid"), "baseUrl");
            }

            if (!string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
                throw new ArgumentException(UiStrings.Get(languageCode, "Api.ErrorBaseUrlQuery"), "baseUrl");

            if (uri.Scheme == Uri.UriSchemeHttp && !uri.IsLoopback)
                throw new ArgumentException(UiStrings.Get(languageCode, "Api.ErrorRemoteHttp"), "baseUrl");

            return uri;
        }
    }

    public sealed class ApiProviderPreset
    {
        public ApiProviderPreset(
            string name,
            string baseUrl,
            ApiProtocol protocol,
            ApiAuthenticationMode authenticationMode,
            string model,
            string note,
            ApiReasoningLevel reasoningLevel = ApiReasoningLevel.Medium,
            string noteKey = null,
            string nameKey = null)
        {
            Name = name;
            BaseUrl = baseUrl;
            Protocol = protocol;
            AuthenticationMode = authenticationMode;
            Model = model;
            Note = note;
            NoteKey = noteKey;
            NameKey = nameKey;
            ReasoningLevel = reasoningLevel;
        }

        public string Name { get; private set; }
        public string BaseUrl { get; private set; }
        public ApiProtocol Protocol { get; private set; }
        public ApiAuthenticationMode AuthenticationMode { get; private set; }
        public string Model { get; private set; }
        public string Note { get; private set; }
        public string NoteKey { get; private set; }
        public string NameKey { get; private set; }
        public string DisplayName { get; set; }
        public ApiReasoningLevel ReasoningLevel { get; private set; }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(DisplayName) ? Name : DisplayName;
        }

        public static IList<ApiProviderPreset> CreateDefaults()
        {
            return new List<ApiProviderPreset>
            {
                new ApiProviderPreset(
                    "CC Switch (local)", "http://127.0.0.1:15721/v1", ApiProtocol.Responses,
                    ApiAuthenticationMode.None, "gpt-5.6-sol",
                    "Start the CC Switch proxy and route first; CC Switch manages the upstream key or OAuth session.",
                    noteKey: "ProviderNote.CcSwitch", nameKey: "ProviderName.CcSwitch"),
                new ApiProviderPreset(
                    "OpenAI", "https://api.openai.com/v1", ApiProtocol.Responses,
                    ApiAuthenticationMode.Bearer, "gpt-5.6-sol",
                    "Use an OpenAI API key; a ChatGPT or Codex login is not itself an API key.",
                    noteKey: "ProviderNote.OpenAi", nameKey: "ProviderName.OpenAi"),
                new ApiProviderPreset(
                    "OpenRouter", "https://openrouter.ai/api/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.Bearer, "openai/gpt-5.2",
                    "Use an OpenRouter key; use the model ID currently listed by OpenRouter.",
                    ApiReasoningLevel.Off,
                    noteKey: "ProviderNote.OpenRouter", nameKey: "ProviderName.OpenRouter"),
                new ApiProviderPreset(
                    "LiteLLM Proxy (local)", "http://127.0.0.1:4000/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.Bearer, "gpt-5.2",
                    "For a local LiteLLM Proxy; choose no authentication when the proxy has no key.",
                    ApiReasoningLevel.Off,
                    noteKey: "ProviderNote.LiteLlm", nameKey: "ProviderName.LiteLlm"),
                new ApiProviderPreset(
                    "LM Studio (local)", "http://127.0.0.1:1234/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.None, "local-model",
                    "Load a model in LM Studio and start its local server.",
                    ApiReasoningLevel.Off,
                    noteKey: "ProviderNote.LmStudio", nameKey: "ProviderName.LmStudio"),
                new ApiProviderPreset(
                    "Ollama (local)", "http://127.0.0.1:11434/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.None, "qwen3",
                    "Start Ollama and change the model ID to one already pulled locally.",
                    ApiReasoningLevel.Off,
                    noteKey: "ProviderNote.Ollama", nameKey: "ProviderName.Ollama"),
                new ApiProviderPreset(
                    "New API / One API", "https://your-gateway.example/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.Bearer, "your-model",
                    "Replace the example host with the gateway you use; compatibility depends on its OpenAI implementation.",
                    ApiReasoningLevel.Off,
                    noteKey: "ProviderNote.NewApi", nameKey: "ProviderName.NewApi"),
                new ApiProviderPreset(
                    "Azure OpenAI", "https://your-resource.openai.azure.com/openai/v1", ApiProtocol.Responses,
                    ApiAuthenticationMode.ApiKeyHeader, "your-deployment",
                    "Use the resource v1 base URL and api-key header; the model ID is usually the deployment name.",
                    ApiReasoningLevel.Off,
                    noteKey: "ProviderNote.Azure", nameKey: "ProviderName.Azure"),
                new ApiProviderPreset(
                    "Custom OpenAI-compatible API", "https://your-gateway.example/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.Bearer, "your-model",
                    "Edit the base URL, protocol, authentication and model; native Anthropic/Gemini protocols are not handled directly.",
                    ApiReasoningLevel.Off,
                    noteKey: "ProviderNote.Custom", nameKey: "ProviderName.Custom")
            };
        }
    }
}
