using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Service
{
    public class LoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var startTime = DateTime.Now;
            string requestBody = string.Empty;
            string responseBody = string.Empty;

            try
            {
                // 读取请求Body
                if (request.Content != null)
                    requestBody = await request.Content.ReadAsStringAsync();

                Log.Information($"""
================ 第三方接口请求开始 ================
Url: {request.RequestUri}
Method: {request.Method}
Headers: {string.Join(";", request.Headers.Select(x => $"{x.Key}:{string.Join(",", x.Value)}"))}
Body: {requestBody}
StartTime: {startTime:yyyy-MM-dd HH:mm:ss.fff}
====================================================
""");

                var response = await base.SendAsync(request, cancellationToken);

                // 读取响应Body
                if (response.Content != null)
                    responseBody = await response.Content.ReadAsStringAsync();

                var endTime = DateTime.Now;
                var cost = (endTime - startTime).TotalMilliseconds;

                Log.Information($"""
================ 第三方接口响应结束 ================
Url: {request.RequestUri}
StatusCode: {(int)response.StatusCode}
ResponseBody: {responseBody}
StartTime: {startTime:yyyy-MM-dd HH:mm:ss.fff}
EndTime: {endTime:yyyy-MM-dd HH:mm:ss.fff}
Cost: {cost} ms
====================================================
""");

                // ----------------------------
                // 非 2xx 返回 HTTP 200 + 原始内容
                // ----------------------------
                if (!response.IsSuccessStatusCode)
                {
                    var newResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(responseBody, Encoding.UTF8, response.Content?.Headers.ContentType?.MediaType ?? "application/json"),
                        RequestMessage = request,
                        ReasonPhrase = response.ReasonPhrase,
                        Version = response.Version
                    };

                    return newResponse;
                }
                stopwatch.Stop();
                Log.Debug($"接口调用总耗时: {stopwatch.ElapsedMilliseconds} ms");
                return response;
            }
            catch (Exception ex)
            {
                var endTime = DateTime.Now;
                var cost = (endTime - startTime).TotalMilliseconds;

                Log.Error($"""
================ 第三方接口异常 ================
Url: {request.RequestUri}
Method: {request.Method}
RequestBody: {requestBody}
Cost: {cost} ms
Exception: {ex.Message}
================================================
""");

                // 返回 HTTP 200 + ApiResult<T> JSON
                var errorContent = JsonConvert.SerializeObject(new
                {
                    Code = 500,
                    Result = (object?)null,
                    Message = ex.Message,
                    Success = false
                });

                stopwatch.Stop();
                Log.Debug($"接口调用总耗时: {stopwatch.ElapsedMilliseconds} ms");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(errorContent, Encoding.UTF8, "application/json"),
                    RequestMessage = request
                };
            }
        }
    }
}
