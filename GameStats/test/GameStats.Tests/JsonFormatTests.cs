using GameStats.Server.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameStats.Tests
{
    public class JsonFormatTests
    {
        private static JsonSerializerSettings GetJsonSettings()
        {
            return new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        [Fact]
        public void ServersTest()
        {
            var settings = GetJsonSettings();

            string json = "[{\"endpoint\":\"167.42.23.32-1337\",\"info\":{\"name\":\"] My P3rfect Server [\",\"gameModes\":[\"DM\",\"TDM\"]}},{\"endpoint\":\"62.210.26.88-1337\",\"info\":{\"name\":\">> Sniper Heaven <<\",\"gameModes\":[\"DM\"]}}]";
            Server.Models.Server[] items = JsonConvert.DeserializeObject<Server.Models.Server[]>(json);
            string jsonResult = JsonConvert.SerializeObject(items, settings);
            Assert.Equal(json, jsonResult);
        }

        [Fact]
        public void ServerInfoTest()
        {
            var settings = GetJsonSettings();

            string json = "{\"name\":\"] My P3rfect Server [\",\"gameModes\":[\"DM\",\"TDM\"]}";
            ServerInfo item = JsonConvert.DeserializeObject<ServerInfo>(json);
            string jsonResult = JsonConvert.SerializeObject(item, settings);
            Assert.Equal(json, jsonResult);
        }

        [Fact]
        public void MatchTest()
        {
            var settings = GetJsonSettings();

            string json = "{\"map\":\"DM-HelloWorld\",\"gameMode\":\"DM\",\"fragLimit\":20,\"timeLimit\":20,\"timeElapsed\":12.345678,\"scoreboard\":[{\"name\":\"Player1\",\"frags\":20,\"kills\":21,\"deaths\":3},{\"name\":\"Player2\",\"frags\":2,\"kills\":2,\"deaths\":21}]}";
            Match item = JsonConvert.DeserializeObject<Match>(json);
            string jsonResult = JsonConvert.SerializeObject(item, settings);
            Assert.Equal(json, jsonResult);
        }

        [Fact]
        public void ServersInfoTest()
        {
            var settings = GetJsonSettings();

            string json = "[{\"endpoint\":\"167.42.23.32-1337\",\"info\":{\"name\":\"] My P3rfect Server [\",\"gameModes\":[\"DM\",\"TDM\"]}},{\"endpoint\":\"62.210.26.88-1337\",\"info\":{\"name\":\">> Sniper Heaven <<\",\"gameModes\":[\"DM\"]}}]";
            Server.Models.Server[] items = JsonConvert.DeserializeObject<Server.Models.Server[]>(json);
            string jsonResult = JsonConvert.SerializeObject(items, settings);
            Assert.Equal(json, jsonResult);
        }
    }
}
