﻿using KidzBizzServer.BL;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KidzBizzServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        // GET: api/<CardsController>
        [HttpGet]
        public IEnumerable<Card> Get()
        {
            Card card = new Card();
            return card.ReadAllCards();
        }

        // GET api/<CardsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CardsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CardsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CardsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
