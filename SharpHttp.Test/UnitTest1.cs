using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace SharpHttp.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var client = new SharpHttpClient();

            client.SetProxy("127.0.0.1", 8087);
            client.UserProxy = false;
            var task = client.GetAsync("https://www.baidu.com");
            task.Wait();

            Assert.Equals(task.Result.StatusCode, HttpStatusCode.OK);
        }
    }
}
