using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class Version
    {
        public Version(string ver)
        {
            ParseVersion(ver);
        }

        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Build { get; private set; }
        public int Revision { get; private set; } = 0;

        public void ParseVersion(string ver)
        {
            var array = ver.Split('.');
            Major = int.Parse(array[0]);
            Minor = int.Parse(array[1]);
            Build = int.Parse(array[2]);
            if (array.Length > 3)
            {
                Revision = int.Parse(array[3]);
            }
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}.{Revision}";
        }
    }

    public class AppVersion
    {
        public string Frontend { get; set; }
        public string Backend { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AppController: ControllerBase
    {
        [HttpGet("Version")]
        public async Task<ActionResult<AppVersion>> Version()
        {
            //Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            var ver = new AppVersion()
            {
                Frontend = (new Version("0.1.1.1")).ToString(),
                Backend = (new Version("0.1.1.1")).ToString()
            };

            return await Task.FromResult<AppVersion>(ver);
        }
    }
}