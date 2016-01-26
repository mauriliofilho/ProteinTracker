using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

namespace ProteinTrackerAPI.Api
{
    public class HelloService : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = "Hello, " + request.Name };
        }
    }

    //Request DTO
    [Route("/hello")]
    [Route("/hello/{Name}")]
    public class Hello
    {
        public string Name { get; set; }
    }

    //Response DTO
    //Follows naming convention
    public class HelloResponse
    {
        public ResponseStatus ResponseStatus { get; set; } //Automatic exception handling

        public string Result { get; set; }
    }
}