using Microsoft.AspNetCore.Mvc;

namespace backend
{
    [ApiController]
    [Route("api")]
    public class RestController : ControllerBase
    {
        [HttpPost("generateCode")]
        public string GenerateCode(GenerateCodeRequest request) 
        {
            var code = Guid.NewGuid().ToString("N").Substring(0,8);
            return code;
        }
    }
}

public class GenerateCodeRequest {
    public List<Entity> Entities {get;set;}
}

public class Entity {
    public string Id {get;set;}
    public int Level {get;set;}
    public Dictionary<string, int> Levels {get;set;}
}