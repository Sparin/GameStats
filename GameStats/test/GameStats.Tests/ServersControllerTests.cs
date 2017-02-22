using GameStats.Server.Controllers;
using GameStats.Server.Models;
using GameStats.Utility.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GameStats.Tests
{
    //TODO: Refactoring
    public class ServersControllerTests
    {
        public const string DbPath = @".\GameStats.Storage.db";
        public static ServersController controller;

        static ServersControllerTests()
        {
            DatabaseContext.ConnectionString = "Filename=" + DbPath;
            controller = new ServersController();
        }

        #region /servers/<endpoint>/info PUT, GET
        [Fact]
        public void PUT_ServersInfo_ValidInfo_OK()
        {
            DatabaseContext.ConnectionString = "Filename=" + DbPath;
            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);

            File.Delete(DbPath);
        }

        [Theory]
        [InlineData("{\r\n\t\"name\": \"] My P3rfect ,\r\n\t\"gameModes\": [ \"DM\", \"TDM\" ]\r\n}")]  //Format
        [InlineData("{\r\n\t\"name\": \"] My P3rfect Server [\"\r\n}")]                                 //Missing value
        public void PUT_ServerInfo_InvalidInfo_BadRequest(string invalidJson)
        {
            ServerInfo invalidInfo = null;
            Assert.ThrowsAny<JsonException>(() => { invalidInfo = JsonConvert.DeserializeObject<ServerInfo>(invalidJson); });

            var response = controller.PutServerInfo("testEndpoint", invalidInfo);

            File.Delete(DbPath);
            Assert.True(response is BadRequestResult);
        }

        [Fact]
        public void GET_ServerInfo_ExistingInfo_OK()
        {
            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);
            var response = controller.GetServerInfo(server.Endpoint);

            ServerInfo info = (ServerInfo)((ObjectResult)response).Value;
            Assert.Equal(server.Info.Endpoint, info.Endpoint);

            File.Delete(DbPath);
        }

        [Fact]
        public void GET_ServerInfo_NotExistingInfo_NotFound()
        {
            var response = controller.GetServerInfo("Not existing endpoint");
            Assert.True(response is NotFoundResult);

            File.Delete(DbPath);
        }
        #endregion
        #region /servers/<endpoint>/matches/<timestamp> PUT, GET
        [Fact]
        public void PUT_Match_ValidInfo_OK()
        {
            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);
            Match match = RandomModels.CreateRandomMatch(server);
            Assert.True(controller.PutMatch(match.Endpoint, match.Timestamp, match) is OkResult);

            File.Delete(DbPath);
        }

        [Fact]
        public void PUT_Match_ValidInfo_Conflict()
        {
            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);
            Match match = RandomModels.CreateRandomMatch(server);
            Assert.True(controller.PutMatch(match.Endpoint, match.Timestamp, match) is OkResult);
            Assert.True(((StatusCodeResult)controller.PutMatch(match.Endpoint, match.Timestamp, match)).StatusCode == 409);

            File.Delete(DbPath);
        }

        [Fact]
        public void PUT_Match_ValidInfo_Forbid()
        {
            Match match = RandomModels.CreateRandomMatch(RandomModels.CreateRandomServer());
            Assert.True(controller.PutMatch(match.Endpoint, match.Timestamp, match) is ForbidResult);

            File.Delete(DbPath);
        }

        [Theory]
        //Missing Player2 stats
        [InlineData("{\r\n\t\"map\": \"DM-HelloWorld\",\r\n\t\"gameMode\": \"DM\",\r\n\t\"fragLimit\": 20,\r\n\t\"timeLimit\": 20,\r\n\t\"timeElapsed\": 12.345678,\r\n\t\"scoreboard\": [\r\n\t\t{\r\n\t\t\t\"name\": \"Player1\",\r\n\t\t\t\"frags\": 20,\r\n\t\t\t\"kills\": 21,\r\n\t\t\t\"deaths\": 3\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\": \"Player2\",\r\n\t\t\t\"frags\": 2\r\n\t\t}\r\n]\r\n}\r\n")]
        //Missing match info
        [InlineData("{\r\n\t\"map\": \"DM-HelloWorld\",\r\n\t\"gameMode\": \"DM\"\r\n\t\"timeElapsed\": 12.345678,\r\n\t\"scoreboard\": [\r\n\t\t{\r\n\t\t\t\"name\": \"Player1\",\r\n\t\t\t\"frags\": 20,\r\n\t\t\t\"kills\": 21,\r\n\t\t\t\"deaths\": 3\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\": \"Player2\",\r\n\t\t\t\"frags\": 2,\r\n\t\t\t\"kills\": 2,\r\n\t\t\t\"deaths\": 21\r\n\t\t}\r\n]\r\n}\r\n")]
        //Wrong format
        [InlineData("{\r\n\t\"map\": \"DM-HelloWorld\",\r\n\t\"gameMode\": \"DM\",\r\n\t\"fragLimit\": 20,\r\n\t\"timeLimit\": 20,\r\n\t\"timeElapsed\": 12.345678,\r\n\t\"scoreboard\": \r\n\t\t\t\"name\": \"Player1\",\r\n\t\t\t\"frags\": 20,\r\n\t\t\t\"kills\": 21,\r\n\t\t\t\"deaths\": 3\r\n\t\t},\r\n\t\t{\r\n\t\t\t\"name\": \"Player2\",\r\n\t\t\t\"frags\": 2,\r\n\t\t\t\"kills\": 2,\r\n\t\t\t\"deaths\": 21\r\n\t\t}\r\n]\r\n}\r\n")]
        public void PUT_Match_InvalidInfo_BadRequest(string invalidJson)
        {
            Match match = null;
            Assert.ThrowsAny<JsonException>(() => { match = JsonConvert.DeserializeObject<Match>(invalidJson); });
            Assert.True(controller.PutMatch(null, DateTime.Now, match) is BadRequestResult);

            File.Delete(DbPath);
        }

        [Fact]
        public void GET_Match_ValidInfo_OK()
        {
            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);

            Match match = RandomModels.CreateRandomMatch(server);
            Assert.True(controller.PutMatch(match.Endpoint, match.Timestamp, match) is OkResult);

            Match response = (Match)((ObjectResult)controller.GetMatch(match.Endpoint, match.Timestamp)).Value;
            Assert.True(response.Endpoint == match.Endpoint && response.Timestamp == match.Timestamp);

            File.Delete(DbPath);
        }

        [Fact]
        public void GET_Match_ValidInfo_NotFound()
        {
            Assert.True(controller.GetMatch("Not existing endpoint", DateTime.Now) is NotFoundResult);

            File.Delete(DbPath);
        }
        #endregion
        //TODO: Add test with values
        #region /servers/info GET
        [Fact]
        public void GET_Servers_OK()
        {
            Assert.True(((ObjectResult)controller.GetServers()).Value is IEnumerable<Server.Models.Server>);

            File.Delete(DbPath);
        }
        #endregion
        #region /servers/<endpoint>/stats GET
        [Fact]
        public void GET_ServersStats_NotExistingServer_NotFound()
        {
            Assert.True(controller.GetServerStats("Not existing endpoint") is NotFoundResult);

            File.Delete(DbPath);
        }

        [Fact]
        public void GET_ServersStats_ExistingServer_OK()
        {
            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);

            for (int i = 0; i < 5; i++)
            {
                Match match = RandomModels.CreateRandomMatch(server);
                Assert.True(controller.PutMatch(server.Endpoint, match.Timestamp, match) is OkResult);
            }
            Assert.True(((ObjectResult)controller.GetServerStats(server.Endpoint)).Value is ServerStats);

            File.Delete(DbPath);
        }
        #endregion
    }
}
