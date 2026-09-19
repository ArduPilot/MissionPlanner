using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public static class LLMTranslationService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string apiKey = "";
        // 默认使用标准 OpenAI 兼容端点；任何兼容实现的 base url 都可在设置里覆盖
        // (OpenAI / Azure OpenAI / DeepSeek / Ollama / vLLM / LM Studio ...)
        internal const string DefaultApiUrl = "https://api.openai.com/v1/chat/completions";
        internal const string DefaultModel = "gpt-4o-mini";
        private static string apiUrl = DefaultApiUrl;
        private static string modelName = "";
        private static double temperature = 1.3;
        private static bool isConfigured = false;
        private const int MaxRetries = 2;

        // 自定义系统提示词（默认使用标准提示）
        private static string _customSystemPrompt = GetDefaultSystemPrompt();

        /// <summary>附在译文之后的原文前缀；界面层可按当前 UI 语言覆盖（见 LLMConfigDialog）。</summary>
        public static string OriginalTextLabel { get; set; } = "Original: ";

        public static event Action<string, string> TranslationCompleted;
        public static string LastDebugMessage { get; private set; } = string.Empty;

        static LLMTranslationService()
        {
            httpClient.Timeout = TimeSpan.FromSeconds(45);
        }

        private static void SetDebugMessage(string message)
        {
            LastDebugMessage = message ?? string.Empty;
            Debug.WriteLine("[LLM] " + LastDebugMessage);
            Console.WriteLine("[LLM] " + LastDebugMessage);
        }

        // 默认系统提示
        private static string GetDefaultSystemPrompt()
        {
            return "You are a professional UAV technical document translator. " +
                   "Translate the following ArduPilot parameter description from English to Simplified Chinese. " +
                   "Keep technical terms accurate. Output ONLY the Chinese translation, no explanation.";
        }

        // 设置自定义系统提示词（传入 null 或空白则恢复默认）
        public static void SetSystemPrompt(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                _customSystemPrompt = GetDefaultSystemPrompt();
            else
                _customSystemPrompt = prompt.Trim();

            SetDebugMessage($"System prompt updated. Length={_customSystemPrompt.Length}");
        }

        public static void Configure(string key, string url)
        {
            Configure(key, url, null);
        }

        public static void Configure(string key, string url, string model)
        {
            Configure(key, url, model, null);
        }

        public static void Configure(string key, string url, string model, double? customTemperature)
        {
            apiKey = (key ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(url))
                apiUrl = NormalizeApiUrl(url);
            if (!string.IsNullOrWhiteSpace(model))
                modelName = model.Trim();
            if (customTemperature.HasValue)
                temperature = NormalizeTemperature(customTemperature.Value);

            isConfigured = !string.IsNullOrWhiteSpace(apiKey);
            SetDebugMessage($"Configure called. Url={apiUrl}, Model={GetEffectiveModel(apiUrl)}, Temperature={temperature.ToString("0.0#", CultureInfo.InvariantCulture)}, KeyLength={(apiKey ?? string.Empty).Length}, Enabled={isConfigured}");
        }

        public static void SetModel(string model)
        {
            modelName = string.IsNullOrWhiteSpace(model) ? string.Empty : model.Trim();
            SetDebugMessage($"Model updated: {GetEffectiveModel(apiUrl)}");
        }

        public static void SetTemperature(double customTemperature)
        {
            temperature = NormalizeTemperature(customTemperature);
            SetDebugMessage($"Temperature updated: {temperature.ToString("0.0#", CultureInfo.InvariantCulture)}");
        }

        public static bool IsConfigured => isConfigured;

        public static async Task<string> TranslateAsync(string englishText, string paramName = "")
        {
            if (!isConfigured || string.IsNullOrEmpty(englishText))
                return englishText;

            try
            {
                var json = BuildChatRequestJson(englishText, 500, true, apiUrl);
                foreach (var requestUrl in GetCandidateUrls(apiUrl))
                {
                    SetDebugMessage($"TranslateAsync start. Param={paramName}, Url={requestUrl}, TextLength={englishText.Length}");

                    using (var response = await PostWithRetryAsync(requestUrl, json, "TranslateAsync", paramName))
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            SetDebugMessage($"TranslateAsync failed. Param={paramName}, Url={requestUrl}, Status={(int)response.StatusCode} {response.StatusCode}, Response={responseJson}");
                            continue;
                        }

                        var content = ExtractAssistantContent(responseJson);
                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            if (requestUrl != apiUrl)
                                apiUrl = requestUrl;

                            string translated = content.Trim() + Environment.NewLine + OriginalTextLabel + englishText;
                            SetDebugMessage($"TranslateAsync success. Param={paramName}, Url={requestUrl}, ResultLength={translated.Length}");
                            TranslationCompleted?.Invoke(paramName, translated);
                            return translated;
                        }

                        SetDebugMessage($"TranslateAsync returned empty content. Param={paramName}, Url={requestUrl}, Response={responseJson}");
                    }
                }
            }
            catch (Exception ex)
            {
                SetDebugMessage($"TranslateAsync exception. Param={paramName}, Error={ex}");
                Console.WriteLine($"Translation error for {paramName}: {ex.Message}");
            }

            return englishText;
        }

        public static async Task<bool> TestConnection()
        {
            if (!isConfigured)
            {
                SetDebugMessage("TestConnection skipped: service is not configured.");
                return false;
            }

            try
            {
                var json = BuildChatRequestJson("Hello", 5, false, apiUrl);
                foreach (var requestUrl in GetCandidateUrls(apiUrl))
                {
                    SetDebugMessage($"TestConnection start. Url={requestUrl}, KeyLength={(apiKey ?? string.Empty).Length}");

                    using (var response = await PostWithRetryAsync(requestUrl, json, "TestConnection", string.Empty))
                    {
                        var responseText = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            SetDebugMessage($"TestConnection failed. Url={requestUrl}, Status={(int)response.StatusCode} {response.StatusCode}, Response={responseText}");
                            continue;
                        }

                        var content = ExtractAssistantContent(responseText);
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            SetDebugMessage($"TestConnection invalid response body. Url={requestUrl}, Response={responseText}");
                            continue;
                        }

                        if (requestUrl != apiUrl)
                            apiUrl = requestUrl;

                        SetDebugMessage($"TestConnection success. Url={requestUrl}, Status={(int)response.StatusCode} {response.StatusCode}");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                SetDebugMessage($"TestConnection exception. Error={ex}");
                Console.WriteLine($"Test connection failed: {ex.Message}");
                return false;
            }
        }

        // 批量翻译（带限流）
        public static async Task BatchTranslateAsync(Dictionary<string, string> textsToTranslate,
            Action<int, int> progressCallback = null)
        {
            var items = new List<KeyValuePair<string, string>>(textsToTranslate);
            int completed = 0;

            foreach (var item in items)
            {
                string translated = await TranslateAsync(item.Value, item.Key);
                textsToTranslate[item.Key] = translated;
                completed++;
                progressCallback?.Invoke(completed, items.Count);

                // 避免 API 限流，每秒最多30次
                await Task.Delay(30);
            }
        }

        private static string BuildChatRequestJson(string userContent, int maxTokens, bool includeSystemPrompt, string requestUrl)
        {
            var messages = new List<object>();
            if (includeSystemPrompt && !string.IsNullOrWhiteSpace(_customSystemPrompt))
                messages.Add(new { role = "system", content = _customSystemPrompt });

            messages.Add(new { role = "user", content = userContent });

            var requestBody = new Dictionary<string, object>();
            if (!IsAzureOpenAI(requestUrl))
                requestBody["model"] = GetEffectiveModel(requestUrl);

            requestBody["messages"] = messages;
            requestBody["temperature"] = temperature;
            requestBody["max_tokens"] = maxTokens;

            return JsonSerializer.Serialize(requestBody);
        }

        private static async Task<HttpResponseMessage> PostWithRetryAsync(string requestUrl, string json,
            string operation, string paramName)
        {
            Exception lastError = null;

            for (var attempt = 0; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    using (var request = CreateRequestMessage(requestUrl, json))
                    {
                        var response = await httpClient.SendAsync(request);
                        if (ShouldRetry(response.StatusCode) && attempt < MaxRetries)
                        {
                            var delay = GetRetryDelayMilliseconds(attempt);
                            SetDebugMessage($"{operation} retrying. Param={paramName}, Url={requestUrl}, Attempt={attempt + 1}, Status={(int)response.StatusCode}, DelayMs={delay}");
                            response.Dispose();
                            await Task.Delay(delay);
                            continue;
                        }

                        return response;
                    }
                }
                catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
                {
                    lastError = ex;
                    if (attempt >= MaxRetries)
                        throw;

                    var delay = GetRetryDelayMilliseconds(attempt);
                    SetDebugMessage($"{operation} transient exception. Param={paramName}, Url={requestUrl}, Attempt={attempt + 1}, DelayMs={delay}, Error={ex.Message}");
                    await Task.Delay(delay);
                }
            }

            throw lastError ?? new InvalidOperationException("HTTP request failed after retries.");
        }

        private static HttpRequestMessage CreateRequestMessage(string requestUrl, string json)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            if (IsAzureOpenAI(requestUrl))
                request.Headers.Add("api-key", apiKey);
            else
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            return request;
        }

        private static bool ShouldRetry(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.RequestTimeout ||
                   statusCode == (HttpStatusCode)429 ||
                   statusCode == HttpStatusCode.BadGateway ||
                   statusCode == HttpStatusCode.ServiceUnavailable ||
                   statusCode == HttpStatusCode.GatewayTimeout ||
                   (int)statusCode >= 500;
        }

        private static int GetRetryDelayMilliseconds(int attempt)
        {
            var baseDelay = (int)Math.Pow(2, attempt) * 400;
            return Math.Min(baseDelay, 3000);
        }

        private static string ExtractAssistantContent(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson))
                return null;

            try
            {
                using (var document = JsonDocument.Parse(responseJson))
                {
                    var root = document.RootElement;

                    JsonElement choices;
                    if (root.TryGetProperty("choices", out choices) && choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0)
                    {
                        var firstChoice = choices[0];

                        JsonElement message;
                        if (firstChoice.TryGetProperty("message", out message))
                        {
                            JsonElement content;
                            if (message.TryGetProperty("content", out content))
                            {
                                var messageContent = ReadContentElement(content);
                                if (!string.IsNullOrWhiteSpace(messageContent))
                                    return messageContent;
                            }
                        }

                        JsonElement text;
                        if (firstChoice.TryGetProperty("text", out text) && text.ValueKind == JsonValueKind.String)
                            return text.GetString();
                    }

                    JsonElement outputText;
                    if (root.TryGetProperty("output_text", out outputText) && outputText.ValueKind == JsonValueKind.String)
                        return outputText.GetString();

                    JsonElement contentArray;
                    if (root.TryGetProperty("content", out contentArray) && contentArray.ValueKind == JsonValueKind.Array && contentArray.GetArrayLength() > 0)
                    {
                        var firstPart = contentArray[0];
                        JsonElement text;
                        if (firstPart.TryGetProperty("text", out text) && text.ValueKind == JsonValueKind.String)
                            return text.GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                SetDebugMessage($"ExtractAssistantContent parse failed: {ex.Message}");
            }

            return null;
        }

        private static string ReadContentElement(JsonElement content)
        {
            if (content.ValueKind == JsonValueKind.String)
                return content.GetString();

            if (content.ValueKind != JsonValueKind.Array)
                return null;

            var sb = new StringBuilder();
            foreach (var part in content.EnumerateArray())
            {
                JsonElement text;
                if (part.ValueKind == JsonValueKind.Object && part.TryGetProperty("text", out text) && text.ValueKind == JsonValueKind.String)
                {
                    if (sb.Length > 0)
                        sb.AppendLine();
                    sb.Append(text.GetString());
                }
            }

            return sb.ToString();
        }

        private static string GetEffectiveModel(string url)
        {
            if (!string.IsNullOrWhiteSpace(modelName))
                return modelName;

            return DefaultModel;
        }

        private static double NormalizeTemperature(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return 1.3;

            if (value < 0)
                return 0;

            if (value > 2)
                return 2;

            return value;
        }

        private static bool IsAzureOpenAI(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            var host = (uri.Host ?? string.Empty).ToLowerInvariant();
            var path = (uri.AbsolutePath ?? string.Empty).ToLowerInvariant();
            return host.Contains("openai.azure.com") || path.Contains("/openai/deployments/");
        }

        private static string NormalizeApiUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return DefaultApiUrl;

            var normalized = url.Trim();
            Uri uri;
            if (!Uri.TryCreate(normalized, UriKind.Absolute, out uri))
                return normalized.TrimEnd('/');

            var path = (uri.AbsolutePath ?? string.Empty).TrimEnd('/');

            if (path.EndsWith("/chat/completions", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("/v1/chat/completions", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/openai/deployments/"))
                return normalized;

            var builder = new UriBuilder(uri);
            if (string.IsNullOrEmpty(path) || path == "/")
            {
                builder.Path = "/v1/chat/completions";
                return builder.Uri.ToString().TrimEnd('/');
            }

            if (path.EndsWith("/v1", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("/openai/v1", StringComparison.OrdinalIgnoreCase))
            {
                builder.Path = path + "/chat/completions";
                return builder.Uri.ToString().TrimEnd('/');
            }

            return normalized.TrimEnd('/');
        }

        private static IEnumerable<string> GetCandidateUrls(string url)
        {
            var candidates = new List<string>();
            var normalized = NormalizeApiUrl(url);
            candidates.Add(normalized);

            if (IsAzureOpenAI(normalized))
                return candidates;

            if (normalized.EndsWith("/v1/chat/completions", StringComparison.OrdinalIgnoreCase))
            {
                candidates.Add(normalized.Substring(0, normalized.Length - "/v1/chat/completions".Length) + "/chat/completions");
            }
            else if (normalized.EndsWith("/chat/completions", StringComparison.OrdinalIgnoreCase) &&
                     !normalized.EndsWith("/v1/chat/completions", StringComparison.OrdinalIgnoreCase))
            {
                candidates.Add(normalized.Substring(0, normalized.Length - "/chat/completions".Length) + "/v1/chat/completions");
            }

            return candidates.Where(a => !string.IsNullOrWhiteSpace(a)).Distinct().ToArray();
        }
    }
}