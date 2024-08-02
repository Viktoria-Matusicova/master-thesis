namespace ruleService.Interfaces
{
    using ruleService.Models;

    public interface IRuleService
    {    
        Task GetTree();
        Task<List<TreeNode>> GetParentTreeAsync();
        string GetCustomTree(string token);
        Task<string> AddNewRuleAsync(CustomRule customRule, string token);
        Rules GetCustomRule(string id);
        Task DeleteSiem(string siemId);
    }
}