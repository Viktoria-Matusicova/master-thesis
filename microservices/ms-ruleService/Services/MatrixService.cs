using ruleService.Interfaces;
using ruleService.Models;
using Newtonsoft.Json;
using ruleService.Utilities;
using System.Net;

namespace ruleService.Services
{
    public class MatrixService : IMatrixService
    {
        private readonly IParentRepository _parentRepository;
        private readonly IRuleRepository _ruleRepository;
        private readonly ILogger<MatrixService> _logger;
        private readonly YamlDotNet.Serialization.Deserializer _yamlDeserializer;
        private Dictionary<int, string> colors = new Dictionary<int, string> { { 0, "none" }, { 1, "#3f0e0e" }, { 20, "#632626" }, { 40, "#a5694b" }, { 60, "#b69945" }, { 80, "#5e8d64" }, { 100, "#263829" } };

        public MatrixService(ILogger<MatrixService> logger, IParentRepository parentRepository, IRuleRepository ruleRepository, YamlDotNet.Serialization.Deserializer yamlDeserializer)
        {
            _parentRepository = parentRepository;
            _logger = logger;
            _ruleRepository = ruleRepository;
            _yamlDeserializer = yamlDeserializer;
        }

        /// <summary>Retrieves all categories and subcategories in Mitre Matrix json file. It calculates the number of user's custom rules that belong to each subcategory 
        /// and assigns a color to each one based on the percentage of the total number.</summary>
        /// <param name="token">Token from the request's header.</param>
        /// <returns>Data for Logsource Matrix.</returns>
        public List<Category> GetMitreMatrix(string token)
        {
            var userId = RuleUtility.GetUserFromToken(token);
            var rules = _ruleRepository.GetRules(userId).ToList();
            string json = File.ReadAllText("./mitre.json");
            var mitreData = JsonConvert.DeserializeObject<List<Category>>(json);

            var calculatedCategories = MatrixUtility.CalculateMitreCategories(mitreData, rules, _yamlDeserializer);

            var maxCounter = calculatedCategories
            .SelectMany(c => c.Subcategories)
            .Max(s => s.Counter);

            if (maxCounter < 6) maxCounter = 6;

            foreach (var category in calculatedCategories)
            {
                foreach (var subcategory in category.Subcategories)
                {
                    var percentage = subcategory.Counter * 100 / maxCounter;
                    var nearestKey = colors.Keys.OrderBy(k => Math.Abs(k - percentage)).First();
                    subcategory.Color = colors[nearestKey];
                }
            }

            return calculatedCategories;
        }
    }
}

