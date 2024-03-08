// https://data.economie.gouv.fr/api/explore/v2.1/catalog/datasets/prix-des-carburants-en-france-flux-instantane-v2/records?limit=20
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
        var apiData = await ProcessRepositories();
        var database = ConnectToMongoDB();
        InsertIntoMongoDB(database, "data", apiData);
    }

    private static async Task<string> ProcessRepositories()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var stringTask = client.GetStringAsync("https://data.economie.gouv.fr/api/explore/v2.1/catalog/datasets/prix-des-carburants-en-france-flux-instantane-v2/records?limit=20");

        var msg = await stringTask;
        return msg;
    }

    private static IMongoDatabase ConnectToMongoDB()
    {
        var connectionString = "mongodb://localhost:27017";
        mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase("mydb");

        Console.WriteLine("Connected to MongoDB!");
        return database;
    }

    private static void InsertIntoMongoDB(IMongoDatabase database, string collectionName, string data)
    {
        var collection = database.GetCollection<BsonDocument>(collectionName);
        var document = BsonDocument.Parse(data);
        collection.InsertOne(document);
        Console.WriteLine("Data inserted into MongoDB!");
    }
}
