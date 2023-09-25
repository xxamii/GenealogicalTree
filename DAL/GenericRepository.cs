using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DAL.Abstractions.Interfaces;
using Core.Models;
using Newtonsoft.Json.Converters;

namespace DAL
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : Entity
    {
        private readonly string _DBPath;
        private readonly string _tableName;

        public GenericRepository(string DBPath)
        {
            _DBPath = DBPath;
            _tableName = typeof(T).Name;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            JObject jsonData = await GetJObject();
            List<T> allEntities = GetAllEntities(jsonData);

            if (allEntities.Any(a => a.Id == entity.Id))
            {
                allEntities.RemoveAll(a => a.Id == entity.Id);
                string newTableData = JsonConvert.SerializeObject(allEntities);
                jsonData[_tableName] = newTableData;
                await File.WriteAllTextAsync(_DBPath, jsonData.ToString());
                return true;
            }

            return false;
        }

        public virtual async Task<List<T>> DeserializeAsync(Func<T, bool> predicate = null)
        {
            JObject jsonData = await GetJObject();
            List<T> allEntities = GetAllEntities(jsonData);

            if (predicate != null)
            {
                return allEntities.Where(predicate).ToList();
            }

            return allEntities;
        }

        public virtual async Task<T> SerializeAsync(T entity)
        {
            JObject jsonData = await GetJObject();
            List<T> allEntities = GetAllEntities(jsonData);

            entity.Id = GetNewEntityId(allEntities);
            
            allEntities.Add(entity);

            string newTableData = JsonConvert.SerializeObject(allEntities);
            jsonData[_tableName] = newTableData;

            await File.WriteAllTextAsync(_DBPath, jsonData.ToString());

            return entity;
        }

        public async Task<T?> UpdateAsync(T entity)
        {
            JObject jsonData = await GetJObject();
            List<T> allEntities = GetAllEntities(jsonData);

            if (allEntities.Any(a => a.Id == entity.Id))
            {
                allEntities = allEntities.Select(a => a.Id == entity.Id ? entity : a).ToList();

                jsonData[_tableName] = JsonConvert.SerializeObject(allEntities);

                await File.WriteAllTextAsync(_DBPath, jsonData.ToString());

                return entity;
            }

            return null;
        }

        private List<T> GetAllEntities(JObject jsonData)
        {
            IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "dd.MM.yyyy" };

            List<T> allEntities = JsonConvert.DeserializeObject<List<T>>(jsonData[_tableName].ToString(), dateTimeConverter);

            return allEntities;
        }

        private async Task<JObject> GetJObject()
        {
            string jsonData = await File.ReadAllTextAsync(_DBPath);
            return JObject.Parse(jsonData);
        }

        private int GetNewEntityId(List<T> entities)
        {
            T lastElement = entities.OrderBy(a => a.Id).LastOrDefault();
            return lastElement == null ? 0 : lastElement.Id + 1;
        }
    }
}
