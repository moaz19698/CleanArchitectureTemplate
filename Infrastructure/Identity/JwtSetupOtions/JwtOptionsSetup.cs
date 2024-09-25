using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Infrastructure.Identity.JwtSetupOtions
{
    public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
    {
        private const string SECTION_NAME = "Jwt";
        private IConfiguration Configuration { get; }

        public JwtOptionsSetup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Configure(JwtOptions options)
        {
            Configuration.GetSection(SECTION_NAME).Bind(options);
        }
    }
}
