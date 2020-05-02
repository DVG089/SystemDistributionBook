using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MySql.Data.MySqlClient;

namespace WcfServiceSDB
{
    [ServiceContract]
    public interface IServiceSDB
    {

        [OperationContract]
        DataSet GetClientInfo(string address);
        [OperationContract]
        DataSet GetBook(FilterSDB filter, string address);
        [OperationContract]
        [FaultContract(typeof(MySqlException))]
        DataSet GetClientStatistics(FilterSDB filter);
    }
}
