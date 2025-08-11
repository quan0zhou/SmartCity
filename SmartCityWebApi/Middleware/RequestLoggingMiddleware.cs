using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SmartCityWebApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly IDistributedCache _cache;
        private readonly int _requestLimit;
        private readonly int _timeWindowSeconds;
        private readonly HashSet<string> _whitelistPaths;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IDistributedCache cache)
        {
            _next = next;
            _logger = logger;
            _cache = cache;
            _requestLimit = 100; // 默认100次请求
            _timeWindowSeconds = 60; // 默认1分钟窗口期

            // 添加白名单路径，这些路径不受请求限制
            _whitelistPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "/api/mobile/order/notify"  // 微信支付回调路径
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value ?? string.Empty;

            // 检查路径是否在白名单中
            if (_whitelistPaths.Contains(path))
            {
                _logger.LogInformation($"白名单路径请求: {path}");
                await _next(context);
                return;
            }

            // 获取客户端IP
            string ip = GetClientIpAddress(context.Request);

            // 检查IP是否在黑名单中
            string blacklistKey = $"ip:blacklist:{ip}";
            if (await _cache.GetStringAsync(blacklistKey) != null)
            {
                _logger.LogWarning($"拒绝访问: IP {ip} 在黑名单中");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("您的IP已被临时限制访问，请稍后再试");
                return;
            }

            // 增加请求计数
            string counterKey = $"ip:counter:{ip}";
            string counterValue = await _cache.GetStringAsync(counterKey);
            int requestCount = 1;

            if (!string.IsNullOrEmpty(counterValue) && int.TryParse(counterValue, out int count))
            {
                requestCount = count + 1;
            }

            // 设置计数器，过期时间为窗口期
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_timeWindowSeconds)
            };
            await _cache.SetStringAsync(counterKey, requestCount.ToString(), options);

            // 检查是否超过请求限制
            if (requestCount > _requestLimit)
            {
                _logger.LogWarning($"检测到爬虫行为: IP {ip} 在 {_timeWindowSeconds}秒内发送了 {requestCount} 个请求");

                // 将IP加入黑名单，过期时间为1天
                var blacklistOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                };
                await _cache.SetStringAsync(blacklistKey, DateTime.UtcNow.ToString(), blacklistOptions);

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("检测到异常请求行为，您的IP已被临时限制访问");
                return;
            }

            // 记录请求信息
            var requestInfo = await FormatRequest(context.Request);
            _logger.LogWarning($"请求信息: {requestInfo}");

            // 调用管道中的下一个中间件
            await _next(context);
        }

        private string GetClientIpAddress(HttpRequest request)
        {
            // 获取客户端IP地址
            var ip = request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // 检查 X-Forwarded-For 头，适用于使用代理的情况
            if (request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ip = request.Headers["X-Forwarded-For"].ToString();
            }

            return ip;
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            // 获取客户端IP地址
            var ip = GetClientIpAddress(request);

            var sb = new StringBuilder();
            sb.AppendLine($"IP: {ip}");
            sb.AppendLine($"方法: {request.Method}");
            sb.AppendLine($"路径: {request.Path}");
            sb.AppendLine($"查询参数: {request.QueryString}");

            // 对于POST/PUT/PATCH请求，记录请求体
            if (request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH")
            {
                request.EnableBuffering();

                using var reader = new StreamReader(
                    request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);

                var body = await reader.ReadToEndAsync();
                sb.AppendLine($"请求体: {body}");

                // 重置请求体位置，以便后续中间件可以读取
                request.Body.Position = 0;
            }

            return sb.ToString();
        }
    }

    // 扩展方法，方便在Program.cs中注册
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}