﻿using NUnit.Framework;
using RestSharp;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMockNetExamples
{
    [TestFixture]
    public class BasicExample
    {
        private WireMockServer server;

        private RestClient client;

        private const string BASE_URL = "http://localhost:9876";
        private const string BASE_URL_LIVE =  "https://mox-referrals.ceta.co.uk";


        [OneTimeSetUp]
        public void SetupRestSharpClient()
        {
            client = new RestClient(BASE_URL);
        }

        [SetUp]
        public void StartServer()
        {
            server = WireMockServer.Start(port: 9876);
        }

        private void CreateHelloWorldStub()
        {
            server.Given(
                Request.Create().WithPath("/hello-world").UsingGet()
            )
            .RespondWith(
                Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/plain")
                .WithBody("Hello, world!")
            );
        }

        private void ForwardToLiveApiUrl()
        {
            server
              .Given(
                Request.Create()
                  .WithPath("/live/*")
              )
              .AtPriority(10)
              .RespondWith(
                Response.Create()
                  .WithProxy("https://mox-referrals.ceta.co.uk")
              );
        }

        [Test]
        public async Task TestHelloWorldStub()
        {
            CreateHelloWorldStub();

            RestRequest request = new RestRequest("/hello-world", Method.Get);

            RestResponse response = await client.ExecuteAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.ContentType, Is.EqualTo("text/plain"));
            Assert.That(response.Content, Is.EqualTo("Hello, world!"));
        }

        [TearDown]
        public void StopServer()
        {
            server.Stop();
        }

    }
}