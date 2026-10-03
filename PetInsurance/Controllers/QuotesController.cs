using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetInsurance.DTOs;
using PetInsurance.Services.Interfaces;

namespace PetInsurance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly IQuoteService _quoteService;
 
        public QuotesController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateQuote(CreateQuoteDto dto)
        {
            int quoteId = await _quoteService.CreateQuoteAsync(dto);
            
            return CreatedAtAction(
                nameof(GetQuote),
                new { id = quoteId },
                new { QuoteId = quoteId });
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetQuote(int id)
        {
            var quote = await _quoteService.GetQuoteByIdAsync(id);
            
            if (quote is null)
            {
                return NotFound();
            }
            
            return Ok(quote);
        }
        
        [HttpPost("{id:int}/convert")]
        public async Task<IActionResult> ConvertQuote(int id)
        {
            var success = await _quoteService.ConvertQuoteAsync(id);
            
            if (!success)
            {
                return NotFound();
            }
            
            return Ok("Quote converted successfully.");
        }
        
        [HttpPost("{id:int}/recalculate")]
        public async Task<IActionResult> RecalculateQuote(int id,[FromQuery] bool applyDiscount)
        {
            var success = await _quoteService.RecalculateQuoteAsync(id,applyDiscount);
            
            if (!success)
            {
                return NotFound();
            }
            
            return Ok("Quote recalculated successfully.");
        }
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> CancelQuote(int id)
        {
            var success = await _quoteService.CancelQuoteAsync(id);
            
            if (!success)
            {
                return NotFound();
            }
            
            return Ok("Quote cancelled successfully.");
        }
    }
}
