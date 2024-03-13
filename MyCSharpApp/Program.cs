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
        try
        {
            var database = ConnectToMongoDB();
            var apiData = await ProcessRepositories();
            InsertIntoMongoDB(database, "data", apiData);
            ReadAllDocuments(database, "data");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static async Task<string> ProcessRepositories()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetStringAsync("https://data.economie.gouv.fr/api/explore/v2.1/catalog/datasets/prix-des-carburants-en-france-flux-instantane-v2/records?limit=20");
        var data = response;
        return data;
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
        var jsonData = BsonDocument.Parse(data);
        var results = jsonData.GetValue("results").AsBsonArray;

        foreach (var result in results)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("id", result["id"]);
            var update = Builders<BsonDocument>.Update.SetOnInsert("id", result["id"]).SetOnInsert("data", result);
            var options = new UpdateOptions { IsUpsert = true };

            collection.UpdateOne(filter, update, options);
        }

        Console.WriteLine("Data inserted into MongoDB!");
    }

    private static void ReadAllDocuments(IMongoDatabase database, string collectionName)
    {
        var collection = database.GetCollection<BsonDocument>(collectionName);
        var documents = collection.Find(new BsonDocument()).ToList();

        foreach (var document in documents)
        {
            var data = document.GetValue("data").AsBsonDocument;
            Console.WriteLine(data.ToString());
        }
    }

}
