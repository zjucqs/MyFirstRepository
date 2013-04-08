using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static readonly string CandidateSecond = @"思斯虒";
        static readonly string CandidateThird = @"涵齐奇琪琦绮";
        static readonly string All = @"梓歌萱紫桐凝桐宇
倪佳亿仪莟菡韩涵姿婵嫣婧娴婉姞妍妙珆琼瑶瑾瑞琦玫琪琳环琬瑗
薇珂茜芮菀菁苑芊茗荔菲蓓芃蔓莓渺漩漪冽霖馡馥晓枫巧嵘呤咛彨昭卿雁钰
怡梦希韵音思丝纤伊依烁炫煜烟炅然冉屏宛逸艺沂仪易依伊亦毅颐懿翊漪羿";

        static void Main(string[] args)
        {
            using (StreamWriter sw = new StreamWriter(File.Open("Good.txt", FileMode.Create)))
            {
                foreach (var s in CandidateSecond.ToCharArray())
                {
                    foreach (var t in CandidateThird.ToCharArray())
                    {
                        if (s.Equals(t) || s.Equals ('\r') || s.Equals ('\n') || t.Equals ('\r') || t.Equals ('\n'))
                        {
                            continue;
                        }

                        string name = s.ToString() + t.ToString();
                        double score = 0;
                        if (DoWebRequest(name, out score))
                        {
                            string output = name + ": " + score;
                            if (score > 94)
                            {
                                sw.WriteLine(output);
                                sw.Flush();
                            }

                            Console.WriteLine(output);
                        }
                        else
                        {
                            Thread.Sleep(30);
                            if (DoWebRequest(name, out score))
                            {
                                string output = name + ": " + score;
                                if (score > 98)
                                {
                                    sw.WriteLine(output);
                                }

                                Console.WriteLine(output);
                            }
                            else
                            {
                                Console.WriteLine(name + ": Failed");
                            }
                        }
                    }
                }
            }
        }



        private static bool DoWebRequest(string name, out double score)
        {
            bool succeeded = false;
            score = 0;

            try
            {
                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create("http://sina.sm.aqioo.com/XingMingPingFen.html");
                // Set the Method property of the request to POST.
                request.Method = "POST";
                // Create POST data and convert it to a byte array.
                string postData = string.Format("txtMFirstName=陈&txtMLastName={0}&sex1=女&year1=2013&month1=4&day1=14", name);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                string status = ((HttpWebResponse)response).StatusDescription;
                succeeded = status.Equals("OK", StringComparison.OrdinalIgnoreCase);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string textHit = "姓名评分：";
                //string allText = reader.ReadToEnd();

                //if(allText.Contains(textHit))
                //{
                //    Console.WriteLine(allText);
                //}

                string scoreLine = null;
                using (StreamWriter sw = new StreamWriter(File.Open("text.txt", FileMode.Append)))
                {
                    string line = reader.ReadLine();
                    while (null != line)
                    {
                        if (line.Trim().StartsWith(textHit))
                        {
                            scoreLine = line.Trim();
                            break;
                        }
                        line = reader.ReadLine();
                    }
                }

                if (!string.IsNullOrEmpty(scoreLine))
                {
                    string start = "姓名评分：<strong style=\"font-size: 30px; color: red;\">";
                    scoreLine = scoreLine.Substring(start.Length);
                    scoreLine = scoreLine.Substring(0, scoreLine.IndexOf("<"));
                    double.TryParse(scoreLine, out score);
                }

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch
            { }

            return succeeded;
        }
    }
}
