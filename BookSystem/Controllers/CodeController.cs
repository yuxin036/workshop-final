using BookSystem.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace BookSystem.Controllers
{

    [Route("api/code")]
    [ApiController]
    public class CodeController : ControllerBase
    {

        [Route("bookstatus")]
        [HttpGet()]
        public IActionResult GetBookStatusData()
        {
            try
            {
                CodeService codeService = new CodeService();
                ApiResult<List<Code>> result = new ApiResult<List<Code>>()
                {
                    Data = codeService.GetBookStatusData(),
                    Status = true,
                    Message = string.Empty
                };

                return Ok(result);
            }
            catch (Exception)
            {

                return Problem();
            }
        }
        //TODO:bookclass下拉選單、借閱人下拉選單
        [Route("bookclass")]
        [HttpGet()]
        public IActionResult GetBookClassData()
        {
            try
            {
                CodeService codeService = new CodeService();
                var result = new ApiResult<List<Code>>()
                {
                    Data = codeService.GetBookClassData(),
                    Status = true,
                    Message = string.Empty
                };

                return Ok(result);
            }
            catch (Exception)
            {

                return Problem();
            }
        }

        [Route("member")]
        [HttpGet()]
        public IActionResult GetMemberData()
        {
            try
            {
                CodeService codeService = new CodeService();
                var result = new ApiResult<List<Member>>()
                {
                    Data = codeService.GetMemberData(),
                    Status = true,
                    Message = string.Empty
                };

                return Ok(result);
            }
            catch (Exception)
            {

                return Problem();
            }
        }

    }
}
