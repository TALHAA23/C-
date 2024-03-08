using System;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static MongoClient mongoClient = new MongoClient();

    static async Task Main(string[] args)
    {
        // await ProcessRepositories();
        ConnectToMongoDB();
    }

    private static async Task ProcessRepositories()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var stringTask = client.GetStringAsync("http://api.github.com/orgs/dotnet/repos");

        var msg = await stringTask;
        Console.Write(msg);
    }

    private static void ConnectToMongoDB()
    {
        var connectionString = "mongodb://localhost:27017";
        mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase("appdb");
        var collection = database.GetCollection<BsonDocument>("users");
        

        var firstDocument = collection.Find(new BsonDocument()).FirstOrDefault();
        if (firstDocument != null)
        {
            Console.WriteLine(firstDocument.ToString());
        }
        else
        {
            Console.WriteLine("No documents found.");
        }

        Console.WriteLine("Connected to MongoDB!");
    }
}
