using System;
using WebApi.Models;
using Xunit;

namespace WebApi.Tests.Features.Scripting
{
    public class ScriptTests
    {
        [Fact]
        public void CreateFilledAllFields()
        {
            //arrange
            var topic = new Topic();
            var name = "script_name";
            var body = "public bool IsTrue { get; set; }";
            var priority = 3;
            var today = DateTime.UtcNow.Date;
            //act
            var result = Script.Create(topic, name, body, priority);
            //assert
            Assert.NotNull(result);
            Assert.Equal(topic, result.Topic);
            Assert.Equal(name, result.Name);
            Assert.Equal(body, result.Body);
            Assert.Equal(priority, result.Priority);
            Assert.Equal(true, result.IsEnabled);
            Assert.Equal(today, result.DateCreated.Date);
            Assert.Equal(today, result.DateModified.Date);
        }

        [Fact]
        public void ThrowCreatedWithoutTopic()
        {
            Assert.Throws<InvalidOperationException>(() => Script.Create(null, "argh", "hargh", 0));
        }

        [Fact]
        public void ThrowCreatedWithNullOrEmptyName()
        {
            Assert.Throws<InvalidOperationException>(() => Script.Create(new Topic(), null, "hargh", 0));
            Assert.Throws<InvalidOperationException>(() => Script.Create(new Topic(), "", "hargh", 0));
        }

        [Fact]
        public void ThrowCreatedWithoutBody()
        {
            Assert.Throws<InvalidOperationException>(() => Script.Create(new Topic(), "argh", null, 0));
        }
        
        [Fact]
        public void ToggleEnabledBothWays()
        {
            var script = Script.Create(new Topic(), "name", "body", 0);
            
            script.ToggleEnabled();
            Assert.Equal(false, script.IsEnabled);
            
            script.ToggleEnabled();
            Assert.Equal(true, script.IsEnabled);
        }
        
        [Fact]
        public void UpdateBodyUpdates()
        {
            var body = "body";
            var script = Script.Create(new Topic(), "name", body, 0);

            var newBody = "new body";
            script.UpdateBody(newBody);
            
            Assert.NotEqual(body, script.Body);
            Assert.Equal(newBody, script.Body);
        }

        [Fact]
        public void UpdatePriorityUpdates()
        {
            var priority = 0;
            var script = Script.Create(new Topic(), "name", "body", priority);

            var newPriority = 2;
            script.UpdatePriority(newPriority);
            
            Assert.NotEqual(priority, script.Priority);
            Assert.Equal(newPriority, script.Priority);
        }
    }
}