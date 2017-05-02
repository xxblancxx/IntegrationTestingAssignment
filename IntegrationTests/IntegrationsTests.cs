using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoApp;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MongoDB.Bson;

namespace IntegrationTests
{
    [TestClass]
    public class IntegrationsTests
    {
        [TestMethod]
        public void VerifySortingTest()
        {
            // Verifies that the list of tweets the interface retrieves from the database is sorted correctly
            // Decending
            // If applicable, verify correct amount of documents retrieved (top 10 should contain 10 etc.)
            MongoDBExerciseHandler ex = new MongoDBExerciseHandler();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Tweet));
            var tweets = GetStringListFromBsonList(ex.GetTopTenTaggers());
            Assert.AreEqual(10, tweets.Count);
            VerifyDecendingOrder(tweets);

            tweets = GetStringListFromBsonList(ex.GetMostActiveUsers());
            Assert.AreEqual(10, tweets.Count);
            VerifyDecendingOrder(tweets);

            tweets = GetStringListFromBsonList(ex.GetMostGrumpyUsers());
            Assert.AreEqual(5, tweets.Count);
            VerifyDecendingOrder(tweets);

            tweets = GetStringListFromBsonList(ex.GetMostHappyUsers());
            Assert.AreEqual(5, tweets.Count);
            VerifyDecendingOrder(tweets);
        }

        public void VerifyDecendingOrder(List<string> jsonList)
        {
            long previousCount = 0;
            foreach (var tweet in jsonList)
            {
                var split = tweet.Replace("_id", "Username:").Replace("\\", "").Replace("\"", "").Replace("{", "").Replace("}", "").Replace(":", "").Split(' ');

                long count = long.Parse(split[split.Length - 2]);
                if (previousCount == 0)
                {
                    previousCount = count;
                }
                else if (previousCount < count)
                {
                    Assert.Fail("Incorrect sorting based on count");
                }
            }
        }

        public List<string> GetStringListFromBsonList(List<BsonDocument> bsonList)
        {
            var stringTweets = new List<string>();
            foreach (var tweet in bsonList)
            {
                stringTweets.Add(tweet.AsBsonDocument.ToString());
            }
            return stringTweets;
        }
    }
}
