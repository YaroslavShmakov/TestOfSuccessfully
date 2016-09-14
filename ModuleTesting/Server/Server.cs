using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using RESTserver;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            string restURL = "http://localhost:8002/";

            try
            {
                RESTReciver.RunRESTReciverControler(restURL);
                Console.WriteLine("The RESTReciever is ready.");
                Console.ReadLine();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("error");
                Console.WriteLine(e.Message);
            }

        }
    }
}

