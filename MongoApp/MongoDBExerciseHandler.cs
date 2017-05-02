using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MongoApp
{
    public class MongoDBExerciseHandler
    {
        private MongoClient client;
        private IMongoDatabase db;
        private IMongoCollection<Tweet> tweets;

        public MongoDBExerciseHandler()
        {
            client = new MongoClient("mongodb://127.0.0.1:27017");
            db = client.GetDatabase("Social_Network");
            tweets = db.GetCollection<Tweet>("tweets");
        }




        public long GetUserCount()
        {
            // Query 1
            // Find users by selecting distinct on User
            var totalUsers = tweets.AsQueryable().Select(x => x.user).Distinct().Count();
            return totalUsers;
        }

        public List<BsonDocument> GetTopTenTaggers()
        {
            // Find top 10 of those who tag most other users.
            var aggregate = tweets.Aggregate(new AggregateOptions { AllowDiskUse = true })
                .Match(new BsonDocument { { "text", BsonRegularExpression.Create(new Regex("@")) } })
                .Group(new BsonDocument { { "_id", "$user" }, { "nrOfTweets", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument { { "nrOfTweets", -1 } })
                .Limit(10);
            return aggregate.ToList();
        }

        public List<BsonDocument> GetMostActiveUsers()
        {
            // Find top 10 most frequent tweeters.
            //Here i'm assuming that most active means most tweets.
            var aggregate = tweets.Aggregate(new AggregateOptions { AllowDiskUse = true })
                .Group(new BsonDocument { { "_id", "$user" }, { "nrOfTweets", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument { { "nrOfTweets", -1 } })
                .Limit(10);
            return aggregate.ToList();
        }

       public List<BsonDocument> GetMostHappyUsers()
        {
            // Seeing as "positive/happy" is not a technical term, i chose a handful of words to examplify "positivity"
            var pattern = BsonRegularExpression.Create(new Regex("happy|love|party|hug|kiss"));
            var aggregate = tweets.Aggregate(new AggregateOptions { AllowDiskUse = true })
                .Match(new BsonDocument { { "text", pattern } })
                .Group(new BsonDocument { { "_id", "$user" }, { "nrOfTweets", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument { { "nrOfTweets", -1 } })
                .Limit(5);
            return aggregate.ToList();
        }

        public List<BsonDocument> GetMostGrumpyUsers()
        {
            // Seeing as "negative/grumpy" is not a technical term, i chose a handful of words to examplify "negativity"
            var pattern = BsonRegularExpression.Create(new Regex("angry|hate|fuck|kill"));
            var aggregate = tweets.Aggregate(new AggregateOptions { AllowDiskUse = true })
                .Match(new BsonDocument { { "text", pattern } })
                .Group(new BsonDocument { { "_id", "$user" }, { "nrOfTweets", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument { { "nrOfTweets", -1 } })
                .Limit(5);
            return aggregate.ToList();
        }
    }
}
