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
        #region /servers/<endpoint>/info PUT, GET
        [Fact]
        public void PUT_ServersInfo_ValidInfo_OK()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";

            DatabaseContext.ConnectionString = "Filename=" + dbPath;
            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);

            File.Delete(dbPath);
        }

        [Theory]
        [InlineData("{\r\n\t\"name\": \"] My P3rfect ,\r\n\t\"gameModes\": [ \"DM\", \"TDM\" ]\r\n}")]  //Format
        [InlineData("{\r\n\t\"name\": \"] My P3rfect Server [\"\r\n}")]                                 //Missing value
        public void PUT_ServerInfo_InvalidInfo_BadRequest(string invalidJson)
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            ServerInfo invalidInfo = null;
            Assert.ThrowsAny<JsonException>(() => { invalidInfo = JsonConvert.DeserializeObject<ServerInfo>(invalidJson); });

            var response = controller.PutServerInfo("testEndpoint", invalidInfo);

            File.Delete(dbPath);
            Assert.True(response is BadRequestResult);
        }

        [Fact]
        public void GET_ServerInfo_ExistingInfo_OK()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);
            var response = controller.GetServerInfo(server.Endpoint);

            ServerInfo info = (ServerInfo)((ObjectResult)response).Value;
            Assert.Equal(server.Info.Endpoint, info.Endpoint);

            File.Delete(dbPath);
        }

        [Fact]
        public void GET_ServerInfo_NotExistingInfo_NotFound()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            var response = controller.GetServerInfo("Not existing endpoint");
            Assert.True(response is NotFoundResult);

            File.Delete(dbPath);
        }
        #endregion
        #region /servers/<endpoint>/matches/<timestamp> PUT, GET
        [Fact]
        public void PUT_Match_ValidInfo_OK()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);
            Match match = RandomModels.CreateRandomMatch(server);
            Assert.True(controller.PutMatch(match.Endpoint, match.Timestamp, match) is OkResult);

            File.Delete(dbPath);
        }

        [Fact]
        public void PUT_Match_ValidInfo_Conflict()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);
            Match match = RandomModels.CreateRandomMatch(server);
            Assert.True(controller.PutMatch(match.Endpoint, match.Timestamp, match) is OkResult);
            Assert.True(((StatusCodeResult)controller.PutMatch(match.Endpoint, match.Timestamp, match)).StatusCode == 409);

            File.Delete(dbPath);
        }

        [Fact]
        public void PUT_Match_ValidInfo_Forbid()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            Match match = RandomModels.CreateRandomMatch(RandomModels.CreateRandomServer());
            Assert.True(controller.PutMatch(match.Endpoint, match.Timestamp, match) is ForbidResult);

            File.Delete(dbPath);
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
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            Match match = null;
            Assert.ThrowsAny<JsonException>(() => { match = JsonConvert.DeserializeObject<Match>(invalidJson); });
            Assert.True(controller.PutMatch(null, DateTime.Now, match) is BadRequestResult);

            File.Delete(dbPath);
        }

        [Fact]
        public void GET_Match_ValidInfo_OK()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info) is OkResult);

            Match match = RandomModels.CreateRandomMatch(server);
            Assert.True(controller.PutMatch(match.Endpoint, match.Timestamp, match) is OkResult);

            Match response = (Match)((ObjectResult)controller.GetMatch(match.Endpoint, match.Timestamp)).Value;
            Assert.True(response.Endpoint == match.Endpoint && response.Timestamp == match.Timestamp);

            File.Delete(dbPath);
        }

        [Fact]
        public void GET_Match_ValidInfo_NotFound()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            Assert.True(controller.GetMatch("Not existing endpoint", DateTime.Now) is NotFoundResult);

            File.Delete(dbPath);
        }
        #endregion
        //TODO: Add test with values
        #region /servers/info GET
        [Fact]
        public void GET_Servers_OK()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            Assert.True(((ObjectResult)controller.GetServers()).Value is IEnumerable<Server.Models.Server>);

            File.Delete(dbPath);
        }
        #endregion
        #region /servers/<endpoint>/stats GET
        [Fact]
        public void GET_ServersStats_NotExistingServer_NotFound()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            Assert.True(controller.GetServerStats("Not existing endpoint") is NotFoundResult);

            File.Delete(dbPath);
        }

        [Fact]
        public void GET_ServersStats_ExistingServer_OK()
        {
            ServersController controller = new ServersController();
            string dbPath = Directory.GetCurrentDirectory() + @"\GameStats.Storage.db";
            DatabaseContext.ConnectionString = "Filename=" + dbPath;

            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(controller.PutServerInfo(server.Endpoint, server.Info)is OkResult);

            for (int i = 0; i < 5; i++)
            {
                Match match = RandomModels.CreateRandomMatch(server);
                Assert.True(controller.PutMatch(server.Endpoint, match.Timestamp, match) is OkResult);
            }
            Assert.True(((ObjectResult)controller.GetServerStats(server.Endpoint)).Value is ServerStats);

            File.Delete(dbPath);
        }
        #endregion
    }
}
