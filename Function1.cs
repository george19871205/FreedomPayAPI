using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using Microsoft.WindowsAzure.Storage;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace HrLeaveRequestListFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "data")] HttpRequestMessage req)
        {
            string body = await req.Content.ReadAsStringAsync();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(body);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://cs.uat.freedompay.com/Freeway/Service.asmx");
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(doc.OuterXml);
            request.Method = "POST";
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader responseStream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
                string receivedResponse = responseStream.ReadToEnd();
                return req.CreateResponse(HttpStatusCode.OK, receivedResponse);
            }
            return null;
        }
    }
}

