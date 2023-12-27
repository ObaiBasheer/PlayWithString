
using System.Text;
using System.Text.Json;

string txt = "l5:helloi52ee";
string txt3 = "d3:foo3:bar5:helloi52ee";
string txt1 = "i4526e";
string txt2 = "4:obai";
string txt4 = "sample.torrent";
 DecodeEncode(txt4);

(string, string) DecodeEncode(string encodedValue) //l5:helloi52ee
{
    
    //encodedValue = param;
    if (string.IsNullOrEmpty(encodedValue) || encodedValue.StartsWith("e"))
    {
        Console.WriteLine("finished");
    }

    if (Char.IsDigit(encodedValue[0]))
    {
        var (value, remainder) =  DecodeString(encodedValue);
        Console.WriteLine("Decode String: " + value);
        return ((value, remainder));
    }
    else if(encodedValue == "sample.torrent")
    {
        ParseFile(encodedValue);
    }
    else if (encodedValue[0] == 'i')
    {
        var (value, remainder) = DecodeNumber(encodedValue);
        Console.WriteLine("Decode number: " + value);
        return ((value, remainder));

    }
    else if (encodedValue[0] == 'l')
    {
        var (value, remainder) = DecodeList(encodedValue);
        foreach (var item in value)
        {
          Console.WriteLine("Decode list: " + item);
            
        }
        return ((value.ToString()!, remainder));


    }
    else if (encodedValue[0] == 'd')
    {
        var value = DecodeDictionary(encodedValue);
        foreach (var item in value)
        {
            Console.WriteLine("Dict list: " + item.Key + "=> " + item.Value);

        }
        return ((value.ToList().ToString()!, null!));
    }

    return(null!, null!);
}

(string value , string rest) DecodeString(string encodedValue)
{
    var colonIndex = encodedValue.IndexOf(':');
    if (colonIndex != -1)
    {
        var strLength = int.Parse(encodedValue[..colonIndex]);
       var strValue = encodedValue.Substring(colonIndex + 1, strLength);
        string restString;
        Console.WriteLine(JsonSerializer.Serialize(strValue));

        if (strValue.Length + 2 < encodedValue.Length)
        {
            restString = encodedValue.Substring(strValue.Length + colonIndex + 1);
            return (strValue, restString);
        }

        return(strValue, null!);

    }



    else
    {
        throw new InvalidOperationException("Invalid encoded value: " + encodedValue);
    }
}

(string value, string rest) DecodeNumber(string encodedValue)
{
    var endIndex = encodedValue.IndexOf('e');

    if (endIndex != -1)
    {
       var strValue = encodedValue.Substring(1, endIndex - 1);
        if (endIndex + 1 < encodedValue.Length)
        {
            var restString = encodedValue.Substring(endIndex + 1);

            restString = restString.TrimStart('e');
            return (strValue, restString);
        }

        return (strValue, null!);
    }
    else
    {
        throw new InvalidOperationException("Invalid encoded value: " + encodedValue);
    }
}

(List<string> values, string rest) DecodeList(string encodedValue)
{
    var items = new List<string>();
   



       var restOfString = encodedValue[1..^1];
        
        while (!string.IsNullOrEmpty(restOfString) && !restOfString.StartsWith('e'))
        {
           var (value , rest) = DecodeEncode(restOfString);
           items.Add(value);
           restOfString = rest;
        }

        return (items, restOfString);

    

}

Dictionary<string, string> DecodeDictionary(string encodedValue)
{
    //try to split the string with :d3:foo3:bar5:helloi52ee

    var items = new Dictionary<string, string>();




    var restOfString = encodedValue[1..^1];

    while (!string.IsNullOrEmpty(restOfString) && !restOfString.StartsWith('e'))
    {
        var (key, rest) = DecodeEncode(restOfString);
        var (value, reminder) = DecodeEncode(rest);
        items.Add(key, value);
        restOfString = reminder;
    }

    return items;
}

Dictionary<string, string> ParseFile(string filename)
{
    var result = new Dictionary<string, string>();
    var te = $"../../../{filename}";
   // filename = "\\PlayWithString\\sample.torrent";
    string data = String.Empty;
    using FileStream file = new FileStream(te, FileMode.Open, FileAccess.Read);
    using (StreamReader reader = new StreamReader(file))
    {
        data = reader.ReadToEnd();
    }
    // Decode the torrent data
    var (key, rest) = DecodeEncode(data);

    // Construct Torrent record
    var torrent = new Torrent(result["announce"], new Info(result["name"], long.Parse(result["length"]), long.Parse(result["piece length"]), result["pieces"]));

    return result;
}

record Torrent(string announce, Info info) { }
record Info(string name, long length, long Piece, string Piceces);