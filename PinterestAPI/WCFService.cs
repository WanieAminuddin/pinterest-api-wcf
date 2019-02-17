using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace PinterestService
{
    [ServiceContract]
    public interface PinterestUser
    {
        [OperationContract]
        string PinterestProfile();

    }

    public class PinterestService : PinterestUser
    {
        public PinterestService() { }
        public static string PinterestXML;

        public void getPinterestUser(string userxml)
        {
            PinterestXML = userxml;
        }
        public string PinterestProfile()
        {
            return PinterestXML;
        }
    }


}
