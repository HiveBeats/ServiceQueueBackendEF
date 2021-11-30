using System.Threading.Tasks;
using WebApi.Features.Triggers.Models;
using Xunit;

namespace WebApi.Tests.Features.Triggers.Models
{
    public class TriggerContextTests
    {
        private const string OriginName = "main_window";
        private const decimal OriginWidth = 500;
        private readonly string _source = $"{{\r\n        \"title\": \"Sample Konfabulator Widget\",\r\n        \"name\": \"{OriginName}\",\r\n        \"width\": {OriginWidth},\r\n        \"height\": 500\r\n}}";
        
        [Fact]
        public void CtorShouldParseSuccessfully()
        {
            //act && assert
            Assert.NotNull(new TriggerContext(_source));
        }

        [Fact]
        public void GetPropertyStringEqualsToOrigin()
        {
            var context = new TriggerContext(_source);

            var result = context.GetPropertyStringValue("name");
            
            Assert.Equal(OriginName, result);
        }
        
        [Fact]
        public void GetPropertyNumberEqualsToOrigin()
        {
            var context = new TriggerContext(_source);

            var result = context.GetPropertyNumberValue("width");
            
            Assert.Equal(OriginWidth, result);
        }
    }
}