using NBench;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataLayer;

namespace TaskManager.PerfTest
{
    public class PerformanceTest
    {
        private HttpClient _client;
        private HttpResponseMessage _response;
        private const string serviceBaseURL = "http://172.18.4.96/api/task/";

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _client = new HttpClient { BaseAddress = new Uri(serviceBaseURL) };
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
        NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.LessThanOrEqualTo, ByteConstants.ThirtyTwoKb)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 0.0d)]
        public void Benchmark_GetTaskDetails()
        {
            _response = _client.GetAsync(serviceBaseURL).Result;
            var responseResult =
                JsonConvert.DeserializeObject<List<view_TaskDetails>>(_response.Content.ReadAsStringAsync().Result);
        }
    }
}
