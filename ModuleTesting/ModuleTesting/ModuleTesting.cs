using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace ModuleTesting
{
    public class Testing
    {
        private enum State { wait, testReady, resultReady };

        private static Testing moduleTesting;
        private object forLock = new object();

        Dictionary<int, State> clients;
        Dictionary<int, List<int>> tests;
        Dictionary<int, List<int>> results;

        private Testing()
        {
            clients = new Dictionary<int, State>();
            tests = new Dictionary<int, List<int>>();
            results = new Dictionary<int, List<int>>();
        }

        public static Testing GetInstance()
        {
            if (moduleTesting == null)
                moduleTesting = new Testing();

            return moduleTesting;
        }

        public int Subscribe()
        {
            int id = 0;

            lock (forLock)
            {
                if (clients.Count > 0)
                    id = clients.Keys.Max() + 1;
                else
                    id = 1;

                clients.Add(id, State.wait);
            }

            return id;
        }

        public bool Unsubscribe(int id)
        {
            bool isRemove = false;

            lock (forLock)
            {
                isRemove = clients.Remove(id);
                tests.Remove(id);
                results.Remove(id);
            }

            return isRemove;
        }

        public void CreateTests(int id, int[] result)
        {
            lock (forLock)
            {
                if (clients.ContainsKey(id))
                {
                    clients[id] = State.wait;
                    List<int> testNumber = new List<int>();

                    int[] needTest = TestLusher(result);

                    for (int i = 0; i < 8; ++i)
                        if (needTest[i] != 0)
                            testNumber.Add(i);

                    tests.Add(id, testNumber);
                    clients[id] = State.testReady;
                }
            }
        }

        public bool TestsReady(int id)
        {
            bool isReady = false;

            lock (forLock)
            {
                if (tests.ContainsKey(id) && clients.ContainsKey(id))
                    if (clients[id] == State.testReady)
                        isReady = true;
            }

            return isReady;
        }

        public string[] GetQuestions(int id)
        {
            List<string> questions = new List<string>();

            lock (forLock)
            {
                if (tests.ContainsKey(id) && clients.ContainsKey(id))
                {
                    foreach (int testNumber in tests[id])
                    {
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load("tests.xml");
                        XmlElement xRoot = xDoc.DocumentElement;

                        XmlNodeList xTest = xRoot.ChildNodes;

                        XmlNode currentTest = xTest[testNumber];

                        XmlNodeList testQuestion = currentTest.ChildNodes;

                        foreach (XmlNode quest in testQuestion)
                            questions.Add(quest.InnerText);
                    }
                }
            }

            string[] str = new string[questions.Count];
            for (int i = 0; i < questions.Count; ++i)
                str[i] = questions[i];

            return str;
        }

        public void GetGradeLucky(int id, int[] result)
        {
            lock (forLock)
            {
                if (tests.ContainsKey(id) && clients.ContainsKey(id))
                {
                    List<int> res = new List<int>();

                    foreach (int i in result)
                        res.Add(i);

                    results.Add(id, res);
                    clients[id] = State.resultReady;
                }
            }
        }

        public bool ResultsReady(int id)
        {
            bool isReady = false;

            lock (forLock)
            {
                if (results.ContainsKey(id) && clients.ContainsKey(id))
                    if (clients[id] == State.resultReady)
                        isReady = true;
            }

            return isReady;
        }

        public string GetFinishResult(int id)
        {
            string res = "";

            lock (forLock)
            {
                if (results.ContainsKey(id) && tests.ContainsKey(id) && clients.ContainsKey(id))
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load("tests.xml");
                    XmlElement xRoot = xDoc.DocumentElement;

                    XmlNodeList xTest = xRoot.ChildNodes;
                    List<int> newResult = new List<int>();
                    int i = 0;

                    foreach (int currentTest in tests[id]) // получаем номер теста
                    {
                        int sum = 0;

                        for (int j = 0; j < xTest[currentTest].ChildNodes.Count; ++j, ++i) // проходим по ответам конкретного блока и суммируем ответы
                            sum += results[id][i];

                        newResult.Add(sum);
                    }

                    results[id] = newResult;

                    res += GetLucky(id) + '\n';

                    for (int j = 0; j < tests[id].Count; ++j)
                        res += GetResultByCharactBlock(tests[id][j], results[id][j]);
                }
            }
            return res;
        }

        private string GetResultByCharactBlock(int testNumber, int summa)
        {
            string res = "";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("results.xml");

            XmlElement xRoot = xDoc.DocumentElement;

            XmlNodeList xAnswers = xRoot.ChildNodes;

            XmlNode currentAnswer = xAnswers[testNumber];

            for (int i = 0; i < currentAnswer.ChildNodes.Count; ++i)
            {
                XmlAttributeCollection ansAttr = currentAnswer.ChildNodes[i].Attributes;
                string min = ansAttr[0].Value;
                string max = ansAttr[1].Value;

                if (Convert.ToInt32(min) <= summa && summa <= Convert.ToInt32(max))
                {
                    res += currentAnswer.ChildNodes[i].InnerText;
                    res += "<br><br>";
                    break;
                }
            }

            return res;
        }

        private string GetLucky(int id)
        {
            string luckyStr = "";
            int summa = 0;
            int maxSumma = 0;
            double lucky = 0;
            int[] resTmp = new int [8];
            int i = 0;

            foreach (int res in results[id])
            {
                resTmp[i] = res;
               // summa += res;
                ++i;
            }

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("results.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNodeList xAnswers = xRoot.ChildNodes;

            i = 0;
            foreach (int index in tests[id])
            {
                if(Convert.ToInt32(xAnswers[index].Attributes[1].Value) == 1)
                    resTmp[i]=Convert.ToInt32(xAnswers[index].Attributes[0].Value) - resTmp[i];
                maxSumma += Convert.ToInt32(xAnswers[index].Attributes[0].Value);
                ++i;
            }
            i = 0;
            for (; i < 8; ++i)
                summa += resTmp[i];

            lucky = (double)summa / (double)maxSumma;
            lucky = (int)(lucky * 100);
            luckyStr +="Ваша везучесть составляет " + lucky.ToString() + "%" + " ";

            xDoc.Load("lucky.xml");
            xRoot = xDoc.DocumentElement;

            XmlNodeList xDegrees = xRoot.ChildNodes;

            foreach (XmlNode ans in xDegrees)
            {
                XmlAttributeCollection ansAttr = ans.Attributes;
                int min = Convert.ToInt32(ansAttr[0].Value);
                int max = Convert.ToInt32(ansAttr[1].Value);

                if (min <= lucky && lucky <= max)
                {
                    luckyStr += ans.InnerText + "<br><br>";
                    break;
                }
            }
            StreamWriter file = new StreamWriter("resLucky.txt", true);
            file.WriteLine(lucky);
            file.Close();
            return luckyStr;
        }

        private int[] TestLusher(int[] rangeColors)
        {
            int[] resArray = new int[8];

            int i = 0;

            for (; i < 4; ++i)
            {
                if (rangeColors[i] < 5)
                    resArray[rangeColors[i] - 1] = 1;
                else
                    resArray[rangeColors[i] - 1] = 0;
            }

            for (; i < 8; ++i)
            {
                if (rangeColors[i] > 4)
                    resArray[rangeColors[i] - 1] = 1;
                else
                    resArray[rangeColors[i] - 1] = 0;
            }
            return resArray;
        }
    }
}
