using Newtonsoft.Json;
using ruleService.Models;
using ruleService.Services;

namespace ruleService.Utilities
{
    public static class MatrixUtility
    {
        /// <summary>Calculates the number of user's custom rules that belong to each Logsource subcategory.</summary>
        /// <param name="categories">List of existing Logsource categories and their subcategories.</param>
        /// <param name="rules">List of user's custom rules.</param>
        /// <param name="yamlDeserializer"><see cref="YamlDotNet.Serialization.Deserializer"/> instance for deserializng YAML string.</param>
        /// <returns>List of calculated Logsource categories and their subcategories.</returns>
        public static List<Category> CalculateLogsourceCategories(List<Category> categories, List<Rules> rules, YamlDotNet.Serialization.Deserializer yamlDeserializer)
        {
            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    var logSource = rule.Sigma.FrameworkMeta.LogSource;
                    var category = categories.Find(x => x.Name == logSource.Category);
                    if (category != null)
                    {
                        var subcategory = category.Subcategories.Find(y => y.Name == logSource.SubCategory);
                        if (subcategory != null)
                        {
                            subcategory.Counter++;
                            var result = yamlDeserializer.Deserialize<Dictionary<string, dynamic>>(new StringReader(rule.Sigma.Value));
                            string title = result["title"] + ".yml";
                            title = title.ToLower().Replace(" ", "_");
                            subcategory.Rules.Add(new SubcategoryRule { RuleId = rule.Id.ToString(), Name = title });
                        }
                    }
                }
            }

            return categories;
        }
    }
}