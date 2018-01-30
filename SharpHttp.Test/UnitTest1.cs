using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;

namespace SharpHttp.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var uri = new Uri("http://whwdev03.ciic.com:8082/index.php/bug/list/29?query_id=-2");
            var t1 = Path.Combine("c://abc", "ef");
            var t2 = Path.Combine("c://abc", "/ef");

            var client = new SharpHttpClient();

            //client.SetProxy("127.0.0.1", 8087);
            //client.UserProxy = false;
            //client.SetUserAgent(UserAgent.BAIDU_SPIDER);
            var task = client.GetAsync("http://ai010.top/forum.php?mod=viewthread&tid=455674&extra=page%3D1&mobile=2");
            task.Wait();

            Assert.Equals(task.Result.StatusCode, HttpStatusCode.OK);
        }
    }
}
