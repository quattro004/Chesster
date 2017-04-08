using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ChessterUciCore;
using ChessterUciCore.Commands;
using Microsoft.Extensions.Logging;
using System;

namespace ChessterWebApi.Controllers
{
    /// <summary>
    /// Controller for the Universal Chess Interface (Uci).
    /// </summary>
    [Route("api/[controller]")]
    public class ChessterController : Controller
    {
        private readonly UniversalChessInterface _uci;
        private readonly ILogger _logger;

        public ChessterController(IEngineController engineController, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("ChessterController") 
                ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger.LogTrace("Constructing ChessterController");
            if(null == engineController) 
            {
                throw new ArgumentNullException(nameof(engineController));
            }
            _uci = new UniversalChessInterface(engineController);
        }

        [HttpGet]
        public IDictionary<string, OptionData> Get()
        {
            _logger.LogTrace("Getting ChessEngineOptions");
            return _uci.ChessEngineOptions;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
