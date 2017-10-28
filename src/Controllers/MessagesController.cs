﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApi.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatApi.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        private string connString = Environment.GetEnvironmentVariable("MONGODB_CONN_STR");

        // GET api/messages
        [HttpGet]
        public IActionResult Get()
        {
            var collection = GetMessageCollection();
            var messages = collection.Find(m => true).ToList();
            return Json(messages);
        }

        // GET api/messages/59f3d3c225eca06c14be4694
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var collection = GetMessageCollection();
            var message = collection.Find(m => m.Id == id).FirstOrDefault();

            if (message == null)
                return NotFound();

            return Json(message);
        }

        [HttpGet]
        [Route("~/api/users/{userId}/sentmessages")]
        public IActionResult GetAllFromUser(string userId, int limit)
        {
            var collection = GetMessageCollection();
            var messages = collection.Find(m => m.SenderId == userId).Limit(limit).ToList();
            return Json(messages);
        }

        // POST api/messages
        [HttpPost]
        public IActionResult Post([FromBody]Message message)
        {
            message.Id = ObjectId.GenerateNewId().ToString();
            message.Timestamp = DateTime.Now;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var collection = GetMessageCollection();
            collection.InsertOne(message);
            return Get(message.Id);
        }

        // DELETE api/messages/59f3d3c225eca06c14be4694
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var collection = GetMessageCollection();
            collection.DeleteOne(m => m.Id == id);
            return Ok();
        }

        private IMongoCollection<Message> GetMessageCollection()
        {
            var client = new MongoClient(connString);
            var db = client.GetDatabase("chat-demo");
            var collection = db.GetCollection<Message>("messages");
            return collection;
        }
    }
}