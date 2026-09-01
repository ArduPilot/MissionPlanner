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

    public sealed class ApiConnectionSettings
    {
        public string BaseUrl { get; set; }
        public ApiProtocol Protocol { get; set; }
        public ApiAuthenticationMode AuthenticationMode { get; set; }
        public string Model { get; set; }
        public string ApiKey { get; set; }
        public string ProjectId { get; set; }

        public Uri BuildEndpoint(string relativePath)
        {
            Uri baseUri = ValidateBaseUrl(BaseUrl);
            string normalized = baseUri.AbsoluteUri.TrimEnd('/') + "/";
            return new Uri(new Uri(normalized, UriKind.Absolute), relativePath.TrimStart('/'));
        }

        public void Validate()
        {
            ValidateBaseUrl(BaseUrl);
            if (string.IsNullOrWhiteSpace(Model))
                throw new ArgumentException("模型 ID 不能为空。", "Model");
            if (AuthenticationMode != ApiAuthenticationMode.None && string.IsNullOrWhiteSpace(ApiKey))
                throw new ArgumentException("当前鉴权方式需要 API 密钥。", "ApiKey");
        }

        public static Uri ValidateBaseUrl(string baseUrl)
        {
            Uri uri;
            if (string.IsNullOrWhiteSpace(baseUrl) ||
                !Uri.TryCreate(baseUrl.Trim(), UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("API Base URL 必须是完整的 http 或 https 地址。", "baseUrl");
            }

            if (!string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
                throw new ArgumentException("API Base URL 不能包含查询参数或片段。", "baseUrl");

            if (uri.Scheme == Uri.UriSchemeHttp && !uri.IsLoopback)
                throw new ArgumentException("仅本机回环地址允许使用明文 HTTP；远程 API 必须使用 HTTPS。", "baseUrl");

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
            string note)
        {
            Name = name;
            BaseUrl = baseUrl;
            Protocol = protocol;
            AuthenticationMode = authenticationMode;
            Model = model;
            Note = note;
        }

        public string Name { get; private set; }
        public string BaseUrl { get; private set; }
        public ApiProtocol Protocol { get; private set; }
        public ApiAuthenticationMode AuthenticationMode { get; private set; }
        public string Model { get; private set; }
        public string Note { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public static IList<ApiProviderPreset> CreateDefaults()
        {
            return new List<ApiProviderPreset>
            {
                new ApiProviderPreset(
                    "CC Switch（本机）", "http://127.0.0.1:15721/v1", ApiProtocol.Responses,
                    ApiAuthenticationMode.None, "gpt-5.6-sol",
                    "需先在 CC Switch 中启动代理与对应路由；上游密钥或 Codex OAuth 由 CC Switch 管理。"),
                new ApiProviderPreset(
                    "OpenAI 官方", "https://api.openai.com/v1", ApiProtocol.Responses,
                    ApiAuthenticationMode.Bearer, "gpt-5.2",
                    "使用 OpenAI API 密钥；ChatGPT/Codex 登录本身不等于 API 密钥。"),
                new ApiProviderPreset(
                    "OpenRouter", "https://openrouter.ai/api/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.Bearer, "openai/gpt-5.2",
                    "使用 OpenRouter 密钥；模型 ID 以 OpenRouter 当前目录为准。"),
                new ApiProviderPreset(
                    "LiteLLM Proxy（本机）", "http://127.0.0.1:4000/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.Bearer, "gpt-5.2",
                    "适用于本机 LiteLLM Proxy；未启用代理密钥时可改为“无需鉴权”。"),
                new ApiProviderPreset(
                    "LM Studio（本机）", "http://127.0.0.1:1234/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.None, "local-model",
                    "需在 LM Studio 中加载模型并启动 Local Server。"),
                new ApiProviderPreset(
                    "Ollama（本机）", "http://127.0.0.1:11434/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.None, "qwen3",
                    "需启动 Ollama，并把模型 ID 改为本机已经拉取的模型。"),
                new ApiProviderPreset(
                    "New API / One API", "https://your-gateway.example/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.Bearer, "your-model",
                    "将示例域名替换为实际网关；兼容性取决于网关的 OpenAI 接口实现。"),
                new ApiProviderPreset(
                    "Azure OpenAI", "https://your-resource.openai.azure.com/openai/v1", ApiProtocol.Responses,
                    ApiAuthenticationMode.ApiKeyHeader, "your-deployment",
                    "填写资源的 v1 Base URL，并使用 api-key 请求头；模型 ID 通常为部署名。"),
                new ApiProviderPreset(
                    "自定义 OpenAI 兼容接口", "https://your-gateway.example/v1", ApiProtocol.ChatCompletions,
                    ApiAuthenticationMode.Bearer, "your-model",
                    "可编辑 Base URL、协议、鉴权方式和模型；不直接支持 Anthropic/Gemini 原生协议。")
            };
        }
    }
}
