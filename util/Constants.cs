using System.Text.RegularExpressions;

namespace Dance_Dreamers_Administartor.util
{
    internal class Constants
    {
        public static readonly string DB_CONNECTION_STRING = @"Server=ec2-54-228-201-167.eu-west-1.compute.amazonaws.com;
                                                    Port=5432;
                                                    Database=d4gvgagulbr0q4;User Id=jeapnlykfhgrhs;
                                                    Password=19acb86cf78b6939c24b4cdb298035e0ab31ca4a0d3225a030056217d441dcbf";
        public static readonly string DB_CONNECTION_STRING_MOCK = @"Server=ec2-34-251-115-141.eu-west-1.compute.amazonaws.com;
                                                    Port=5432;
                                                    Database=deeg605qn66ojc;User Id=anansgdoqcysxm;
                                                    Password=f40cda46907947f43d75cc74b4fa8b2058534b0dff132a485f502a86499b69b7";
        public static readonly Regex INPUT_REGEX = new Regex(@"^\w{1}(\w+|\s?|-?|:?)+$");
    }
}
