using System.Collections.Generic;

public class Request
{
    public string AccountCode { get; set; }
    public string AuthKey { get; set; }
    public Dictionary<string, string> Data { get; set; }

    public Request Make(string accountCode, string authKey, Dictionary<string,string> data)
    {
        Request Request = new Request();
        AccountCode = accountCode;
        AuthKey = authKey;
        Data = data;
        return Request;
    }
}

public class Response
{
    public int StateCode { get; set; }
    // TODO : 데이터 업데이트는 생각을 좀 해봐야 된다.
    public Dictionary<string,string> Update {  get; set; }
    public Dictionary<string,string> Data { get; set; }

    public Response Make(eStateCode stateCode, Dictionary<string,string> update, Dictionary<string,string> data)
    {
        Response Response = new Response();
        StateCode = stateCode.GetHashCode();
        Update = update;
        Data = data;
        return Response;
    }
}