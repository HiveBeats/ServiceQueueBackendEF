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
            Assert.Equal(LogLevel.Warning, result.LogLevel);
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
            Assert.Throws<ArgumentNullException>(() => Script.Create(new Topic(), "argh", null, 0));
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
        public void UpdateBodyWithNullThrows()
        {
            var script = Script.Create(new Topic(), "name", "body", 0);
            Assert.Throws<ArgumentNullException>(() => script.UpdateBody(null));
        }

        [Fact]
        public void UpdateSettingsUpdates()
        {
            var priority = 0;
            var script = Script.Create(new Topic(), "name", "body", priority);

            var newPriority = 2;
            var newLogLevel = LogLevel.Debug;
            
            script.UpdateSettings(false, newLogLevel, newPriority);
            
            Assert.Equal(newPriority, script.Priority);
            Assert.Equal(false, script.IsEnabled);
            Assert.Equal(newLogLevel, script.LogLevel);
        }
    }
}