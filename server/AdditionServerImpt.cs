using Grpc.Core;
using Sum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sum.AdditionService;

namespace server
{
    public class AdditionServerImpt : AdditionServiceBase
    {
        public override Task<AdditionResponse> Sum(AdditionRequest request, ServerCallContext context)
        {
            int result = 0;

            result = Convert.ToInt32(request.ValuePackage.ValueOne) + Convert.ToInt32(request.ValuePackage.ValueTwo);
;          
            return Task.FromResult(new AdditionResponse() { Result = result });
        }
    }
}
