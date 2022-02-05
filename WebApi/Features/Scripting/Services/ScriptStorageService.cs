using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Scripting.Requests;
using WebApi.Features.Scripting.Responses;
using WebApi.Models;

namespace WebApi.Features.Scripting.Services
{
    public interface IScriptService
    {
        Task<ScriptDto> CreateScriptForTopic(CreateScriptOnTopicRequest request);
        Task<ScriptDto> EnableOrDisableScript(EnableOrDisableScriptRequest request);
        Task<ScriptDto> UpdateScriptBody(UpdateScriptBodyRequest request);
        Task<ScriptDto> UpdateScriptPriority(UpdateScriptPriorityRequest request);
        Task<IEnumerable<ScriptDto>> GetTopicScripts(long topic);
        Task<IEnumerable<ScriptLogDto>> GetScriptLogs(long script);
    }

    public interface IScriptStorageService
    {
        Task<IEnumerable<Script>> GetTopicScriptsByPriority(InvokeScriptsOnTopicRequest request);
        Task AddScriptLogs(Topic topic, IEnumerable<ScriptLog> logs);
    }
    public class ScriptStorageService: IScriptService, IScriptStorageService
    {
        private readonly AppDbContext _db;
        public ScriptStorageService(AppDbContext db)
        {
            _db = db;
        }
        
        public async Task<ScriptDto> CreateScriptForTopic(CreateScriptOnTopicRequest request)
        {
            var topic = await _db.Topics.FindAsync(request.TopicId);
            if (topic == null)
                throw new InvalidOperationException("Такого топика не существует");

            var script = _db.Scripts.Add(Script.Create(topic, request.Name, request.Body, request.Priority));
            
            await _db.SaveChangesAsync();

            return new ScriptDto(script.Entity);
        }

        public async Task<ScriptDto> EnableOrDisableScript(EnableOrDisableScriptRequest request)
        {
            var script = await _db.Scripts.FindAsync(request.ScriptId);
            if (script == null)
                throw new InvalidOperationException("Такого скрипта не существует");
            
            script.ToggleEnabled();
            await _db.SaveChangesAsync();

            return new ScriptDto(script);
        }

        public async Task<ScriptDto> UpdateScriptBody(UpdateScriptBodyRequest request)
        {
            var script = await _db.Scripts.FindAsync(request.Id);
            if (script == null)
                throw new InvalidOperationException("Такого скрипта не существует");
            
            script.UpdateBody(request.Body);
            await _db.SaveChangesAsync();

            return new ScriptDto(script);
        }

        public async Task<ScriptDto> UpdateScriptPriority(UpdateScriptPriorityRequest request)
        {
            var script = await _db.Scripts.FindAsync(request.Id);
            if (script == null)
                throw new InvalidOperationException("Такого скрипта не существует");
            
            script.UpdatePriority(request.Priority);
            await _db.SaveChangesAsync();

            return new ScriptDto(script);
        }

        public async Task<IEnumerable<ScriptDto>> GetTopicScripts(long topic)
        {
            var scripts = await GetTopicScriptsByPriority(new InvokeScriptsOnTopicRequest() { TopicId = topic });
            return scripts.Select(x => new ScriptDto(x));
        }

        public async Task<IEnumerable<ScriptLogDto>> GetScriptLogs(long script)
        {
            var logs = await _db.ScriptLogs.Where(x => x.ScriptId == script).AsNoTracking().ToListAsync();
            return logs.Select(x => new ScriptLogDto(x));
        }

        public async Task<IEnumerable<Script>> GetTopicScriptsByPriority(InvokeScriptsOnTopicRequest request)
        {
            return await _db.Scripts.Where(s => s.TopicId == request.TopicId).OrderBy(s => s.Priority).ToListAsync();
        }

        public async Task AddScriptLogs(Topic topic, IEnumerable<ScriptLog> logs)
        {
            foreach (var log in logs)
            {
                _db.ScriptLogs.Add(log);    
            }

            await _db.SaveChangesAsync();
        }
    }
}